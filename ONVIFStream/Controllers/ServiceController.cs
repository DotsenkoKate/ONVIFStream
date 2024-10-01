using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using ONVIFStream.Models;
using SharpOnvifServer.Media;
using System.Diagnostics;

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
        public ActionResult<string> GetDeviceInfo()
        {
            return "Device Info";
        }
    }
}