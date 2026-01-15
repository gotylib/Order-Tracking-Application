using Microsoft.AspNetCore.Mvc;
using OrderTracking.Domain.Common;
using OrderTracking.Domain.DTOs;
using OrderTracking.Service.Interfaces;

namespace OrderTracking.API.Controllers;

/// <summary>
/// Контроллер для работы с заказами.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrdersController"/>.
    /// </summary>
    /// <param name="orderService">Сервис для работы с заказами.</param>
    /// <param name="logger">Логгер.</param>
    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Получает список всех заказов.
    /// </summary>
    /// <returns>Результат со списком заказов.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrders()
    {
        var result = await _orderService.GetAllOrdersAsync();
        
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<IEnumerable<OrderDto>>.Success(result.Value!));
        }

        return StatusCode(result.ErrorCode ?? 500, ApiResponse<object>.Failure(result.Error!, result.ErrorCode ?? 500));
    }

    /// <summary>
    /// Получает заказ по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <returns>Результат с заказом или ошибкой.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(Guid id)
    {
        var result = await _orderService.GetOrderByIdAsync(id);
        
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<OrderDto>.Success(result.Value!));
        }

        return StatusCode(result.ErrorCode ?? 500, ApiResponse<object>.Failure(result.Error!, result.ErrorCode ?? 500));
    }

    /// <summary>
    /// Создает новый заказ.
    /// </summary>
    /// <param name="createOrderDto">Данные для создания заказа.</param>
    /// <returns>Результат с созданным заказом или ошибкой.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Failure("Неверные данные запроса", 400));
        }

        var result = await _orderService.CreateOrderAsync(createOrderDto);
        
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetOrder), new { id = result.Value!.Id }, 
                ApiResponse<OrderDto>.Success(result.Value));
        }

        return StatusCode(result.ErrorCode ?? 500, ApiResponse<object>.Failure(result.Error!, result.ErrorCode ?? 500));
    }

    /// <summary>
    /// Обновляет статус заказа.
    /// </summary>
    /// <param name="id">Идентификатор заказа.</param>
    /// <param name="updateOrderStatusDto">Данные для обновления статуса.</param>
    /// <returns>Результат с обновленным заказом или ошибкой.</returns>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrderStatus(
        Guid id,
        [FromBody] UpdateOrderStatusDto updateOrderStatusDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Failure("Неверные данные запроса", 400));
        }

        var result = await _orderService.UpdateOrderStatusAsync(id, updateOrderStatusDto);
        
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<OrderDto>.Success(result.Value!));
        }

        return StatusCode(result.ErrorCode ?? 500, ApiResponse<object>.Failure(result.Error!, result.ErrorCode ?? 500));
    }
}
