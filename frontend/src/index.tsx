import React from 'react';
import ReactDOM from 'react-dom/client';
import { Provider } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
import './index.css';
import App from './App';
import { store } from './store/store';
import ErrorBoundary from './utils/errorBoundary';

// Логирование для диагностики
console.log('React app starting...');
console.log('API URL:', process.env.REACT_APP_API_URL || '/api');
console.log('WebSocket URL:', process.env.REACT_APP_WS_URL || 'auto-detect');

const rootElement = document.getElementById('root');
if (!rootElement) {
  throw new Error('Root element not found');
}

const root = ReactDOM.createRoot(rootElement);

try {
  root.render(
    <React.StrictMode>
      <ErrorBoundary>
        <Provider store={store}>
          <BrowserRouter>
            <App />
          </BrowserRouter>
        </Provider>
      </ErrorBoundary>
    </React.StrictMode>
  );
  console.log('React app rendered successfully');
} catch (error) {
  console.error('Failed to render React app:', error);
  rootElement.innerHTML = `
    <div style="padding: 20px; text-align: center; font-family: Arial, sans-serif;">
      <h1 style="color: #d32f2f;">Ошибка загрузки приложения</h1>
      <p style="color: #666;">${error instanceof Error ? error.message : 'Неизвестная ошибка'}</p>
      <button onclick="window.location.reload()" style="padding: 10px 20px; font-size: 16px; background-color: #1976d2; color: white; border: none; border-radius: 4px; cursor: pointer;">
        Перезагрузить страницу
      </button>
    </div>
  `;
}
