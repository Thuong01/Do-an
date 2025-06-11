import { useEffect, useState } from 'react';
import { Container } from 'react-bootstrap';
import { useLocation, useNavigate, useParams } from 'react-router-dom';
import { ChangeStatusOrder, GetOrderById } from '../../Services/OrderService';
import { FormatPrice } from '../../Untils/CommonUntil';
import './OrderDetail.scss';
import BreadCrumb from '../../Components/BreadCrumb';
import CustomToast from '../../Untils/CustomToast';
import { GetProductById } from '../../Services/ProductService';
import FeedbackModal from './FeedbackModal';
import { CreateProductFeedback } from '../../Services/FeedbackService';
import styled from 'styled-components';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheckCircle, faCube, faTruck, faXmarkCircle } from '@fortawesome/free-solid-svg-icons';

const ActionButton = styled.button`
    padding: 0.6rem 1.2rem;
    border-radius: 6px;
    font-weight: 500;
    margin-left: 0.8rem;
    cursor: pointer;
    transition: all 0.2s;
    border: 1px solid transparent;

    &:hover {
        transform: translateY(-1px);
    }

    &.cancel {
        background: #fee2e2;
        color: #b91c1c;
        border-color: #fca5a5;

        &:hover {
            background: #fecaca;
        }
    }

    &.rebuy {
        background: #e0f2fe;
        color: #0369a1;
        border-color: #7dd3fc;

        &:hover {
            background: #bae6fd;
        }
    }

    &.review {
        background: #fef3c7;
        color: #92400e;
        border-color: #fcd34d;

        &:hover {
            background: #fde68a;
        }
    }
`;

const statusIcons = {
    0: <FontAwesomeIcon icon={faXmarkCircle} style={{ marginRight: '0.5rem' }} />,
    1: <FontAwesomeIcon icon={faCube} style={{ marginRight: '0.5rem' }} />,
    2: <FontAwesomeIcon icon={faCube} style={{ marginRight: '0.5rem' }} />,
    3: <FontAwesomeIcon icon={faCube} style={{ marginRight: '0.5rem' }} />,
    4: <FontAwesomeIcon icon={faTruck} style={{ marginRight: '0.5rem' }} />,
    5: <FontAwesomeIcon icon={faCheckCircle} style={{ marginRight: '0.5rem' }} />,
};

