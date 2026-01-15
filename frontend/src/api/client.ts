import axios, { AxiosInstance, AxiosError } from 'axios';
import { ApiResponse } from '../types/api';

/**
 * Базовый API клиент с настройкой axios.
 */
class ApiClient {
  private client: AxiosInstance;

  constructor(baseURL: string = '/api') {
    this.client = axios.create({
      baseURL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Interceptor для обработки ошибок
    this.client.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        // Логируем ошибку для отладки
        console.error('API Error:', error);
        return Promise.reject(error);
      }
    );
  }

  /**
   * Обрабатывает ответ API и извлекает данные или ошибку.
   */
  private handleResponse<T>(response: { data: ApiResponse<T> }): T {
    if (response.data.isSuccess && response.data.value) {
      return response.data.value;
    }
    throw new Error(response.data.error || 'Неизвестная ошибка API');
  }

  /**
   * Обрабатывает ошибку API и возвращает понятное сообщение.
   */
  private handleError(error: unknown): string {
    if (axios.isAxiosError(error)) {
      if (error.response?.data) {
        const apiResponse = error.response.data as ApiResponse<unknown>;
        return apiResponse.error || error.response.statusText || 'Ошибка сервера';
      } else if (error.request) {
        return 'Нет ответа от сервера. Проверьте подключение.';
      }
    }
    return error instanceof Error ? error.message : 'Неизвестная ошибка';
  }

  /**
   * Выполняет GET запрос.
   */
  async get<T>(url: string): Promise<T> {
    try {
      const response = await this.client.get<ApiResponse<T>>(url);
      return this.handleResponse(response);
    } catch (error) {
      throw new Error(this.handleError(error));
    }
  }

  /**
   * Выполняет POST запрос.
   */
  async post<T>(url: string, data?: unknown): Promise<T> {
    try {
      const response = await this.client.post<ApiResponse<T>>(url, data);
      return this.handleResponse(response);
    } catch (error) {
      throw new Error(this.handleError(error));
    }
  }

  /**
   * Выполняет PUT запрос.
   */
  async put<T>(url: string, data?: unknown): Promise<T> {
    try {
      const response = await this.client.put<ApiResponse<T>>(url, data);
      return this.handleResponse(response);
    } catch (error) {
      throw new Error(this.handleError(error));
    }
  }

  /**
   * Выполняет DELETE запрос.
   */
  async delete<T>(url: string): Promise<T> {
    try {
      const response = await this.client.delete<ApiResponse<T>>(url);
      return this.handleResponse(response);
    } catch (error) {
      throw new Error(this.handleError(error));
    }
  }
}

// Создаем и экспортируем экземпляр API клиента
const apiClient = new ApiClient(process.env.REACT_APP_API_URL || '/api');

export default apiClient;
