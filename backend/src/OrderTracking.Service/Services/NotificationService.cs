using Microsoft.Extensions.Logging;
using OrderTracking.Domain.Events;
using OrderTracking.Service.Interfaces;

namespace OrderTracking.Service.Services;

/// <summary>
/// Сервис для отправки уведомлений через WebSocket.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IHubContextWrapper _hubContextWrapper;
    private readonly ILogger<NotificationService> _logger;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="NotificationService"/>.
    /// </summary>
    /// <param name="hubContextWrapper">Обертка над SignalR Hub Context.</param>
    /// <param name="logger">Логгер.</param>
    public NotificationService(IHubContextWrapper hubContextWrapper, ILogger<NotificationService> logger)
    {
        _hubContextWrapper = hubContextWrapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task NotifyOrderStatusChangedAsync(OrderStatusChangedEvent @event)
    {
        try
        {
            await _hubContextWrapper.SendOrderStatusChangedAsync(@event);
            _logger.LogInformation("Уведомление о изменении статуса заказа {OrderId} отправлено через WebSocket",
                @event.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке уведомления через WebSocket для заказа {OrderId}",
                @event.OrderId);
        }
    }
}
