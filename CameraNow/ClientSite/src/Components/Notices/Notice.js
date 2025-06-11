import { useState, useEffect, useContext } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faClose, faArrowRight, faShoppingCart } from '@fortawesome/free-solid-svg-icons';
import { NavLink, useNavigate } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { CartContext } from '../../Context/cartContext';
import { setLoading } from '../../Redux/Slices/LoadingSlice';
import { FormatPrice } from '../../Untils/CommonUntil';
import CustomToast from '../../Untils/CustomToast';
import './Notice.scss';
import { removeFromCard } from '../../Services/CartService';

const Notice = ({ title = 'Cart', items = [], isCart = true }) => {
    const [isOpen, setIsOpen] = useState(false);
    const auth = useSelector((state) => state.auth);
    const { fetchUserCart } = useContext(CartContext);
    const navigate = useNavigate();
    const dispatch = useDispatch();

    // Calculate total items count
    const totalItems = items?.items?.length || 0;

    // Close dropdown when clicking outside
    useEffect(() => {
        const handleClickOutside = (e) => {
            if (!e.target.closest('.notice-container')) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const handleCheckLogin = (e) => {
        if (!auth.login_Successed) {
            e.preventDefault();
            CustomToast.error('Vui lòng đăng nhập!');
            setIsOpen(false);
            return false;
        }
        return true;
    };

    const handleDeleteFromCart = async (cartId, productId) => {
        try {
            if (!cartId || !productId) return;

            const res = await removeFromCard({ productId, cartId });
            if (res?.data?.success) {
                await fetchUserCart();
            }
        } catch (error) {
            CustomToast.error('Xóa không thành công');
        }
    };

    const handleCheckOut = () => {
        try {
            if (!items?.items?.length) {
                CustomToast.warning('Giỏ hàng trống');
                return;
            }

            dispatch(setLoading(true));
            navigate('/order', {
                state: {
                    products: items.items,
                    detail_yn: true,
                },
            });
        } catch (err) {
            CustomToast.error('Chuyển trang thất bại!');
        } finally {
            dispatch(setLoading(false));
            setIsOpen(false);
        }
    };

    return (
        <div className="notice-container">
            {/* Trigger Button */}
            <div className="notice-trigger-wrapper">
                {totalItems > 0 && <div className="notice-total-items">{totalItems}</div>}
                <button
                    className="notice-trigger"
                    onClick={() => {
                        if (!auth.login_Successed) {
                            CustomToast.error('Vui lòng đăng nhập!');
                            return;
                        }
                        setIsOpen(!isOpen);
                    }}
                    aria-label={title}
                >
                    <FontAwesomeIcon icon={faShoppingCart} />
                    {items?.items?.length > 0 && <span className="notice-badge">{items.items.length}</span>}
                </button>
            </div>

            {/* Dropdown Content */}
            {isOpen && (
                <div className="notice-dropdown">
                    <div className="notice-header">
                        <h3>{title}</h3>
                        <button className="notice-close" onClick={() => setIsOpen(false)} aria-label="Close">
                            <FontAwesomeIcon icon={faClose} />
                        </button>
                    </div>

                    {/* Items List */}
                    <div className="notice-items">
                        {items?.items?.length > 0 ? (
                            items.items.map((item, index) => (
                                <div key={`${item.product_Id}-${index}`} className="notice-item">
                                    <div className="item-image">
                                        <img src={item.product_Image} alt={item.product_Name} loading="lazy" />
                                    </div>
                                    <div className="item-details">
                                        <NavLink
                                            to={`/product-detail/${item.product_Id}`}
                                            className="item-name"
                                            onClick={() => setIsOpen(false)}
                                        >
                                            {item.product_Name}
                                        </NavLink>

                                        <div className="item-price">
                                            <span className="item-quantity">x{item.quantity}</span>
                                            {FormatPrice(item.product_Price, 'VNĐ')}
                                        </div>
                                    </div>
                                    <button
                                        className="item-remove"
                                        onClick={() => handleDeleteFromCart(item.cart_Id, item.product_Id)}
                                        aria-label="Remove item"
                                    >
                                        <FontAwesomeIcon icon={faClose} />
                                    </button>
                                </div>
                            ))
                        ) : (
                            <div className="notice-empty">
                                <p>Giỏ hàng trống</p>
                            </div>
                        )}
                    </div>

                    {/* Footer */}
                    <div className="notice-footer">
                        {isCart && items?.items?.length > 0 && (
                            <div className="notice-summary">
                                <span>{items.items.length} sản phẩm</span>
                                <strong>Tổng: {FormatPrice(items.totalPrice, 'VNĐ')}</strong>
                            </div>
                        )}

                        <div className="notice-actions">
                            <NavLink to="/shopping-cart" className="notice-view" onClick={() => setIsOpen(false)}>
                                Xem giỏ hàng
                            </NavLink>
                            <button
                                className="notice-checkout"
                                onClick={handleCheckOut}
                                disabled={!items?.items?.length}
                            >
                                Thanh toán
                                <FontAwesomeIcon icon={faArrowRight} />
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Notice;
