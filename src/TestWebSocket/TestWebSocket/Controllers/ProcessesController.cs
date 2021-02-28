using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TestWebSocket.Services;

namespace TestWebSocket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessesController : ControllerBase
    {
        private readonly ILogger<ProcessesController> _logger;
        private readonly IProcessInfoStorage _processInfoStorage;

        public ProcessesController(ILogger<ProcessesController> logger, IProcessInfoStorage processInfoStorage)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _processInfoStorage = processInfoStorage ?? throw new ArgumentNullException(nameof(processInfoStorage));
        }

        /// <summary>
        /// Provides information about processes
        /// </summary>
        /// <returns>Collection of active processes</returns>
        [HttpGet]
        public IActionResult GetProcesses()
        {
            try
            {
                return Ok(_processInfoStorage.ProcessInfo);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Something went wrong.");
                throw;
            }
        }
    }
}
