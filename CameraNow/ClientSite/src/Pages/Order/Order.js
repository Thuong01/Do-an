import clsx from 'clsx';
import styles from './Order.module.scss';
import { useLocation, useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { FormatPrice, generateOrderCode } from '../../Untils/CommonUntil';
import { useDispatch, useSelector } from 'react-redux';
import CustomToast from '../../Untils/CustomToast';
import { GetCoupon, OrderService } from '../../Services/OrderService';
import { setLoading } from '../../Redux/Slices/LoadingSlice';
import Swal from 'sweetalert2';
import { getVnpayLinkPaymentService } from '../../Services/PaymentService';
import * as Yup from 'yup';
import axios from '../../axios';
import { Form, Modal } from 'react-bootstrap';
import { getUserInforService } from '../../Services/UserService';
import VoucherItem from './VoucherItem';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronRight } from '@fortawesome/free-solid-svg-icons';

export const orderSchema = Yup.object().shape({
    fullName: Yup.string().required('Họ tên là bắt buộc'),
    address: Yup.string().required('Địa chỉ là bắt buộc'),
    phone: Yup.string()
        .matches(/^[0-9]{10,11}$/, 'Số điện thoại không hợp lệ')
        .required('Số điện thoại là bắt buộc'),
    email: Yup.string().email('Email không hợp lệ').required('Email là bắt buộc'),
});

