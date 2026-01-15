namespace OrderTracking.Domain.Common;

/// <summary>
/// Результат операции, который может быть успешным или содержать ошибку.
/// </summary>
/// <typeparam name="T">Тип данных результата.</typeparam>
public class Result<T>
{
    /// <summary>
    /// Значение, если операция успешна.
    /// </summary>
    public T? Value { get; private set; }

    /// <summary>
    /// Сообщение об ошибке, если операция неуспешна.
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// Код ошибки HTTP, если операция неуспешна.
    /// </summary>
    public int? ErrorCode { get; private set; }

    /// <summary>
    /// Указывает, успешна ли операция.
    /// </summary>
    public bool IsSuccess => Error == null;

    /// <summary>
    /// Указывает, неуспешна ли операция.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    private Result(T? value, string? error, int? errorCode = null)
    {
        Value = value;
        Error = error;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Создает успешный результат.
    /// </summary>
    /// <param name="value">Значение результата.</param>
    /// <returns>Успешный результат.</returns>
    public static Result<T> Success(T value)
    {
        return new Result<T>(value, null);
    }

    /// <summary>
    /// Создает неуспешный результат.
    /// </summary>
    /// <param name="error">Сообщение об ошибке.</param>
    /// <param name="errorCode">Код ошибки HTTP.</param>
    /// <returns>Неуспешный результат.</returns>
    public static Result<T> Failure(string error, int errorCode = 400)
    {
        return new Result<T>(default, error, errorCode);
    }

    /// <summary>
    /// Создает неуспешный результат с кодом 404.
    /// </summary>
    /// <param name="error">Сообщение об ошибке.</param>
    /// <returns>Неуспешный результат с кодом 404.</returns>
    public static Result<T> NotFound(string error = "Ресурс не найден")
    {
        return new Result<T>(default, error, 404);
    }
}

/// <summary>
/// Результат операции без возвращаемого значения.
/// </summary>
public class Result
{
    /// <summary>
    /// Сообщение об ошибке, если операция неуспешна.
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// Код ошибки HTTP, если операция неуспешна.
    /// </summary>
    public int? ErrorCode { get; private set; }

    /// <summary>
    /// Указывает, успешна ли операция.
    /// </summary>
    public bool IsSuccess => Error == null;

    /// <summary>
    /// Указывает, неуспешна ли операция.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    private Result(string? error, int? errorCode = null)
    {
        Error = error;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Создает успешный результат.
    /// </summary>
    /// <returns>Успешный результат.</returns>
    public static Result Success()
    {
        return new Result(null);
    }

    /// <summary>
    /// Создает неуспешный результат.
    /// </summary>
    /// <param name="error">Сообщение об ошибке.</param>
    /// <param name="errorCode">Код ошибки HTTP.</param>
    /// <returns>Неуспешный результат.</returns>
    public static Result Failure(string error, int errorCode = 400)
    {
        return new Result(error, errorCode);
    }

    /// <summary>
    /// Создает неуспешный результат с кодом 404.
    /// </summary>
    /// <param name="error">Сообщение об ошибке.</param>
    /// <returns>Неуспешный результат с кодом 404.</returns>
    public static Result NotFound(string error = "Ресурс не найден")
    {
        return new Result(error, 404);
    }
}
