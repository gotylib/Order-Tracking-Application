using Microsoft.EntityFrameworkCore;
using OrderTracking.Domain.Entities;

namespace OrderTracking.Data;

/// <summary>
/// Контекст базы данных для работы с заказами.
/// </summary>
public class OrderTrackingDbContext : DbContext
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderTrackingDbContext"/>.
    /// </summary>
    /// <param name="options">Параметры конфигурации контекста базы данных.</param>
    public OrderTrackingDbContext(DbContextOptions<OrderTrackingDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Набор данных заказов.
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// Настраивает модель данных при создании контекста.
    /// </summary>
    /// <param name="modelBuilder">Построитель модели данных.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasIndex(e => e.OrderNumber).IsUnique();
        });
    }
}
