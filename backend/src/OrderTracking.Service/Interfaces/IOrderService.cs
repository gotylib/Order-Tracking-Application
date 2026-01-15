using OrderTracking.Domain.Common;
using OrderTracking.Domain.DTOs;

namespace OrderTracking.Service.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с заказами.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Получает все заказы.
    /// </summary>
    /// <returns>Результат со списком всех заказов.</returns>
    Task<Result<IEnumerable<OrderDto>>> GetAllOrdersAsync();

    /// <summary>
    /// Получает заказ по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>Результат с заказом или ошибкой.</returns>
    Task<Result<OrderDto>> GetOrderByIdAsync(Guid id);

    /// <summary>
    /// Создает новый заказ.
    /// </summary>
    /// <param name="createOrderDto">DTO для создания заказа.</param>
    /// <returns>Результат с созданным заказом или ошибкой.</returns>
    Task<Result<OrderDto>> CreateOrderAsync(CreateOrderDto createOrderDto);

    /// <summary>
    /// Обновляет статус заказа.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="updateOrderStatusDto">DTO для обновления статуса.</param>
    /// <returns>Результат с обновленным заказом или ошибкой.</returns>
    Task<Result<OrderDto>> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto updateOrderStatusDto);
}
