using OrderTracking.Domain.Entities;

namespace OrderTracking.Data.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с заказами.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Получает все заказы.
    /// </summary>
    /// <returns>Список всех заказов.</returns>
    Task<IEnumerable<Order>> GetAllAsync();

    /// <summary>
    /// Получает заказ по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>Заказ или null, если не найден.</returns>
    Task<Order?> GetByIdAsync(Guid id);

    /// <summary>
    /// Получает заказ по номеру заказа.
    /// </summary>
    /// <param name="orderNumber">Номер заказа.</param>
    /// <returns>Заказ или null, если не найден.</returns>
    Task<Order?> GetByOrderNumberAsync(string orderNumber);

    /// <summary>
    /// Добавляет новый заказ.
    /// </summary>
    /// <param name="order">Заказ для добавления.</param>
    /// <returns>Добавленный заказ.</returns>
    Task<Order> AddAsync(Order order);

    /// <summary>
    /// Обновляет существующий заказ.
    /// </summary>
    /// <param name="order">Заказ для обновления.</param>
    /// <returns>Обновленный заказ.</returns>
    Task<Order> UpdateAsync(Order order);

    /// <summary>
    /// Удаляет заказ.
    /// </summary>
    /// <param name="order">Заказ для удаления.</param>
    Task DeleteAsync(Order order);
}
