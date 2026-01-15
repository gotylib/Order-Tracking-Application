import apiClient from './client';
import { OrderDto, CreateOrderDto, UpdateOrderStatusDto } from '../types/order';

/**
 * API функции для работы с заказами.
 */
export const ordersApi = {
  /**
   * Получает список всех заказов.
   */
  getAllOrders: async (): Promise<OrderDto[]> => {
    return apiClient.get<OrderDto[]>('/orders');
  },

  /**
   * Получает заказ по идентификатору.
   */
  getOrderById: async (id: string): Promise<OrderDto> => {
    return apiClient.get<OrderDto>(`/orders/${id}`);
  },

  /**
   * Создает новый заказ.
   */
  createOrder: async (order: CreateOrderDto): Promise<OrderDto> => {
    return apiClient.post<OrderDto>('/orders', order);
  },

  /**
   * Обновляет статус заказа.
   */
  updateOrderStatus: async (id: string, status: UpdateOrderStatusDto): Promise<OrderDto> => {
    return apiClient.put<OrderDto>(`/orders/${id}/status`, status);
  },
};
