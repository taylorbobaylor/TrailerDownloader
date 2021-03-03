using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using TrailerDownloader.Models;
using TrailerDownloader.Services.Interfaces;

namespace TrailerDownloader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigService _configService;

        public ConfigController(IConfigService configService)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        // GET: api/<ConfigController>
        [HttpGet]
        public IActionResult GetConfig()
        {
            try
            {
                Config result = _configService.GetConfig();
                if (result == null)
                {
                    Log.Error("No config found");
                    return NotFound("No config found");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in ConfigController.GetConfig");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // POST api/<ConfigController>
        [HttpPost]
        public IActionResult SaveConfig(Config config)
        {
            try
            {
                bool result = _configService.SaveConfig(config.BaseMediaPath);
                if (!result)
                {
                    return NotFound("Path does not exist");
                }

                Log.Information($"Path: {config.BaseMediaPath} saved successfully");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in ConfigController.SaveConfig");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving config");
            }
        }
    }
}
