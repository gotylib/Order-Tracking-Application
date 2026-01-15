using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrderTracking.Domain.Common;
using OrderTracking.Domain.DTOs;
using OrderTracking.Domain.Entities;
using OrderTracking.Domain.Enums;
using OrderTracking.Data.Repositories;
using OrderTracking.Service.Interfaces;
using OrderTracking.Service.Services;
using Xunit;

namespace OrderTracking.Service.Tests.Services;

/// <summary>
/// Тесты для сервиса заказов.
/// </summary>
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IMessagePublisher> _mockMessagePublisher;
    private readonly Mock<ILogger<OrderService>> _mockLogger;
    private readonly OrderService _orderService;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderServiceTests"/>.
    /// </summary>
    public OrderServiceTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockMessagePublisher = new Mock<IMessagePublisher>();
        _mockLogger = new Mock<ILogger<OrderService>>();
        _orderService = new OrderService(_mockRepository.Object, _mockMessagePublisher.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new()
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD-001",
                Description = "Test Order 1",
                Status = OrderStatus.Created,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD-002",
                Description = "Test Order 2",
                Status = OrderStatus.Sent,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);

        // Act
        var result = await _orderService.GetAllOrdersAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(2);
        result.Value!.First().OrderNumber.Should().Be("ORD-001");
    }

    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderExists_ShouldReturnOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            OrderNumber = "ORD-001",
            Description = "Test Order",
            Status = OrderStatus.Created,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

        // Act
        var result = await _orderService.GetOrderByIdAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(orderId);
        result.Value!.OrderNumber.Should().Be("ORD-001");
    }

    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

        // Act
        var result = await _orderService.GetOrderByIdAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(404);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateOrder()
    {
        // Arrange
        var createOrderDto = new CreateOrderDto
        {
            OrderNumber = "ORD-001",
            Description = "Test Order"
        };

        _mockRepository.Setup(r => r.GetByOrderNumberAsync(createOrderDto.OrderNumber))
            .ReturnsAsync((Order?)null);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order order) => order);

        // Act
        var result = await _orderService.CreateOrderAsync(createOrderDto);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.OrderNumber.Should().Be(createOrderDto.OrderNumber);
        result.Value!.Description.Should().Be(createOrderDto.Description);
        result.Value!.Status.Should().Be(OrderStatus.Created);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenOrderNumberExists_ShouldReturnFailure()
    {
        // Arrange
        var createOrderDto = new CreateOrderDto
        {
            OrderNumber = "ORD-001",
            Description = "Test Order"
        };

        var existingOrder = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            Description = "Existing Order",
            Status = OrderStatus.Created,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByOrderNumberAsync(createOrderDto.OrderNumber))
            .ReturnsAsync(existingOrder);

        // Act
        var result = await _orderService.CreateOrderAsync(createOrderDto);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(400);
        result.Error.Should().Contain("ORD-001");
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenOrderExists_ShouldUpdateStatus()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            OrderNumber = "ORD-001",
            Description = "Test Order",
            Status = OrderStatus.Created,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var updateOrderStatusDto = new UpdateOrderStatusDto
        {
            Status = OrderStatus.Sent
        };

        _mockRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _orderService.UpdateOrderStatusAsync(orderId, updateOrderStatusDto);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be(OrderStatus.Sent);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenOrderNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var updateOrderStatusDto = new UpdateOrderStatusDto
        {
            Status = OrderStatus.Sent
        };

        _mockRepository.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

        // Act
        var result = await _orderService.UpdateOrderStatusAsync(orderId, updateOrderStatusDto);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be(404);
    }
}
