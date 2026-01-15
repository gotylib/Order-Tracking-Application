using Microsoft.AspNetCore.SignalR;
using OrderTracking.Domain.Events;
using OrderTracking.Service.Interfaces;

namespace OrderTracking.API.Services;

/// <summary>
/// Реализация обертки над SignalR Hub Context.
/// </summary>
public class SignalRHubContextWrapper : IHubContextWrapper
{
    private readonly IHubContext<Hubs.OrderTrackingHub> _hubContext;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="SignalRHubContextWrapper"/>.
    /// </summary>
    /// <param name="hubContext">Контекст SignalR Hub.</param>
    public SignalRHubContextWrapper(IHubContext<Hubs.OrderTrackingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <inheritdoc/>
    public async Task SendOrderStatusChangedAsync(OrderStatusChangedEvent @event)
    {
        await _hubContext.Clients.All.SendAsync("OrderStatusChanged", @event);
    }
}
