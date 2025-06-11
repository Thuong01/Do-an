import { useEffect, useState } from 'react';
import { Col, Container, Row } from 'react-bootstrap';
import { useDispatch, useSelector } from 'react-redux';
import { ChangeStatusOrder, GetOrderListServicce } from '../../Services/OrderService';
import clsx from 'clsx';
import styles from './OrderList.module.scss';
import { NavLink, useNavigate } from 'react-router-dom';
import { FormatLongDate, FormatPrice } from '../../Untils/CommonUntil';
import CustomToast from '../../Untils/CustomToast';
import { GetProductById } from '../../Services/ProductService';
import FeedbackModal from './FeedbackModal';
import { CreateProductFeedback } from '../../Services/FeedbackService';
import styled from 'styled-components';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import {
    faCheckCircle,
    faCube,
    faSearch,
    faShoppingCart,
    faStar,
    faTruck,
    faXmarkCircle,
} from '@fortawesome/free-solid-svg-icons';
import { setLoading } from '../../Redux/Slices/LoadingSlice';
import Swal from 'sweetalert2';

//#region Styled

const OrderListContainer = styled.div`
    max-width: 1200px;
    margin: 0 auto;
    padding: 2rem 1rem;
`;

const SearchContainer = styled.div`
    display: flex;
    align-items: center;
    background: #fff;
    border-radius: 8px;
    padding: 0.8rem 1rem;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
    margin-bottom: 2rem;
`;

const SearchInput = styled.input`
    flex: 1;
    border: none;
    outline: none;
    padding: 0.5rem 1rem;
    font-size: 1rem;
    color: #333;

    &::placeholder {
        color: #999;
    }
`;

const OrderCard = styled.div`
    background: #fff;
    border-radius: 12px;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
    margin-bottom: 2rem;
    overflow: hidden;
`;

const OrderHeader = styled.div`
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1.2rem 1.5rem;
    background: #f9fafb;
    border-bottom: 1px solid #eee;
`;

const OrderStatus = styled.span`
    padding: 0.4rem 0.8rem;
    border-radius: 20px;
    font-size: 0.85rem;
    font-weight: 500;
    background: ${(props) => {
        switch (props.status) {
            case 0:
                return '#fee2e2'; // Cancelled
            case 1:
            case 3:
                return '#fef3c7'; // Pending/Confirmed
            case 2:
            case 4:
                return '#dbeafe'; // Paid/Shipping
            case 5:
                return '#dcfce7'; // Delivered
            default:
                return '#e5e7eb';
        }
    }};
    color: ${(props) => {
        switch (props.status) {
            case 0:
                return '#b91c1c'; // Cancelled
            case 1:
            case 3:
                return '#92400e'; // Pending/Confirmed
            case 2:
            case 4:
                return '#1e40af'; // Paid/Shipping
            case 5:
                return '#166534'; // Delivered
            default:
                return '#4b5563';
        }
    }};
`;

const OrderItem = styled.div`
    display: flex;
    padding: 1.5rem;
    border-bottom: 1px solid #f0f0f0;
    cursor: pointer;
    transition: background 0.2s;

    &:hover {
        background: #f9fafb;
    }

    &:last-child {
        border-bottom: none;
    }
`;

const ProductImage = styled.img`
    width: 80px;
    height: 80px;
    object-fit: cover;
    border-radius: 8px;
    margin-right: 1.5rem;
`;

const ProductInfo = styled.div`
    flex: 1;
`;

const ProductName = styled.div`
    font-weight: 500;
    margin-bottom: 0.5rem;
    color: #333;
`;

const ProductQuantity = styled.div`
    color: #666;
    font-size: 0.9rem;
`;

const ProductPrice = styled.div`
    font-weight: 600;
    color: #333;
    min-width: 120px;
    text-align: right;
`;

const OrderFooter = styled.div`
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1.2rem 1.5rem;
    background: #f9fafb;
    border-top: 1px solid #eee;
`;

const TotalAmount = styled.div`
    font-weight: 600;
    font-size: 1.1rem;
    color: #333;
`;

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

const TabsContainer = styled.div`
    display: flex;
    margin-bottom: 1.5rem;
    border-bottom: 1px solid #e5e7eb;
`;

const Tab = styled.button`
    padding: 0.75rem 1.5rem;
    font-size: 0.95rem;
    font-weight: 500;
    color: ${(props) => (props.active ? '#3b82f6' : '#6b7280')};
    background: none;
    border: none;
    cursor: pointer;
    position: relative;
    transition: all 0.2s;

    &:hover {
        color: #3b82f6;
    }

    &::after {
        content: '';
        position: absolute;
        bottom: -1px;
        left: 0;
        width: 100%;
        height: 2px;
        background: #3b82f6;
        transform: scaleX(${(props) => (props.active ? 1 : 0)});
        transition: transform 0.2s;
    }
`;

