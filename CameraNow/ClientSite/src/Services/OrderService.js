import axios from '../axios';

const OrderService = (credential) => {
    return axios.post('OrderAPIs/orders', credential);
};

const GetOrderListServicce = ({ PageNumber = 1, PageSize = 10, Keyword = '' }) => {
    return axios.get(`OrderAPIs/orders/user-orders?PageNumber=${PageNumber}&PageSize=${PageSize}&keyword=${Keyword}`);
};

const GetOrderById = ({ orderId, userId }) => {
    return axios.get(`OrderAPIs/orders/order-user/${orderId}?userId=${userId}`);
};

const ChangeStatusOrder = ({ orderId }) => {
    return axios.delete(`OrderAPIs/orders/cancelled-order/${orderId}`);
};

const GetCoupon = ({ code }) => {
    return axios.get(`CouponAPIs/coupons/${code}`);
};

export { OrderService, GetOrderListServicce, GetOrderById, ChangeStatusOrder, GetCoupon };
