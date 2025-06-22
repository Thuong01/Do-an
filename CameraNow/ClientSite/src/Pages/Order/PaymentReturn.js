import { useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import CustomToast from '../../Untils/CustomToast';
import axios from '../../axios';
import styles from './PaymentReturn.module.scss';
import clsx from 'clsx';
import loading from '../../assets/gifs/loading_payment_return.gif';
import { useDispatch } from 'react-redux';
import { setLoading } from '../../Redux/Slices/LoadingSlice';

const PaymentReturn = () => {
    const dispatch = useDispatch();
    var location = useLocation();
    var navigate = useNavigate();

    const handleRequestPaymentReturn = async () => {
        dispatch(setLoading(false));
        const params = new URLSearchParams(location.search);

        const paymentData = {
            vnp_Amount: params.get('vnp_Amount'),
            vnp_BankCode: params.get('vnp_BankCode'),
            vnp_BankTranNo: params.get('vnp_BankTranNo'),
            vnp_CardType: params.get('vnp_CardType'),
            vnp_OrderInfo: params.get('vnp_OrderInfo'),
            vnp_PayDate: params.get('vnp_PayDate'),
            vnp_ResponseCode: params.get('vnp_ResponseCode'),
            vnp_TmnCode: params.get('vnp_TmnCode'),
            vnp_TransactionNo: params.get('vnp_TransactionNo'),
            vnp_TransactionStatus: params.get('vnp_TransactionStatus'),
            vnp_TxnRef: params.get('vnp_TxnRef'),
            vnp_SecureHash: params.get('vnp_SecureHash'),
        };

        console.log(paymentData);

        var res = await axios.get('/PaymentAPIs/vnpay-return', { params: paymentData });

        if (res?.data?.paymentStatus === '00') {
            CustomToast.success('Thanh toán thành công!');
            navigate('/orders');
        }
        if (res?.data?.paymentStatus === '24') {
            CustomToast.success('Đã hủy thanh toán!');
            navigate('/orders');
        }
        if (res?.data?.paymentStatus === '10') {
            CustomToast.success('Đã hủy thanh toán!');
            navigate('/orders');
        }
    };

    useEffect(() => {
        handleRequestPaymentReturn();
    }, [location, navigate]);

    return (
        <div className="container section">
            <div className={clsx(styles['processing-wrapper'])}>
                <h3>Đang xử lý</h3>
                <img src={loading} alt="Loading" />
            </div>
        </div>
    );
};

export default PaymentReturn;