const OrderDetail = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const [order, setOrder] = useState({});
    const { orderId, userId } = location.state || {};
    const [showFeedbackModal, setShowFeedbackModal] = useState(false);

    const statusMessage = {
        0: 'Đã hủy',
        1: 'Chờ xác nhận',
        2: 'Đã thanh toán, chờ giao hàng',
        3: 'Đơn hàng của bạn đã được xác nhận',
        4: 'Đang giao hàng',
        5: 'Đã giao',
    };

    const fetchOrderById = async () => {
        var res = await GetOrderById({ orderId: orderId, userId: userId });
        if (res?.status == 200) {
            setOrder(res?.data);
        }
    };

    const handleChangeStatusOrder = async () => {
        var res = await ChangeStatusOrder({ orderId: order.id });

        CustomToast.success('Hủy đơn hàng thành công.');

        fetchOrderById();
    };

    const handleRebuy = async () => {
        const prods = await Promise.all(
            order?.orderDetails.map(async (e) => {
                const prod = await GetProductById(e.product_ID);
                prod.data.quantity_order = e.quantity;
                return prod?.data;
            }),
        );
        navigate('/order', {
            state: {
                products: prods,
                detail_yn: true,
            },
        });
    };

    const handleSubmitFeedback = async (feedbackData) => {
        order?.orderDetails?.forEach(async (item) => {
            feedbackData.product_ID = item.product_ID;
            feedbackData.order_ID = item.order_ID;

            var res = await CreateProductFeedback(feedbackData);

            console.log(res);

            // await SendFeedback(feedbackData); // gọi API thực tế nếu có
            CustomToast.success('Gửi đánh giá thành công!');
            setShowFeedbackModal(false);
        });
    };

    useEffect(() => {
        fetchOrderById();
    }, [orderId, userId]);

    return (
        <div className="section">
            <Container>
                <BreadCrumb brecrumbs={[]} action_item={'chi tiết đơn hàng '} />

                <div className="my-4 text-end">
                    <div>
                        MÃ ĐƠN HÀNG. {order?.orderNo} |{' '}
                        <span className="custom-color">{statusMessage[order?.status]}</span>
                    </div>
                </div>

                <div>
                    <div className="pack_border"></div>
                </div>

                <div className="my-3">
                    <div>
                        <p>Địa chỉ nhận hàng</p>
                        <div className="text-secondary">
                            <p>Người đặt hàng: {order.fullName}</p>
                            <p>Email: {order.email}</p>
                            <p>Số điện thoại: {order.phone}</p>
                            <p>Địa chỉ giao hàng: {order.address}</p>
                        </div>
                    </div>
                </div>

                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <div></div>
                    <div>
                        {order?.status == 0 ? (
                            <ActionButton className="rebuy" onClick={() => handleRebuy()}>
                                Mua lại
                            </ActionButton>
                        ) : (
                            <></>
                        )}

                        {order?.status !== 5 && order?.status !== 4 && order?.status !== 0 ? (
                            <ActionButton className="cancel" onClick={() => handleChangeStatusOrder()}>
                                Hủy đơn
                            </ActionButton>
                        ) : (
                            <></>
                        )}

                        {order?.status == 5 ? (
                            <ActionButton className="rebuy" onClick={() => handleRebuy(order)}>
                                Mua lại
                            </ActionButton>
                        ) : (
                            <></>
                        )}

                        {order?.status == 5 && !order?.isCommented ? (
                            <ActionButton
                                className="review"
                                onClick={() => {
                                    setShowFeedbackModal(true);
                                }}
                            >
                                Đánh giá đơn hàng
                            </ActionButton>
                        ) : (
                            <></>
                        )}
                    </div>
                </div>

                <div className="mt-5 order-product-wrap">
                    {order?.orderDetails?.map((item, i) => (
                        <div className="mt-3 d-flex justify-content-between align-items-center">
                            <div className="d-flex justify-content-start align-items-center ">
                                <div>
                                    <img height="100" src={item.product_Image} alt={item.product_Name} />
                                </div>
                                <div className="ms-3 ">
                                    <p className="fw-semibold">{item.product_Name}</p>
                                    <p className="">Số lượng: x{item.quantity}</p>
                                </div>
                            </div>
                            <div>
                                <p className="custom-color">{FormatPrice(item.product_Price, 'VNĐ')}</p>
                            </div>
                        </div>
                    ))}
                </div>

                <div>
                    <div>
                        <div className="display-info-wrap">
                            <div className="display-info">
                                <div className="display-info-title">
                                    <span>Phương thức thanh toán:</span>
                                </div>
                                <div className="display-info-value">
                                    <span>
                                        {order?.payment_Method === 'PAYMENT_VNPAY'
                                            ? 'Thanh toán VNPay'
                                            : 'Thanh toán COD'}
                                    </span>
                                </div>
                            </div>
                            <div className="display-info">
                                <div className="display-info-title">
                                    <span>Phí vận chuyển:</span>
                                </div>
                                <div className="display-info-value">
                                    <span>Miễn phí</span>
                                </div>
                            </div>

                            <div className="display-info">
                                <div className="display-info-title">
                                    <span>Thành tiền:</span>
                                </div>
                                <div className="display-info-value">
                                    <span style={{ fontSize: '20px' }} className="custom-color">
                                        {FormatPrice(order?.total_Amount, 'VNĐ')}
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </Container>

            <FeedbackModal
                show={showFeedbackModal}
                onClose={() => setShowFeedbackModal(false)}
                onSubmit={handleSubmitFeedback}
                orderId={order.id}
            />
        </div>
    );
};

export default OrderDetail;
