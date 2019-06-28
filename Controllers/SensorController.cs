using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Apex.RaspberryPiServer.Models;
using Apex.RaspberryPiServer.Sensors;

namespace Apex.RaspberryPiServer.Controllers
{
    public class SensorController : ControllerBase
    {
        readonly ISensorService sensorService;

        public SensorController(ISensorService sensorService)
        {
            this.sensorService = sensorService;
        }

        [HttpGet("api/check")]
        public IActionResult Check()
        {
            return Ok("Connection is ok");
        }

        [Route("api/img/{file}")]
        public async Task<IActionResult> GetImage([FromServices] ICameraService cameraService, string fileName)
        {
            var imageFileStream = await cameraService.GetImage();
            return File(imageFileStream, "image/jpeg", fileName);            
        }

        [Route("api/distance")]
        public async Task<IActionResult> GetDistance()
        {
            double distance = await sensorService.ReadDistance();
            return Ok(distance);
        }

        [Route("api/temperature")]
        public async Task<IActionResult> GetTemperature()
        {
            double temperature = await sensorService.ReadTemperature();
            return Ok(temperature);
        }

        [Route("api/luminosity")]
        public async Task<IActionResult> GetLuminosity()
        {
            var luminosity = await sensorService.ReadLuminosity();
            return Ok(luminosity);
        }

        [HttpGet("api/hasinfra")]
        public async Task<IActionResult> HasInfrared()
        {
            var infra = await sensorService.HasInfrared(25);
            return Ok(infra);
        }

        [HttpGet("api/infra")]
        public async Task<IActionResult> GetInfrared()
        {
            var infra = await sensorService.ReadInfrared();
            return Ok(infra);
        }

        [Route("api/sensor/data")]
        public string GetReadings([FromServices] SensorsDBContext context)
        {
            var readings = new StringBuilder();
            foreach (var reading in context.Readings)
            {
                readings.Append($"{reading.Temperature},{reading.Luminosity},{reading.Infrared},{reading.Distance},{reading.Source}\r\n");
            }
            return readings.ToString();
        }
    }
}
