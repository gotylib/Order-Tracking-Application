using Microsoft.EntityFrameworkCore;
using OrderTracking.Domain.Entities;
using OrderTracking.Data.Repositories;

namespace OrderTracking.Data.Repositories;

/// <summary>
/// Реализация репозитория для работы с заказами.
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly OrderTrackingDbContext _context;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderRepository"/>.
    /// </summary>
    /// <param name="context">Контекст базы данных.</param>
    public OrderRepository(OrderTrackingDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders.FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    /// <inheritdoc/>
    public async Task<Order> AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
    }

    /// <inheritdoc/>
    public async Task<Order> UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }
}