const EmptyState = styled.div`
    text-align: center;
    padding: 3rem 0;
    color: #6b7280;
`;

const statusIcons = {
    0: <FontAwesomeIcon icon={faXmarkCircle} style={{ marginRight: '0.5rem' }} />,
    1: <FontAwesomeIcon icon={faCube} style={{ marginRight: '0.5rem' }} />,
    2: <FontAwesomeIcon icon={faCube} style={{ marginRight: '0.5rem' }} />,
    3: <FontAwesomeIcon icon={faCube} style={{ marginRight: '0.5rem' }} />,
    4: <FontAwesomeIcon icon={faTruck} style={{ marginRight: '0.5rem' }} />,
    5: <FontAwesomeIcon icon={faCheckCircle} style={{ marginRight: '0.5rem' }} />,
};

//#endregion

const OrderList = () => {
    const dispatch = useDispatch();
    const [orders, setOrders] = useState([]);
    const [filteredOrders, setFilteredOrders] = useState([]);
    const [keyword, setKeyword] = useState('');
    const [activeTab, setActiveTab] = useState('all');
    const auth = useSelector((state) => state.auth);
    const user = useSelector((state) => state.user);
    const navigate = useNavigate();
    const [showFeedbackModal, setShowFeedbackModal] = useState(false);
    const [orderNeedFb, setOrderNeedDb] = useState('');

    const statusMessage = {
        0: 'Đã hủy',
        1: 'Chờ xác nhận',
        2: 'Đã thanh toán, chờ giao hàng',
        3: 'Đơn hàng của bạn đã được xác nhận',
        4: 'Đang giao hàng',
        5: 'Đã giao',
    };

    const GetOrders = async () => {
        dispatch(setLoading(true));
        const resOrders = await GetOrderListServicce({ PageNumber: 1, PageSize: 10, Keyword: keyword });
        setOrders(resOrders?.data?.data);
        setFilteredOrders(resOrders?.data?.data);
        dispatch(setLoading(false));
    };

    useEffect(() => {
        if (!auth.login_Successed) {
            CustomToast.success('Bạn cần đăng nhập để xem danh sách đơn hàng');
            navigate('/login');
            return;
        }

        GetOrders();
    }, [keyword, auth.login_Successed]);

    useEffect(() => {
        filterOrdersByTab();
    }, [activeTab, orders]);

    const filterOrdersByTab = () => {
        switch (activeTab) {
            case 'pending':
                // Status 1 (Pending), 2 (Paid), 3 (Confirmed), 4 (Shipping)
                setFilteredOrders(orders.filter((order) => [1, 2, 3, 4].includes(order.status)));
                break;
            case 'delivered':
                // Status 5 (Delivered)
                setFilteredOrders(orders.filter((order) => order.status === 5));
                break;
            case 'cancelled':
                // Status 0 (Cancelled)
                setFilteredOrders(orders.filter((order) => order.status === 0));
                break;
            default:
                // All orders
                setFilteredOrders(orders);
        }
    };

    const HandleSearchOrder = (value) => {
        setKeyword(value);
    };

    const handleClick = (orderId, userId) => {
        navigate('/order-detail', {
            state: {
                orderId: orderId,
                userId: userId,
            },
        });
    };

    const handleChangeStatusOrder = (orderId) => {
        Swal.fire({
            title: 'Bạn có chắc muốn hủy đơn hàng này?',
            text: '',
            icon: 'warning',
            showCancelButton: true,
            cancelButtonText: 'Không',
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Hủy đơn hàng!',
        }).then(async (result) => {
            if (result.isConfirmed) {
                var res = await ChangeStatusOrder({ orderId: orderId });
                CustomToast.success('Hủy đơn hàng thành công.');
                GetOrders();
            }
        });
    };

    const handleRebuy = async (order) => {
        const prods = await Promise.all(
            order?.orderDetails.map(async (e) => {
                const prod = await GetProductById(e.product_ID);
                prod.data.quantity_order = e.quantity;
                prod.data.product_size = e.size;
                prod.data.product_color = e.color;
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
        orders?.forEach((ord) => {
            if (ord.id == orderNeedFb) {
                ord?.orderDetails?.forEach((item) => {
                    feedbackData.product_ID = item.product_ID;

                    CreateProductFeedback(feedbackData);

                    console.log('Đánh giá gửi lên:', feedbackData);
                    // await SendFeedback(feedbackData); // gọi API thực tế nếu có
                    CustomToast.success('Gửi đánh giá thành công!');
                    setShowFeedbackModal(false);
                });

                setOrderNeedDb('');
                return;
            }
        });
    };

    return (
        <OrderListContainer>
            <h2 style={{ marginBottom: '1.5rem' }}>Danh sách đơn hàng</h2>

            <TabsContainer>
                <Tab active={activeTab === 'all'} onClick={() => setActiveTab('all')}>
                    Tất cả
                </Tab>
                <Tab active={activeTab === 'pending'} onClick={() => setActiveTab('pending')}>
                    Chờ giao hàng
                </Tab>
                <Tab active={activeTab === 'delivered'} onClick={() => setActiveTab('delivered')}>
                    Đã giao hàng
                </Tab>
                <Tab active={activeTab === 'cancelled'} onClick={() => setActiveTab('cancelled')}>
                    Đã hủy
                </Tab>
            </TabsContainer>

            <SearchContainer>
                <FontAwesomeIcon icon={faSearch} size={20} color="#666" style={{ marginRight: '0.8rem' }} />
                <SearchInput
                    type="text"
                    onChange={(e) => setKeyword(e.target.value)}
                    placeholder="Tìm kiếm theo ID đơn hàng hoặc tên sản phẩm"
                    value={keyword}
                />
            </SearchContainer>

            {filteredOrders?.length === 0 ? (
                <EmptyState>
                    <p>Không có đơn hàng nào trong mục này</p>
                </EmptyState>
            ) : (
                filteredOrders?.map((item, ind) => (
                    <OrderCard key={`order-${ind}`}>
                        <OrderHeader>
                            <div>
                                <div>
                                    <span style={{ fontWeight: '500' }}>{item.fullName}</span>
                                    <span style={{ color: '#666', marginLeft: '1rem' }}>#{item?.orderNo}</span>
                                </div>
                                <div>{FormatLongDate(item.order_Date)}</div>
                            </div>
                            <OrderStatus status={item.status}>
                                {statusIcons[item.status]}
                                {statusMessage[item.status]}
                            </OrderStatus>
                        </OrderHeader>

                        <div>
                            {item?.orderDetails?.map((detail, i) => (
                                <OrderItem key={`detail-${i}`} onClick={() => handleClick(detail?.order_ID, user?.id)}>
                                    <ProductImage src={detail?.product_Image} alt={detail?.product_Name} />
                                    <ProductInfo>
                                        <ProductName>{detail?.product_Name}</ProductName>
                                        <div>
                                            <span>Kích cỡ: {detail?.size}</span>
                                            <span className="ms-2">Màu sắc: {detail?.color}</span>
                                        </div>
                                        <ProductQuantity>x{detail?.quantity}</ProductQuantity>
                                    </ProductInfo>
                                    <ProductPrice>{FormatPrice(detail?.product_Price, 'VNĐ')}</ProductPrice>
                                </OrderItem>
                            ))}
                        </div>

                        <OrderFooter>
                            <TotalAmount>Thành tiền: {FormatPrice(item?.total_Amount, 'VNĐ')}</TotalAmount>
                            <div>
                                {item?.status !== 0 && item?.status !== 4 && item?.status !== 5 && (
                                    <ActionButton className="cancel" onClick={() => handleChangeStatusOrder(item?.id)}>
                                        Hủy đơn
                                    </ActionButton>
                                )}
                                {(item?.status === 5 || item?.status === 0) && (
                                    <ActionButton className="rebuy" onClick={() => handleRebuy(item)}>
                                        <FontAwesomeIcon icon={faShoppingCart} style={{ marginRight: '0.5rem' }} />
                                        Mua lại
                                    </ActionButton>
                                )}
                                {item?.status === 5 && !item?.isCommented && (
                                    <ActionButton
                                        className="review"
                                        onClick={() => {
                                            setShowFeedbackModal(true);
                                            setOrderNeedDb(item?.id);
                                        }}
                                    >
                                        <FontAwesomeIcon icon={faStar} style={{ marginRight: '0.5rem' }} />
                                        Đánh giá
                                    </ActionButton>
                                )}
                            </div>
                        </OrderFooter>
                    </OrderCard>
                ))
            )}

            <FeedbackModal
                show={showFeedbackModal}
                onClose={() => setShowFeedbackModal(false)}
                onSubmit={handleSubmitFeedback}
                orderId={''}
            />
        </OrderListContainer>
    );
};

export default OrderList;
