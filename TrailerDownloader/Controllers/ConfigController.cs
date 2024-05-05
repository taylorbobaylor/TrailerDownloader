using Microsoft.AspNetCore.Mvc;
using TrailerDownloader.Models;
using TrailerDownloader.Data;

namespace TrailerDownloader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConfigController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<ConfigController>
        [HttpGet]
        public IActionResult Get()
        {
            var config = _context.Configs.FirstOrDefault();
            if (config == null)
            {
                return NotFound("Configuration not found.");
            }
            return Ok(config);
        }

        // POST api/<ConfigController>
        [HttpPost]
        public IActionResult Post(Config configs)
        {
            _context.Configs.Add(configs);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = configs.Id }, configs);
        }
    }
}
