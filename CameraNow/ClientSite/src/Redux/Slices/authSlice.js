import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';
import { userLogin } from '../Actions/authAction';

const AT = localStorage.getItem('access_token');
const expiresAt = localStorage.getItem('expiresAt');

// Kiểm tra expiresAt có hợp lệ và có nhỏ hơn thời điểm hiện tại không
if (expiresAt && new Date().toISOString() > expiresAt) {
    // Xóa access_token nếu expired
    localStorage.removeItem('access_token');
    localStorage.removeItem('expiresAt');
}

const initialState = {
    accessToken: AT && expiresAt && new Date().toISOString() <= expiresAt ? AT : null,
    expiresAt: expiresAt && new Date().toISOString() <= expiresAt ? expiresAt : null,
    login_Error: null,
    login_Successed: AT && expiresAt && new Date().toISOString() <= expiresAt ? true : false,
};

const authSlice = createSlice({
    name: 'auth/login',
    initialState,
    reducers: {
        loginStart: (state) => {
            state.login_Error = null;
            state.expiresAt = null;
            state.login_Successed = false;
        },
        loginSuccess: (state, action) => {
            state.login_Successed = true;
            state.expiresAt = action.payload.expiresAt;
            state.accessToken = action.payload.token;
        },
        loginFailure: (state, action) => {
            state.login_Error = action.payload;
        },
        logout: (state) => {
            state.accessToken = null;
            state.login_Error = null;
            state.expiresAt = expiresAt;
            state.login_Successed = false;
        },
    },
});

export const { loginStart, loginSuccess, loginFailure, logout } = authSlice.actions;
export default authSlice.reducer;
