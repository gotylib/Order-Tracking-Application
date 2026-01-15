import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { createOrder } from '../store/slices/ordersSlice';

const CreateOrder: React.FC = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { loading, error } = useAppSelector((state) => state.orders);

  const [formData, setFormData] = useState({
    orderNumber: '',
    description: '',
  });

  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSuccessMessage(null);

    try {
      const result = await dispatch(createOrder(formData));
      if (createOrder.fulfilled.match(result)) {
        setSuccessMessage('Заказ успешно создан!');
        setFormData({ orderNumber: '', description: '' });
        setTimeout(() => {
          navigate(`/orders/${result.payload.id}`);
        }, 1500);
      }
    } catch (err) {
      console.error('Error creating order:', err);
    }
  };

  return (
    <div className="create-order">
      <div className="card">
        <h2>Создать новый заказ</h2>
        {error && <div className="error">Ошибка: {error}</div>}
        {successMessage && <div className="success">{successMessage}</div>}
        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <label htmlFor="orderNumber">Номер заказа *</label>
            <input
              type="text"
              id="orderNumber"
              name="orderNumber"
              value={formData.orderNumber}
              onChange={handleChange}
              required
              placeholder="Например: ORD-001"
            />
          </div>
          <div className="input-group">
            <label htmlFor="description">Описание *</label>
            <textarea
              id="description"
              name="description"
              value={formData.description}
              onChange={handleChange}
              required
              placeholder="Введите описание заказа"
            />
          </div>
          <button type="submit" className="button" disabled={loading}>
            {loading ? 'Создание...' : 'Создать заказ'}
          </button>
        </form>
      </div>
    </div>
  );
};

export default CreateOrder;
