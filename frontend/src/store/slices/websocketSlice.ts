import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import * as signalR from '@microsoft/signalr';
import { OrderStatusChangedEvent } from '../../types/order';
import type { AppDispatch } from '../store';

interface WebSocketState {
  connected: boolean;
  error: string | null;
}

const initialState: WebSocketState = {
  connected: false,
  error: null,
};

let connection: signalR.HubConnection | null = null;

const websocketSlice = createSlice({
  name: 'websocket',
  initialState,
  reducers: {
    setConnected: (state, action: PayloadAction<boolean>) => {
      state.connected = action.payload;
    },
    setError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
    },
  },
});

export const { setConnected, setError } = websocketSlice.actions;

export const connectWebSocket = () => async (dispatch: AppDispatch) => {
  // Определяем WebSocket URL в зависимости от окружения
  const getWebSocketUrl = () => {
    if (process.env.REACT_APP_WS_URL) {
      return process.env.REACT_APP_WS_URL;
    }
    // SignalR использует HTTP/HTTPS, не ws://
    const protocol = window.location.protocol === 'https:' ? 'https:' : 'http:';
    return `${protocol}//${window.location.host}/orderTrackingHub`;
  };
  
  const wsUrl = getWebSocketUrl();
  
  try {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(wsUrl)
      .withAutomaticReconnect()
      .build();

    connection.on('OrderStatusChanged', (event: OrderStatusChangedEvent) => {
      console.log('OrderStatusChanged event received:', event);
      if (event.orderId) {
        // Динамический импорт для избежания циклической зависимости
        import('./ordersSlice').then(({ fetchOrderById }) => {
          dispatch(fetchOrderById(event.orderId));
        }).catch((error) => {
          console.error('Error importing fetchOrderById:', error);
        });
      }
    });

    connection.onreconnecting(() => {
      console.log('SignalR reconnecting...');
      dispatch(setConnected(false));
    });

    connection.onreconnected(() => {
      console.log('SignalR reconnected');
      dispatch(setConnected(true));
      dispatch(setError(null));
    });

    connection.onclose(() => {
      console.log('SignalR connection closed');
      dispatch(setConnected(false));
    });

    await connection.start();
    console.log('SignalR connected');
    dispatch(setConnected(true));
    dispatch(setError(null));
  } catch (error) {
    console.error('Error creating SignalR connection:', error);
    dispatch(setError('Не удалось создать SignalR соединение'));
    dispatch(setConnected(false));
  }
};

export const disconnectWebSocket = () => async (dispatch: AppDispatch) => {
  if (connection) {
    await connection.stop();
    connection = null;
  }
  dispatch(setConnected(false));
};

export default websocketSlice.reducer;
