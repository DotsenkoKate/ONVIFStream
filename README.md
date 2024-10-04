# Что мы тут наделали?
Мы взяли библиотеку [SharpOnvif](https://github.com/jimm98y/SharpOnvif/tree/main?tab=readme-ov-file) и использовали ее возможности для развертывания сервера onvif. 
Ссылки и профили сервер отдает. Эти ссылки (Links) и профили (Profiles) нужно описать в JSON файле: "media_settings.json". Также, если нужно можно написать общую информацию по камере в файле "device_settings.json" (хз зачем, но можно).

# Как перенести в проект?
Если не разворачивать это приложение отдельно, то нужно сделать следующие вещи:
* подключить [Newtonsoft.Json](https://www.newtonsoft.com/json), [SharpOnvifServer](https://github.com/jimm98y/SharpOnvif), SharpOnvifServer.DeviceMgmt, SharpOnvifServer.Media;
* перенести классы DeviceImpl, JSONReader, MediaImpl, UserRepository в свой проект + перенести всю папку Config;
* добавить в Program.cs сервисы:
```C#
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddOnvifDigestAuthentication(); 
builder.Services.AddOnvifDiscovery();

builder.Services.AddSingleton((sp) => { return new ONVIFStream.DeviceImpl(sp.GetService<IServer>()); });
builder.Services.AddSingleton((sp) => { return new ONVIFStream.MediaImpl(sp.GetService<IServer>()); });
...
app.UseAuthentication();
app.UseStaticFiles();
app.UseOnvif();

((IApplicationBuilder)app).UseServiceModel(serviceBuilder =>
{
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;

    serviceBuilder.AddService<ONVIFStream.DeviceImpl>();
    serviceBuilder.AddServiceEndpoint<ONVIFStream.DeviceImpl, SharpOnvifServer.DeviceMgmt.Device>(OnvifBindingFactory.CreateBinding(), "/onvif/device_service");

    serviceBuilder.AddService<ONVIFStream.MediaImpl>();
    serviceBuilder.AddServiceEndpoint<ONVIFStream.MediaImpl, SharpOnvifServer.Media.Media>(OnvifBindingFactory.CreateBinding(), "/onvif/media_service");
});
...
```
* изменить в UserRepository логин и пароль пользователя.
