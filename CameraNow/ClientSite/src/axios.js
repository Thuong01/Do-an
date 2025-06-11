import axios from 'axios';

const instance = axios.create({
    baseURL: 'https://localhost:7085/api/v1/',
});

// Thêm một bộ đón chặn request
instance.interceptors.request.use(
    // Làm gì đó trước khi request dược gửi đi
    function (config) {
        const token = localStorage.getItem('access_token');
        const expiresAt = localStorage.getItem('expiresAt');

        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }

        return config;
    },
    function (error) {
        return Promise.reject(error);
    },
);

// Thêm một bộ đón chặn response
instance.interceptors.response.use(
    function (response) {
        const customReponse = {
            data: response.data,
            status: response.status,
        };
        return customReponse;
    },
    function (error) {
        if (error?.response && error?.response.status === 401) {
            // Nếu là lỗi 401, không làm gì cả
            return Promise.resolve(); // Trả về resolve để tiếp tục mà không log lỗi
        }

        // console.log(error?.response.status);

        if (error?.response?.data) {
            return Promise.reject(error.response);
        }

        return Promise.reject(error);
    },
);

export default instance;
