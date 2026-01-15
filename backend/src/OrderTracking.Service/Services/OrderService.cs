using Microsoft.Extensions.Logging;
using OrderTracking.Domain.Common;
using OrderTracking.Domain.DTOs;
using OrderTracking.Domain.Entities;
using OrderTracking.Domain.Enums;
using OrderTracking.Domain.Events;
using OrderTracking.Data.Repositories;
using OrderTracking.Service.Interfaces;
using OrderTracking.Service.Mappings;

namespace OrderTracking.Service.Services;

/// <summary>
/// Сервис для работы с заказами.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<OrderService> _logger;
    private readonly OrderMapper _mapper;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderService"/>.
    /// </summary>
    /// <param name="orderRepository">Репозиторий заказов.</param>
    /// <param name="messagePublisher">Публикатор сообщений.</param>
    /// <param name="logger">Логгер.</param>
    public OrderService(
        IOrderRepository orderRepository,
        IMessagePublisher messagePublisher,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _messagePublisher = messagePublisher;
        _logger = logger;
        _mapper = new OrderMapper();
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<OrderDto>>> GetAllOrdersAsync()
    {
        try
        {
            _logger.LogInformation("Получение списка всех заказов");
            var orders = await _orderRepository.GetAllAsync();
            var orderDtos = _mapper.ToOrderDtoList(orders);
            return Result<IEnumerable<OrderDto>>.Success(orderDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка заказов");
            return Result<IEnumerable<OrderDto>>.Failure("Ошибка при получении списка заказов", 500);
        }
    }

    /// <inheritdoc/>
    public async Task<Result<OrderDto>> GetOrderByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Получение заказа по идентификатору: {OrderId}", id);
            var order = await _orderRepository.GetByIdAsync(id);
            
            if (order == null)
            {
                _logger.LogWarning("Заказ с идентификатором {OrderId} не найден", id);
                return Result<OrderDto>.NotFound($"Заказ с идентификатором {id} не найден");
            }

            var orderDto = _mapper.ToOrderDto(order);
            return Result<OrderDto>.Success(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении заказа {OrderId}", id);
            return Result<OrderDto>.Failure("Ошибка при получении заказа", 500);
        }
    }

    /// <inheritdoc/>
    public async Task<Result<OrderDto>> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        try
        {
            _logger.LogInformation("Создание нового заказа с номером: {OrderNumber}", createOrderDto.OrderNumber);

            var existingOrder = await _orderRepository.GetByOrderNumberAsync(createOrderDto.OrderNumber);
            if (existingOrder != null)
            {
                _logger.LogWarning("Заказ с номером {OrderNumber} уже существует", createOrderDto.OrderNumber);
                return Result<OrderDto>.Failure($"Заказ с номером {createOrderDto.OrderNumber} уже существует", 400);
            }

            var order = _mapper.ToOrderWithDefaults(createOrderDto);

            var createdOrder = await _orderRepository.AddAsync(order);
            _logger.LogInformation("Заказ успешно создан с идентификатором: {OrderId}", createdOrder.Id);

            var orderDto = _mapper.ToOrderDto(createdOrder);
            return Result<OrderDto>.Success(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании заказа");
            return Result<OrderDto>.Failure("Ошибка при создании заказа", 500);
        }
    }

    /// <inheritdoc/>
    public async Task<Result<OrderDto>> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto updateOrderStatusDto)
    {
        try
        {
            _logger.LogInformation("Обновление статуса заказа {OrderId} на {NewStatus}", id, updateOrderStatusDto.Status);

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Заказ с идентификатором {OrderId} не найден", id);
                return Result<OrderDto>.NotFound($"Заказ с идентификатором {id} не найден");
            }

            var previousStatus = order.Status;
            order.Status = updateOrderStatusDto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            var updatedOrder = await _orderRepository.UpdateAsync(order);
            _logger.LogInformation("Статус заказа {OrderId} успешно обновлен с {PreviousStatus} на {NewStatus}",
                id, previousStatus, updateOrderStatusDto.Status);

            // Асинхронная отправка события в RabbitMQ
            var statusChangedEvent = new OrderStatusChangedEvent
            {
                OrderId = updatedOrder.Id,
                OrderNumber = updatedOrder.OrderNumber,
                PreviousStatus = previousStatus,
                NewStatus = updateOrderStatusDto.Status,
                ChangedAt = DateTime.UtcNow
            };

            // Отправляем событие асинхронно, не блокируя основной поток
            _ = Task.Run(async () =>
            {
                try
                {
                    await _messagePublisher.PublishOrderStatusChangedAsync(statusChangedEvent);
                    _logger.LogInformation("Событие изменения статуса заказа {OrderId} отправлено в очередь",
                        updatedOrder.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при отправке события изменения статуса заказа {OrderId}",
                        updatedOrder.Id);
                }
            });

            var orderDto = _mapper.ToOrderDto(updatedOrder);
            return Result<OrderDto>.Success(orderDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении статуса заказа {OrderId}", id);
            return Result<OrderDto>.Failure("Ошибка при обновлении статуса заказа", 500);
        }
    }
}
