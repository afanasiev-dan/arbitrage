using Arbitrage.Domain;
using Arbitrage.ExchangeDomain;
using Microsoft.AspNetCore.Mvc;

namespace Arbitrage.WebApi.Presentation
{
    public class AhahahaController(
        ILogger<AhahahaController> logger
    ) : ControllerBase
    {
        private readonly ILogger<AhahahaController> _logger = logger;

        [HttpPost("proxy-huyoksi")]
        public async Task<IActionResult> Proxy([FromQuery] string url)
        {
            if (string.IsNullOrEmpty(url))
                return BadRequest(new ApiResponce() { RetMsg = "Не указан URL" });

            try
            {
                var response = await Network.PostAsync(url);
                return Content(response, "application/json");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка при проксировании запроса");
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponce() { RetMsg = "Ошибка при проксировании запроса" });
            }
        }
    }
}