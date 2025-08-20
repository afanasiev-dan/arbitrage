using Arbitrage.Notification.Presentation.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Arbitrage.Notification.Presentation
{

    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto)
        {
            var created = await _service.CreateNotificationAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateNotificationDto dto)
        {
            return await _service.UpdateNotificationAsync(dto) ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
            => await _service.DeleteNotificationAsync(id) ? NoContent() : NotFound();

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var notification = await _service.GetNotificationAsync(id);
            return notification != null ? Ok(notification) : NotFound();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var notifications = await _service.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        [HttpGet("currencypair/{currencyPairId}")]
        public async Task<IActionResult> GetByCurrencyPair(Guid currencyPairId)
        {
            var notifications = await _service.GetCurrencyPairNotificationsAsync(currencyPairId);
            return Ok(notifications);
        }
    }
}