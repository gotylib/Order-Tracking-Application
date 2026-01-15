export enum OrderStatus {
  Created = 0,
  Sent = 1,
  Delivered = 2,
  Cancelled = 3,
}

export interface OrderDto {
  id: string;
  orderNumber: string;
  description: string;
  status: OrderStatus;
  createdAt: string;
  updatedAt: string;
}

export interface CreateOrderDto {
  orderNumber: string;
  description: string;
}

export interface UpdateOrderStatusDto {
  status: OrderStatus;
}

export interface OrderStatusChangedEvent {
  orderId: string;
  orderNumber: string;
  previousStatus: OrderStatus;
  newStatus: OrderStatus;
  changedAt: string;
}
