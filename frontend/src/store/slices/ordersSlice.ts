import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { OrderDto, CreateOrderDto, UpdateOrderStatusDto, OrderStatus } from '../../types/order';
import { ordersApi } from '../../api/ordersApi';

// Async thunks - используют API функции
export const fetchOrders = createAsyncThunk('orders/fetchAll', async (_, { rejectWithValue }) => {
  try {
    return await ordersApi.getAllOrders();
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : 'Ошибка при загрузке заказов';
    return rejectWithValue(errorMessage);
  }
});

export const fetchOrderById = createAsyncThunk('orders/fetchById', async (id: string, { rejectWithValue }) => {
  try {
    return await ordersApi.getOrderById(id);
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : 'Ошибка при загрузке заказа';
    return rejectWithValue(errorMessage);
  }
});

export const createOrder = createAsyncThunk('orders/create', async (order: CreateOrderDto, { rejectWithValue }) => {
  try {
    return await ordersApi.createOrder(order);
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : 'Ошибка при создании заказа';
    return rejectWithValue(errorMessage);
  }
});

export const updateOrderStatus = createAsyncThunk(
  'orders/updateStatus',
  async ({ id, status }: { id: string; status: UpdateOrderStatusDto }, { rejectWithValue }) => {
    try {
      return await ordersApi.updateOrderStatus(id, status);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Ошибка при обновлении статуса';
      return rejectWithValue(errorMessage);
    }
  }
);

interface OrdersState {
  orders: OrderDto[];
  currentOrder: OrderDto | null;
  loading: boolean;
  error: string | null;
}

const initialState: OrdersState = {
  orders: [],
  currentOrder: null,
  loading: false,
  error: null,
};

const ordersSlice = createSlice({
  name: 'orders',
  initialState,
  reducers: {
    updateOrderFromWebSocket: (state, action: PayloadAction<OrderDto>) => {
      const updatedOrder = action.payload;
      const index = state.orders.findIndex((o) => o.id === updatedOrder.id);
      if (index !== -1) {
        state.orders[index] = updatedOrder;
      } else {
        state.orders.push(updatedOrder);
      }
      if (state.currentOrder?.id === updatedOrder.id) {
        state.currentOrder = updatedOrder;
      }
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch all orders
      .addCase(fetchOrders.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchOrders.fulfilled, (state, action) => {
        state.loading = false;
        // Нормализуем статусы к числам, если пришли как строки
        state.orders = action.payload.map(order => ({
          ...order,
          status: typeof order.status === 'string' ? parseInt(order.status, 10) as OrderStatus : order.status
        }));
      })
      .addCase(fetchOrders.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Ошибка при загрузке заказов';
      })
      // Fetch order by id
      .addCase(fetchOrderById.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchOrderById.fulfilled, (state, action) => {
        state.loading = false;
        // Нормализуем статус к числу, если пришел как строка
        const order = { ...action.payload };
        if (typeof order.status === 'string') {
          order.status = parseInt(order.status, 10) as OrderStatus;
        }
        state.currentOrder = order;
        console.log('Order fetched:', order);
      })
      .addCase(fetchOrderById.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Ошибка при загрузке заказа';
      })
      // Create order
      .addCase(createOrder.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(createOrder.fulfilled, (state, action) => {
        state.loading = false;
        // Нормализуем статус к числу, если пришел как строка
        const order = { ...action.payload };
        if (typeof order.status === 'string') {
          order.status = parseInt(order.status, 10) as OrderStatus;
        }
        state.orders.unshift(order);
      })
      .addCase(createOrder.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Ошибка при создании заказа';
      })
      // Update order status
      .addCase(updateOrderStatus.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(updateOrderStatus.fulfilled, (state, action) => {
        state.loading = false;
        // Нормализуем статус к числу, если пришел как строка
        const order = { ...action.payload };
        if (typeof order.status === 'string') {
          order.status = parseInt(order.status, 10) as OrderStatus;
        }
        
        const index = state.orders.findIndex((o) => o.id === order.id);
        if (index !== -1) {
          state.orders[index] = order;
        }
        if (state.currentOrder?.id === order.id) {
          state.currentOrder = order;
        }
        console.log('Order status updated:', order);
      })
      .addCase(updateOrderStatus.rejected, (state, action) => {
        state.loading = false;
        state.error = action.error.message || 'Ошибка при обновлении статуса заказа';
      });
  },
});

export const { updateOrderFromWebSocket, clearError } = ordersSlice.actions;
export default ordersSlice.reducer;
