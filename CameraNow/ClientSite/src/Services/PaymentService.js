import axios from '../axios';

export const getMomoLinkPaymentService = ({ amount, orderId, orderInfo }) => {
    return axios.post('/PaymentAPIs/momo-payment', {
        amount,
        orderId,
        orderInfo,
    });
};

export const getVnpayLinkPaymentService = ({ amount, orderId, orderInfo }) => {
    return axios.post('PaymentAPIs/vnpay-payment', {
        amount,
        orderId,
        orderInfo,
    });
};
