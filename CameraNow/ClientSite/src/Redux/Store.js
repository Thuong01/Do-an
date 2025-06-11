import { configureStore } from '@reduxjs/toolkit';
import LoadingReducer from './Slices/LoadingSlice';
import authSlice from './Slices/authSlice';
import userSlide from './Slices/userSlide';

export const store = configureStore({ reducer: { loading: LoadingReducer, auth: authSlice, user: userSlide } });
