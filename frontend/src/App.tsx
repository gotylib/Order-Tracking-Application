import React, { useEffect } from 'react';
import { Routes, Route, Link, useLocation } from 'react-router-dom';
import { useAppDispatch } from './store/hooks';
import { connectWebSocket, disconnectWebSocket } from './store/slices/websocketSlice';
import OrderList from './components/OrderList';
import OrderDetails from './components/OrderDetails';
import CreateOrder from './components/CreateOrder';
import './App.css';

const App: React.FC = () => {
  const dispatch = useAppDispatch();
  const location = useLocation();

  useEffect(() => {
    console.log('App component mounted');
    try {
      dispatch(connectWebSocket());
    } catch (error) {
      console.error('Error connecting WebSocket:', error);
    }
    return () => {
      try {
        dispatch(disconnectWebSocket());
      } catch (error) {
        console.error('Error disconnecting WebSocket:', error);
      }
    };
  }, [dispatch]);

  return (
    <div className="App">
      <header className="App-header">
        <nav className="App-nav">
          <Link to="/" className={location.pathname === '/' ? 'active' : ''}>
            Список заказов
          </Link>
          <Link to="/create" className={location.pathname === '/create' ? 'active' : ''}>
            Создать заказ
          </Link>
        </nav>
        <h1>Система отслеживания заказов</h1>
      </header>
      <main className="App-main">
        <Routes>
          <Route path="/" element={<OrderList />} />
          <Route path="/create" element={<CreateOrder />} />
          <Route path="/orders/:id" element={<OrderDetails />} />
        </Routes>
      </main>
    </div>
  );
};

export default App;