const Order = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    var location = useLocation();
    var user = useSelector((state) => state.user);
    var products = location.state?.products;
    var detail_yn = location.state?.detail_yn;
    var from_cart_yn = location.state?.from_cart_yn;
    var cartID = location.state?.cartID;
    var [order, setOrder] = useState({
        user_Id: '',
        orderNo: '',
        email: '',
        phone: '',
        address: '',
        message: '',
        fullName: '',
        payment_Method: 'PAYMENT_COD',
        status: 1,
        coupon_Id: 0,
        order_Date: new Date().toISOString(),
        fromCart_YN: from_cart_yn,
        cartID: cartID,
        orderDetails: [
            {
                product_ID: '',
                quantity: 0,
                size: '',
                color: '',
            },
        ],
    });
    var [total_amount, setTotal_Amount] = useState(0);
    const [phoneError, setPhoneError] = useState('');
    const [emailError, setEmailError] = useState('');
    const [coupon, setCoupon] = useState('');
    const [couponData, setCouponData] = useState(null);
    const [vouchers, setVouchers] = useState([]);
    const [showVoucher, setShowVoucher] = useState(false);

    const getUser = async () => {
        try {
            var res = await getUserInforService();
            if (!res) {
                navigate('/login');
                return;
            }

            user = res.data.result;
        } catch (err) {
            console.log(err);
            navigate('/login');
        }
    };

    const handlePhoneChange = (e) => {
        const value = e.target.value.trim();
        const phoneRegex = /^(0)(3[2-9]|5[6|8|9]|7[0|6-9]|8[1-5]|9[0-9])[0-9]{7}$/;

        if (value === '') {
            setPhoneError('Vui lòng nhập số điện thoại.');
            return;
        }

        if (!/^\d+$/.test(value)) {
            setPhoneError('Số điện thoại chỉ được chứa chữ số.');
            return;
        }

        if (!phoneRegex.test(value)) {
            setPhoneError('Số điện thoại không hợp lệ. Vui lòng nhập số đúng định dạng Việt Nam.');
            return;
        }

        // Nếu hợp lệ:
        setPhoneError('');
        setOrder({ ...order, phone: value });
    };

    const handleEmailChange = (e) => {
        const value = e.target.value.trim();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (value === '') {
            setEmailError('Vui lòng nhập email.');
            return;
        }

        if (!emailRegex.test(value)) {
            setEmailError('Email không hợp lệ. Vui lòng nhập đúng định dạng.');
            return;
        }

        // Nếu hợp lệ
        setEmailError('');
        setOrder({ ...order, email: value });
    };

    // fetch vouchers list
    useEffect(() => {
        dispatch(setLoading(true));
        const fetchVoucher = async () => {
            try {
                const response = await axios.get('CouponAPIs/coupons', {
                    params: {
                        Status: -1,
                        Sorting: 'name',
                        PageNumber: 1,
                        PageSize: 60,
                        Filter: '',
                    },
                });

                console.log(response.data?.result?.data);
                dispatch(setLoading(false));

                setVouchers(response.data?.result?.data);
            } catch (error) {
                console.log(error);
            }
        };
        fetchVoucher();
    }, []);

    useEffect(() => {
        setOrder((prevOrder) => {
            const newOrderDetails = products.map((product) => ({
                product_ID: product.id,
                quantity: product.quantity_order || 1, // Nếu không có quantity, mặc định là 1
                size: product.product_size,
                color: product.product_color,
            }));
            return {
                ...prevOrder,
                orderNo: prevOrder.orderNo || generateOrderCode(), // Nếu orderNo rỗng, tạo mới
                user_Id: user.id,
                email: user.email,
                fullName: user.fullName,
                address: user.address,
                phone: user.phoneNumber,
                fromCart_YN: from_cart_yn,
                cartID: cartID,
                orderDetails: newOrderDetails,
            };
        });
    }, []);

    useEffect(() => {
        getUser();
        setOrder((prevOrder) => {
            const newOrderDetails = products.map((product) => ({
                product_ID: product.id,
                quantity: product.quantity_order || 1, // Nếu không có quantity, mặc định là 1
                size: product.product_size,
                color: product.product_color,
            }));
            return {
                ...prevOrder,
                orderNo: prevOrder.orderNo || generateOrderCode(), // Nếu orderNo rỗng, tạo mới
                user_Id: user.id,
                email: user.email,
                fullName: user.fullName,
                address: user.address,
                phone: user.phoneNumber,
                orderDetails: newOrderDetails,
            };
        });
    }, [user]);

    useEffect(() => {
        const total = products.reduce((sum, item) => {
            return sum + item.quantity_order * (item.price || 0); // Nếu không có price, mặc định là 0
        }, 0);
        setTotal_Amount(total);
    }, [products]);

    const HandleOrder = async (e) => {
        try {
            e.preventDefault();

            const requiredFields = {
                fullName: 'Họ tên',
                address: 'Địa chỉ nhận hàng',
                phone: 'Số điện thoại',
                email: 'Email',
                payment_Method: 'Phương thức thanh toán',
            };

            const missingFields = Object.entries(requiredFields)
                .filter(([key]) => !order[key] || order[key].trim() === '')
                .map(([_, label]) => label);

            if (missingFields.length > 0) {
                CustomToast.error(`Vui lòng điền đầy đủ các trường:\n- ${missingFields.join('\n- ')}`);
                return;
            }
            if (order.payment_Method !== '') {
                dispatch(setLoading(true));
                var res = await OrderService(order);

                if (res.status == 200) {
                    dispatch(setLoading(false));

                    if (res.data?.payment_Method == 'PAYMENT_VNPAY') {
                        var resVnpay = await getVnpayLinkPaymentService({
                            amount: res.data?.total_Amount.toString(),
                            orderId: res.data?.id,
                            orderInfo: `Thanh toán ${res.data?.total_Amount} cho đơn hàng ${res.data?.orderNo} có ID: ${res.data?.id}`,
                        });

                        if (resVnpay.status === 200) {
                            window.location.href = resVnpay.data?.payUrl;
                        }
                    } else {
                        // CustomToast.success('Đặt hàng thành công, vui lòng kiểm tra email của bạn!');
                        Swal.fire({
                            title: 'Đặt hàng thành công!',
                            icon: 'success',
                            draggable: true,
                        }).then((result) => {
                            navigate('/orders');
                        });
                    }
                } else {
                    CustomToast.error('Đặt hàng thất bại!');
                    dispatch(setLoading(false));
                    return;
                }
            } else {
                CustomToast.error('Chọn phương thức thanh toán!');
                dispatch(setLoading(false));
            }
        } catch {
            CustomToast.error('Không thể đặt hàng');
            dispatch(setLoading(false));
        }
    };

    const handleGetCoupon = async (couponCode) => {
        try {
            var res = await GetCoupon({ code: couponCode });

            console.log(res);

            const endDate = new Date(res?.data?.endDate);
            const now = new Date();

            if (endDate < now) {
                CustomToast.info('Mã giảm giá này đã hết hạn.');
                return false;
            }

            var total = products.reduce((sum, item) => sum + item.quantity_order * item.price, 0);

            if (res?.data?.minOrderAmount < total) {
                setCoupon('');
                setCouponData(res?.data);
                setOrder((prev) => ({ ...prev, coupon_Id: res?.data?.id }));

                if (res?.data.type == 0) {
                    var thanhtien = total - (total * res?.data?.value) / 100;

                    setTotal_Amount(thanhtien);
                } else if (res?.data.type == 1) {
                    var thanhtien = total - res?.data?.value;
                    setTotal_Amount(thanhtien);
                }
            } else {
                CustomToast.info(
                    `Mã giảm giá yêu cầu đơn hàng tối thiểu ${FormatPrice(res?.data?.minOrderAmount, 'VNĐ')}. `,
                );
            }
        } catch (err) {
            if (err?.status == '404') {
                CustomToast.warning('Không tìm thấy mã giảm giá');
            } else CustomToast.warning('Lấy coupon thất bại');
        }
    };

    const handleShowVoucher = () => setShowVoucher(true);
    const handleCloseVoucher = () => setShowVoucher(false);

    const addVoucherForOrder = (voucherId) => {
        console.log(voucherId);

        setCoupon(voucherId);
        handleGetCoupon(voucherId);
        handleCloseVoucher();
    };

    return (
        <main className="page-wrapper">
            <div className="mb-4 pb-4" />
            <section className="shop-checkout container">
                <h2 className="page-title">THANH TOÁN VÀ VẬN CHUYỂN</h2>

                <Form onSubmit={(e) => HandleOrder(e)}>
                    <div className={clsx(styles['checkout-form'])}>
                        <div className={clsx(styles['billing-info__wrapper'])}>
                            <h4>CHI TIẾT ĐƠN HÀNG No. {order.orderNo}</h4>
                            <div className="row">
                                <div className="col-md-12">
                                    <div className="form-floating my-3">
                                        <input
                                            className="form-control"
                                            id="checkout_last_name"
                                            placeholder="Last Name"
                                            type="text"
                                            onChange={(e) => setOrder({ ...order, fullName: e.target.value })}
                                            value={order?.fullName}
                                        />
                                        <label htmlFor="checkout_last_name">
                                            Họ tên <span className="text-danger">*</span>
                                        </label>
                                    </div>
                                </div>
                                <div className="col-md-12">
                                    <div className="form-floating mt-3 mb-3">
                                        <input
                                            className="form-control"
                                            id="checkout_street_address"
                                            placeholder="Street Address *"
                                            type="text"
                                            onChange={(e) => setOrder({ ...order, address: e.target.value })}
                                            value={order?.address}
                                        />
                                        <label htmlFor="checkout_company_name">
                                            Địa chỉ nhận hàng <span className="text-danger">*</span>
                                        </label>
                                    </div>
                                </div>
                                <div className="col-md-12">
                                    <div className="form-floating my-3">
                                        <input
                                            className={`form-control ${phoneError ? 'is-invalid' : ''}`}
                                            id="checkout_phone"
                                            placeholder="Phone *"
                                            type="text"
                                            onChange={handlePhoneChange}
                                            value={order?.phoneNumber}
                                        />
                                        <label htmlFor="checkout_phone">
                                            Điện thoại <span className="text-danger">*</span>
                                        </label>
                                        {phoneError && <div className="invalid-feedback">{phoneError}</div>}
                                    </div>
                                </div>
                                <div className="col-md-12">
                                    <div className="form-floating my-3">
                                        <input
                                            className="form-control"
                                            id="checkout_email"
                                            placeholder="Your Mail *"
                                            type="email"
                                            onChange={handleEmailChange}
                                            value={order?.email}
                                        />
                                        <label htmlFor="checkout_email">
                                            Email <span className="text-danger">*</span>
                                        </label>
                                        {emailError && <div className="text-danger mt-1">{emailError}</div>}
                                    </div>
                                </div>
                            </div>
                            <div className="col-md-12">
                                <div className="mt-3">
                                    <textarea
                                        className="form-control form-control_gray"
                                        placeholder="Thêm ghi chú cho đơn hàng"
                                        cols={30}
                                        rows={8}
                                        defaultValue={''}
                                        onChange={(e) => setOrder({ ...order, message: e.target.value })}
                                        value={order.message}
                                    />
                                </div>
                            </div>
                        </div>
                        <div className={clsx(styles['checkout__totals-wrapper'])}>
                            <div className={clsx(styles['sticky-content'])}>
                                <div className={clsx(styles['checkout__totals'])}>
                                    <h3>Đơn hàng của bạn</h3>
                                    <table className={clsx(styles['checkout-cart-items'])}>
                                        <thead>
                                            <tr>
                                                <th>Sản phẩm</th>
                                                <th>Giá</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {products.map((product) => {
                                                return (
                                                    <tr>
                                                        <td>
                                                            <div className={clsx('d-flex')}>
                                                                <img width={60} height={60} src={product?.image} />
                                                                <div className="ms-3">
                                                                    <div className={clsx(styles['text-truncate'])}>
                                                                        {product.name}
                                                                    </div>
                                                                    <div
                                                                        style={{ fontSize: '13px', fontWeight: 'bold' }}
                                                                        className="fz-2"
                                                                    ></div>
                                                                    <div>x {product.quantity_order}</div>
                                                                </div>
                                                            </div>
                                                        </td>
                                                        <td>
                                                            {FormatPrice(product.price * product.quantity_order, 'VNĐ')}
                                                        </td>
                                                    </tr>
                                                );
                                            })}
                                        </tbody>
                                    </table>
                                    <table className={clsx(styles['checkout-totals'])}>
                                        <tbody>
                                            <tr>
                                                <th>Tổng tiền</th>
                                                <td>
                                                    {FormatPrice(
                                                        products.reduce(
                                                            (sum, item) => sum + item.quantity_order * item.price,
                                                            0,
                                                        ),
                                                        'VNĐ',
                                                    )}
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>Phí vận chuyển</th>
                                                <td>Free shipping</td>
                                            </tr>
                                            {couponData && (
                                                <tr>
                                                    <th>Giảm giá</th>
                                                    <td>{couponData?.type == 0 ? couponData?.value + '%' : 0}</td>
                                                </tr>
                                            )}
                                            <tr>
                                                <th>Thành tiền</th>
                                                <td>{FormatPrice(total_amount, 'VNĐ')}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>

                                <div>
                                    {!couponData ? (
                                        <>
                                            <div className={clsx(styles['checkout__totals'])}>
                                                <h3>Mã giảm giá (Nếu có)</h3>

                                                <div className="form-group row mb-2">
                                                    <div className="col-md-9 col-sm-9 col-xs-9">
                                                        <input
                                                            type="text"
                                                            className="form-control field"
                                                            id="coupon_code"
                                                            name="coupon_code"
                                                            rows={5}
                                                            value={coupon}
                                                            onChange={(e) => setCoupon(e.target.value)}
                                                        />
                                                    </div>
                                                    <div className="col-md-3 col-sm-3 col-xs-3">
                                                        <input
                                                            type="button"
                                                            id="coupon_code_apply"
                                                            name="coupon_code_apply"
                                                            className="btn btn-default "
                                                            onClick={() => handleGetCoupon(coupon)}
                                                            style={{ background: '#F1AF0A' }}
                                                            defaultValue="Áp dụng"
                                                        />
                                                    </div>
                                                </div>

                                                <span
                                                    style={{ cursor: 'pointer' }}
                                                    className={clsx(styles['choose-ticket'])}
                                                    onClick={handleShowVoucher}
                                                >
                                                    Xem thêm <FontAwesomeIcon icon={faChevronRight} />
                                                </span>
                                            </div>
                                        </>
                                    ) : (
                                        <>
                                            <div className={clsx(styles['voucher-container'])}>
                                                <div className={clsx(styles['voucher-header'])}>
                                                    <h3>Mã giảm giá đã áp dụng</h3>
                                                    <button
                                                        className={clsx(styles['remove-voucher'])}
                                                        onClick={() => setCouponData(null)}
                                                    >
                                                        Xóa
                                                    </button>
                                                </div>
                                                <div className={clsx(styles['voucher-body'])}>
                                                    <div className={clsx(styles['voucher-code'])}>
                                                        {couponData.code}
                                                    </div>
                                                    <div className={clsx(styles['voucher-discount'])}>
                                                        Giảm {couponData.value}%
                                                    </div>
                                                </div>
                                            </div>
                                        </>
                                    )}
                                </div>

                                <div className={clsx(styles['checkout__payment-methods'])}>
                                    <p>Phương thức thanh toán</p>
                                    <div className="form-check">
                                        <input
                                            className="form-check-input form-check-input_fill"
                                            id="checkout_payment_method_1"
                                            type="radio"
                                            value={'PAYMENT_COD'}
                                            checked={order.payment_Method === 'PAYMENT_COD'}
                                            onChange={(e) => setOrder({ ...order, payment_Method: e.target.value })}
                                            name="checkout_payment_method"
                                        />
                                        <label className="form-check-label" htmlFor="checkout_payment_method_1">
                                            Thanh toán khi nhận hàng
                                        </label>
                                    </div>
                                    <div className="form-check mt-2">
                                        <input
                                            className="form-check-input form-check-input_fill"
                                            id="checkout_payment_method_2"
                                            type="radio"
                                            checked={order.payment_Method === 'PAYMENT_VNPAY'}
                                            onChange={(e) => setOrder({ ...order, payment_Method: e.target.value })}
                                            value={'PAYMENT_VNPAY'}
                                            name="checkout_payment_method"
                                        />
                                        <label className="form-check-label" htmlFor="checkout_payment_method_2">
                                            Thanh toán trực tuyến
                                        </label>
                                    </div>
                                </div>
                                <button type="submit" className={clsx(styles['btn-checkout'], 'btn btn-primary ')}>
                                    Đặt hàng
                                </button>
                            </div>
                        </div>
                    </div>
                </Form>
            </section>

            <div className={clsx(styles['voucher-wrapper'])}>
                <Modal show={showVoucher} onHide={handleCloseVoucher}>
                    <Modal.Header closeButton>
                        <Modal.Title>Chọn mã khuyến mãi</Modal.Title>
                    </Modal.Header>

                    <Modal.Body>
                        {vouchers?.length > 0 ? (
                            vouchers?.map((voucher) => {
                                return (
                                    <VoucherItem
                                        key={`voucher-${voucher?.id}`}
                                        voucherId={voucher?.id}
                                        name={
                                            voucher?.type == 0
                                                ? `Giảm giá ${voucher?.value} %`
                                                : `Giảm giá ${FormatPrice(voucher?.value, 'VNĐ')}`
                                        }
                                        desc={voucher?.code}
                                        code={voucher?.code}
                                        status={voucher?.status}
                                        endDate={voucher?.endDate}
                                        startDate={voucher?.startDate}
                                        handleSelectVoucher={addVoucherForOrder}
                                    />
                                );
                            })
                        ) : (
                            <div className="text-center">Bạn không có voucher nào</div>
                        )}
                    </Modal.Body>
                </Modal>
            </div>
        </main>
    );
};

export default Order;
