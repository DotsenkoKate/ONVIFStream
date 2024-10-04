using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharpOnvifServer.Media;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ONVIFStream.Controllers
{
    [Authorize(AuthenticationSchemes = "Digest")]
    [Route("/[controller]")]
    public class ServiceController : Controller
    {
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(ILogger<ServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet("test")]
        public ActionResult<VideoEncoderConfigurationOptions> Get()
        {
            // �������� ����������, ��� ��������� ����������� ����
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // ���� � ����� ������������ ���������� ������
            string relativeFilePath = Path.Combine(basePath, "Config", "VideoEncoderConfigurationOptions.json");

            // ���������, ���������� �� ����
            if (!System.IO.File.Exists(relativeFilePath))
            {
                throw new FileNotFoundException($"���� �� ������: {relativeFilePath}");
            }

            // ������ ���������� ����� JSON
            string json = System.IO.File.ReadAllText(relativeFilePath);

            VideoEncoderConfigurationOptions co = JsonConvert.DeserializeObject<VideoEncoderConfigurationOptions>(json);

            return co;
        }
    }
}