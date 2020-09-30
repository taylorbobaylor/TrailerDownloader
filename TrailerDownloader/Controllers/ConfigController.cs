using Microsoft.AspNetCore.Mvc;
using TrailerDownloader.Models;
using TrailerDownloader.Repositories;

namespace TrailerDownloader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigRepository _configRepository;

        public ConfigController(IConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }

        // GET: api/<ConfigController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_configRepository.GetConfig());
        }

        // POST api/<ConfigController>
        [HttpPost]
        public IActionResult Post(Config configs)
        {
            return Ok(_configRepository.SaveConfig(configs));
        }
    }
}
