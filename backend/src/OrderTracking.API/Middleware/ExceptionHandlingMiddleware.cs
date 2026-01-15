using System.Net;
using System.Text.Json;
using OrderTracking.Domain.Common;

namespace OrderTracking.API.Middleware;

/// <summary>
/// Middleware для обработки исключений и возврата стандартизированных ответов.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="ExceptionHandlingMiddleware"/>.
    /// </summary>
    /// <param name="next">Следующий middleware в цепочке.</param>
    /// <param name="logger">Логгер.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Обрабатывает HTTP запрос и перехватывает исключения.
    /// </summary>
    /// <param name="context">HTTP контекст.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Необработанное исключение: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Обрабатывает исключение и возвращает стандартизированный ответ.
    /// </summary>
    /// <param name="context">HTTP контекст.</param>
    /// <param name="exception">Исключение.</param>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ArgumentNullException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var result = Result<object>.Failure(
            exception.Message,
            context.Response.StatusCode
        );

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(new
        {
            isSuccess = result.IsSuccess,
            value = result.Value,
            error = result.Error,
            errorCode = result.ErrorCode
        }, options);

        await context.Response.WriteAsync(json);
    }
}
