import { configureStore } from '@reduxjs/toolkit';
import ordersReducer from './slices/ordersSlice';
import websocketReducer from './slices/websocketSlice';

export const store = configureStore({
  reducer: {
    orders: ordersReducer,
    websocket: websocketReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
