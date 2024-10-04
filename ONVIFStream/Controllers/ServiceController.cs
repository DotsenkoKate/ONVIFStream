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
            // Получаем директорию, где находится исполняемый файл
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // Путь к файлу относительно директории сборки
            string relativeFilePath = Path.Combine(basePath, "Config", "VideoEncoderConfigurationOptions.json");

            // Проверяем, существует ли файл
            if (!System.IO.File.Exists(relativeFilePath))
            {
                throw new FileNotFoundException($"Файл не найден: {relativeFilePath}");
            }

            // Читаем содержимое файла JSON
            string json = System.IO.File.ReadAllText(relativeFilePath);

            VideoEncoderConfigurationOptions co = JsonConvert.DeserializeObject<VideoEncoderConfigurationOptions>(json);

            return co;
        }
    }
}