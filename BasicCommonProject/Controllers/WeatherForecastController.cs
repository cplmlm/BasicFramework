using BasicCommonProject.Result;
using IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.ViewModels;

namespace BasicCommonProject.Controllers
{
    [ApiController]
    public class WeatherForecastController : ApiControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ITest iTest;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,ITest iTest)
        {
            _logger = logger;
            this.iTest = iTest;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet]
        public ResponseResult<IEnumerable<WeatherForecast>> GetAll()
        {
            _logger.LogInformation("About page visited at {DT}",
           DateTime.UtcNow.ToLongTimeString());
            var datas = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });
            return datas.ToList();
        }
        [HttpPost]
        public ResponseResult<string> GetAll111()
        {
            int a = 1;
            int b = 0;

            return FailResult<string>("1111");
        }
        [HttpPost]
        public async Task<ResponseResult<BlogViewModels>> GetTotal(int id)
        {
           BlogViewModels result = await iTest.GetBlog(id);
            return SuccessResult<BlogViewModels>(result);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<string> GetAll1211()
        {
            int a = 1;
            int b = 0;

            return "11333";
        }
        [HttpGet]
        [Authorize(Policy = "SystemOrAdmin")]
        public ActionResult<IEnumerable<string>> Get223324()
        {
            return new string[] { "value1", "value2" };
        }
       
    }
}
