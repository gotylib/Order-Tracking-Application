import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchOrderById, updateOrderStatus } from '../store/slices/ordersSlice';
import { OrderStatus } from '../types/order';
import './OrderDetails.css';

const OrderDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { currentOrder, loading, error } = useAppSelector((state) => state.orders);
  const { connected } = useAppSelector((state) => state.websocket);

  const [selectedStatus, setSelectedStatus] = useState<OrderStatus>(OrderStatus.Created);

  useEffect(() => {
    if (id) {
      dispatch(fetchOrderById(id));
    }
  }, [dispatch, id]);

  useEffect(() => {
    if (currentOrder) {
      // Нормализуем статус к числу
      const statusNum = typeof currentOrder.status === 'string' 
        ? parseInt(currentOrder.status, 10) 
        : Number(currentOrder.status);
      setSelectedStatus(statusNum as OrderStatus);
      console.log('Order status updated:', currentOrder.status, 'normalized:', statusNum);
    }
  }, [currentOrder]);

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

  const handleStatusChange = async (e: React.ChangeEvent<HTMLSelectElement>) => {
    const newStatus = Number(e.target.value) as OrderStatus;
    const currentStatusNum = typeof currentOrder?.status === 'string' 
      ? parseInt(currentOrder.status, 10) 
      : Number(currentOrder?.status);
    
    setSelectedStatus(newStatus);

    if (id && newStatus !== currentStatusNum) {
      try {
        console.log('Updating status:', { id, from: currentStatusNum, to: newStatus });
        const result = await dispatch(updateOrderStatus({ id, status: { status: newStatus } }));
        console.log('Status update result:', result);
        
        // Если обновление успешно, обновим локальное состояние
        if (updateOrderStatus.fulfilled.match(result)) {
          console.log('Status updated successfully');
        } else if (updateOrderStatus.rejected.match(result)) {
          console.error('Status update rejected:', result.error);
          setSelectedStatus(currentStatusNum as OrderStatus);
        }
      } catch (err) {
        console.error('Error updating order status:', err);
        setSelectedStatus(currentStatusNum as OrderStatus);
      }
    }
  };

  if (loading && !currentOrder) {
    return <div className="loading">Загрузка заказа...</div>;
  }

  if (error) {
    return <div className="error">Ошибка: {error}</div>;
  }

  if (!currentOrder) {
    return <div className="error">Заказ не найден</div>;
  }

  return (
    <div className="order-details">
      <div className="card">
        <div className="order-details-header">
          <button className="button button-secondary" onClick={() => navigate('/')}>
            ← Назад к списку
          </button>
          <div className="websocket-status">
            <span className={connected ? 'status-connected' : 'status-disconnected'}>
              {connected ? '● Подключено' : '○ Отключено'}
            </span>
          </div>
        </div>
        <h2>Детали заказа</h2>
        <div className="order-info">
          <div className="info-row">
            <span className="info-label">Номер заказа:</span>
            <span className="info-value">{currentOrder.orderNumber}</span>
          </div>
          <div className="info-row">
            <span className="info-label">Описание:</span>
            <span className="info-value">{currentOrder.description}</span>
          </div>
          <div className="info-row">
            <span className="info-label">Текущий статус:</span>
            <span className={`status-badge ${getStatusClass(currentOrder.status)}`}>
              {getStatusLabel(currentOrder.status)}
            </span>
          </div>
          <div className="info-row">
            <span className="info-label">Дата создания:</span>
            <span className="info-value">{formatDate(currentOrder.createdAt)}</span>
          </div>
          <div className="info-row">
            <span className="info-label">Последнее обновление:</span>
            <span className="info-value">{formatDate(currentOrder.updatedAt)}</span>
          </div>
        </div>
        <div className="status-update-section">
          <h3>Изменить статус</h3>
          <select
            value={selectedStatus.toString()}
            onChange={handleStatusChange}
            className="status-select"
            disabled={loading}
          >
            <option value={OrderStatus.Created.toString()}>{getStatusLabel(OrderStatus.Created)}</option>
            <option value={OrderStatus.Sent.toString()}>{getStatusLabel(OrderStatus.Sent)}</option>
            <option value={OrderStatus.Delivered.toString()}>{getStatusLabel(OrderStatus.Delivered)}</option>
            <option value={OrderStatus.Cancelled.toString()}>{getStatusLabel(OrderStatus.Cancelled)}</option>
          </select>
          {loading && <span className="loading-text">Обновление...</span>}
          {error && <span style={{ color: '#d32f2f', fontSize: '14px' }}>{error}</span>}
        </div>
      </div>
    </div>
  );
};

export default OrderDetails;
