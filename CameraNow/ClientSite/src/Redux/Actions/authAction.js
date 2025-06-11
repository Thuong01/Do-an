import { createAsyncThunk } from '@reduxjs/toolkit';
import axios from 'axios';
import { loginStart, loginSuccess, logout } from '../Slices/authSlice';
import { setLoading } from '../Slices/LoadingSlice';
import { toast } from 'react-toastify';
import { getUserInforService } from '../../Services/UserService';
import { removeUser, saveUser } from '../Slices/userSlide';
import CustomToast from '../../Untils/CustomToast';

const userLogin = (credentials, navigate) => async (dispatch) => {
    dispatch(loginStart());
    dispatch(setLoading(true));

    try {
        const config = {
            headers: {
                'Content-Type': 'application/json',
            },
        };

        const response = await axios.post('https://localhost:7085/api/LoginAPI/login', credentials, config);

        if (response?.data?.success) {
            localStorage.setItem('access_token', response.data.result.token);
            localStorage.setItem('expiresAt', response.data.result.expiresAt);
            CustomToast.success('Đăng nhập thành công');
            navigate('/');
            var user_data = getUserInforService();
            if (user_data.status === 200) {
                dispatch(saveUser(user_data?.data?.result));
            }
            dispatch(loginSuccess(response?.data?.result));
        }

        dispatch(setLoading(false));
    } catch (error) {
        console.log(error);

        if (error?.response?.data?.message == 'Invalid password.') {
            CustomToast.error('Sai mật khẩu');
        }

        dispatch(setLoading(false));
        localStorage.removeItem('access_token');
        localStorage.removeItem('expiresAt');
    }
};

const userLogOut = () => async (dispatch) => {
    try {
        localStorage.removeItem('access_token');
        dispatch(removeUser());
        dispatch(logout());
    } catch (error) {
        toast.error('Không thể đăng xuất, có lỗi xảy ra!');
        localStorage.removeItem('access_token');
    }
};

export { userLogin, userLogOut };
