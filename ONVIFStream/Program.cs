using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.OpenApi.Models;
using SharpOnvifCommon;
using SharpOnvifServer;

var builder = WebApplication.CreateBuilder(args);

// Добавляем Swagger генерацию
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ONVIF API",
        Version = "v1",
        Description = "ONVIF-compatible API for device and media management",
    });
});

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.Services.AddControllers();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddOnvifDigestAuthentication(); 
builder.Services.AddOnvifDiscovery();

builder.Services.AddSingleton((sp) => { return new ONVIFStream.DeviceImpl(sp.GetService<IServer>()); });
builder.Services.AddSingleton((sp) => { return new ONVIFStream.MediaImpl(sp.GetService<IServer>()); });
builder.Services.AddSingleton((sp) => { return new ONVIFStream.EventsImpl(sp.GetService<IServer>()); });
//builder.Services.AddSingleton((sp) => { return new ONVIFStream.SubscriptionManagerImpl(sp.GetService<IServer>()); });

var app = builder.Build();

// Включаем Swagger и Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ONVIF API v1");
        c.RoutePrefix = string.Empty; // Путь для доступа к Swagger UI
    });
}

app.UseAuthentication();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseOnvif();

((IApplicationBuilder)app).UseServiceModel(serviceBuilder =>
{
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;

    serviceBuilder.AddService<ONVIFStream.DeviceImpl>();
    serviceBuilder.AddServiceEndpoint<ONVIFStream.DeviceImpl, SharpOnvifServer.DeviceMgmt.Device>(OnvifBindingFactory.CreateBinding(), "/onvif/device_service");

    serviceBuilder.AddService<ONVIFStream.MediaImpl>();
    serviceBuilder.AddServiceEndpoint<ONVIFStream.MediaImpl, SharpOnvifServer.Media.Media>(OnvifBindingFactory.CreateBinding(), "/onvif/media_service");

    var eventBinding = OnvifBindingFactory.CreateBinding();
    serviceBuilder.AddService<ONVIFStream.EventsImpl>();
    serviceBuilder.AddServiceEndpoint<ONVIFStream.EventsImpl, SharpOnvifServer.Events.NotificationProducer>(eventBinding, "/onvif/events_service");
    serviceBuilder.AddServiceEndpoint<ONVIFStream.EventsImpl, SharpOnvifServer.Events.EventPortType>(eventBinding, "/onvif/events_service");
    serviceBuilder.AddServiceEndpoint<ONVIFStream.EventsImpl, SharpOnvifServer.Events.PullPoint>(eventBinding, "/onvif/events_service");

    //    var subscriptionBinding = OnvifBindingFactory.CreateBinding();
    //    serviceBuilder.AddService<ONVIFStream.SubscriptionManagerImpl>();
    //    serviceBuilder.AddServiceEndpoint<ONVIFStream.SubscriptionManagerImpl, SharpOnvifServer.Events.SubscriptionManager>(subscriptionBinding, "/onvif/Events/SubManager");
    //    serviceBuilder.AddServiceEndpoint<ONVIFStream.SubscriptionManagerImpl, SharpOnvifServer.Events.PausableSubscriptionManager>(subscriptionBinding, "/onvif/Events/SubManager");
    //    serviceBuilder.AddServiceEndpoint<ONVIFStream.SubscriptionManagerImpl, SharpOnvifServer.Events.PullPointSubscription>(subscriptionBinding, "/onvif/Events/SubManager");

});

app.Run();
