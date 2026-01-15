namespace OrderTracking.Domain.Common
{
    /// <summary>
    /// Стандартизированный ответ API.
    /// </summary>
    /// <typeparam name="T">Тип данных в ответе.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Указывает, успешен ли запрос.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Данные ответа.
        /// </summary>
        public T? Value { get; set; }

        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Код ошибки HTTP.
        /// </summary>
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Создает успешный ответ.
        /// </summary>
        /// <param name="value">Данные ответа.</param>
        /// <returns>Успешный ответ.</returns>
        public static ApiResponse<T> Success(T value)
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Value = value
            };
        }

        /// <summary>
        /// Создает ответ с ошибкой.
        /// </summary>
        /// <param name="error">Сообщение об ошибке.</param>
        /// <param name="errorCode">Код ошибки HTTP.</param>
        /// <returns>Ответ с ошибкой.</returns>
        public static ApiResponse<T> Failure(string error, int errorCode = 400)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Error = error,
                ErrorCode = errorCode
            };
        }
    }

}
