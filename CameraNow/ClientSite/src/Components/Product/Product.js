import clsx from 'clsx';
import './Product.scss';
import default_product_image from '../../assets/imgs/default_product_image.jpg';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faExchange, faEye, faHeart, faShoppingCart, faStar } from '@fortawesome/free-solid-svg-icons';
import { useContext, useEffect } from 'react';
import { FormatPrice } from '../../Untils/CommonUntil';
import { NavLink } from 'react-router-dom';
import { useDispatch, useSelector } from 'react-redux';
import { CartContext } from '../../Context/cartContext';

const Product = ({ product, flag = '' }) => {
    const dispatch = useDispatch();
    const auth = useSelector((state) => state.auth);
    const user = useSelector((state) => state.user);
    const { handleAddToCart } = useContext(CartContext);
    return (
        <div className={clsx('product')}>
            <NavLink to={`/product-detail/${product?.id}`} className={clsx('product-img')}>
                {product?.image ? <img src={product?.image} alt="" /> : <img src={default_product_image} alt="" />}
                <div className={clsx('product-label')}>
                    {/* <span className={clsx('sale')}>-30%</span> */}
                    {flag === 'NEW' ? (
                        <span className={clsx('new')}>MỚI</span>
                    ) : flag === 'HOT' ? (
                        <span className={clsx('new')}>HOT</span>
                    ) : (
                        <div></div>
                    )}
                </div>
            </NavLink>

            <div className={clsx('product-body')}>
                <h3 className={clsx('product-name')}>
                    <NavLink to={`/product-detail/${product?.id}`}>{product?.name}</NavLink>
                </h3>
                <h4 className={clsx('product-price')}>
                    {product?.promotion_Price && product?.promotion_Price != 0 ? (
                        <>
                            <span>{FormatPrice(product?.promotion_Price, 'VNĐ')}</span>
                        </>
                    ) : (
                        <div>
                            <span>{FormatPrice(product?.price, 'VNĐ')}</span>
                        </div>
                    )}
                </h4>
            </div>
            <div className={clsx('add-to-cart')}>
                <button
                    onClick={() => handleAddToCart(product?.id, user?.cartId, 1, auth)}
                    className={clsx('add-to-cart-btn')}
                >
                    <FontAwesomeIcon icon={faShoppingCart} /> Thêm giỏ hàng
                </button>
            </div>
        </div>
    );
};

export default Product;
