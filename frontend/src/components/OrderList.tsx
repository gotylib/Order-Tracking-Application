import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchOrders } from '../store/slices/ordersSlice';
import { OrderStatus } from '../types/order';
import './OrderList.css';

const OrderList: React.FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { orders, loading, error } = useAppSelector((state) => state.orders);

  useEffect(() => {
    dispatch(fetchOrders());
  }, [dispatch]);

  const getStatusLabel = (status: OrderStatus | number | string): string => {
    // Нормализуем статус к числу
    const statusNum = typeof status === 'string' ? parseInt(status, 10) : Number(status);
    
    switch (statusNum) {
      case OrderStatus.Created:
      case 0:
        return 'Создан';
      case OrderStatus.Sent:
      case 1:
        return 'Отправлен';
      case OrderStatus.Delivered:
      case 2:
        return 'Доставлен';
      case OrderStatus.Cancelled:
      case 3:
        return 'Отменен';
      default:
        console.warn('Unknown status value:', status, 'type:', typeof status);
        return 'Неизвестно';
    }
  };

  const getStatusClass = (status: OrderStatus | number | string): string => {
    // Нормализуем статус к числу
    const statusNum = typeof status === 'string' ? parseInt(status, 10) : Number(status);
    
    switch (statusNum) {
      case OrderStatus.Created:
      case 0:
        return 'status-created';
      case OrderStatus.Sent:
      case 1:
        return 'status-sent';
      case OrderStatus.Delivered:
      case 2:
        return 'status-delivered';
      case OrderStatus.Cancelled:
      case 3:
        return 'status-cancelled';
      default:
        return '';
    }
  };

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleString('ru-RU');
  };

  if (loading && orders.length === 0) {
    return <div className="loading">Загрузка заказов...</div>;
  }

  if (error) {
    return <div className="error">Ошибка: {error}</div>;
  }

  return (
    <div className="order-list">
      <div className="card">
        <h2>Список заказов</h2>
        {orders.length === 0 ? (
          <p>Заказы не найдены. Создайте первый заказ!</p>
        ) : (
          <div className="orders-container">
            {orders.map((order) => (
              <div
                key={order.id}
                className="order-item"
                onClick={() => navigate(`/orders/${order.id}`)}
              >
                <div className="order-item-header">
                  <span className="order-item-number">{order.orderNumber}</span>
                  <span className={`status-badge ${getStatusClass(order.status)}`}>
                    {getStatusLabel(order.status)}
                  </span>
                </div>
                <div className="order-item-description">{order.description}</div>
                <div className="order-item-footer">
                  <span>Создан: {formatDate(order.createdAt)}</span>
                  <span>Обновлен: {formatDate(order.updatedAt)}</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default OrderList;
