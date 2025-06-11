import { faClose } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { NavLink, useNavigate } from 'react-router-dom';
import './ShoppingCart.scss';
import { useContext, useEffect, useState } from 'react';
import { FormatPrice } from '../../Untils/CommonUntil';
import { width } from '@fortawesome/free-brands-svg-icons/fa42Group';
import BreadCrumb from '../../Components/BreadCrumb';
import { Form } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { CartContext } from '../../Context/cartContext';
import { useDispatch, useSelector } from 'react-redux';
import CustomToast from '../../Untils/CustomToast';
import { GetProductById } from '../../Services/ProductService';
import { setLoading } from '../../Redux/Slices/LoadingSlice';
import { getCartsService, removeFromCard, UpdateCartItemQuantity } from '../../Services/CartService';

const ShoppingCart = () => {
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { cart, fetchUserCart } = useContext(CartContext);
    const [selectAll, setSelectAll] = useState(false);
    const [selectedItems, setSelectedItems] = useState([]);
    const auth = useSelector((state) => state.auth);
    const [subTotal, setSubTotal] = useState(0);
    const [coupons, setCoupons] = useState([]);
    const [selectedCoupon, setSelectedCoupon] = useState();
    const [t_priceSelected, setT_ProceSelected] = useState(0);

    useEffect(() => {
        if (!auth.login_Successed) {
            toast.error('Vui lòng đăng nhập để xem giỏ hàng!');
            navigate('/');
        }
    }, [auth.login_Successed]);

    useEffect(() => {
        setSubTotal(cart.totalPrice);
    }, [cart]);

    useEffect(() => {
        if (selectAll) {
            setSelectedItems(
                cart?.items?.map((item) => ({
                    product_Id: item.product_Id,
                    quantity_order: item.quantity,
                })),
            );
        } else {
            setSelectedItems([]);
        }
    }, [selectAll, cart.items]);

    const ChangeQuantity = async (type, productId, cartId, quantity) => {
        try {
            var res = await UpdateCartItemQuantity({ productId: productId, cartId, cartId, quantity: quantity });

            if (res?.data?.success) {
                await fetchUserCart();
                // CustomToast.success('Cập nhật thành công.');
            }
        } catch (error) {
            CustomToast.error('Cập nhật sô lượng thất bại.');
        }
    };

    function handleSelectAll() {
        setSelectAll(!selectAll);
    }

    useEffect(() => {
        if (selectedItems.length > 0) {
            const t_price = selectedItems.reduce((sum, item) => {
                return sum + item.product_price * item.quantity_order;
            }, 0);
            setT_ProceSelected(t_price);
        } else {
            setT_ProceSelected(0);
        }
    }, [selectedItems]);

    function handleSelectedItems(product_Id, quantity, price, color, size) {
        const exists = selectedItems.find((item) => item.product_Id === product_Id);

        if (exists) {
            setSelectedItems(selectedItems.filter((item) => item.product_Id !== product_Id));
        } else {
            setSelectedItems([
                ...selectedItems,
                {
                    product_Id,
                    quantity_order: quantity,
                    product_price: price,
                    product_color: color,
                    product_size: size,
                },
            ]);
        }
    }

    async function handleCheckOut(items) {
        if (selectedItems && selectedItems.length > 0) {
            dispatch(setLoading(true));
            if (auth.login_Successed) {
                // anhtv: Chờ các call api được gọi hoàn tất trước khi chuyển sang trang order
                const prods = await Promise.all(
                    selectedItems.map(async (e) => {
                        const prod = await GetProductById(e.product_Id);
                        prod.data.quantity_order = e.quantity_order;
                        prod.data.product_color = e.product_color;
                        prod.data.product_size = e.product_size;
                        return prod?.data;
                    }),
                );
                dispatch(setLoading(false));

                // product.quantity_order = quantity;
                navigate('/order', {
                    state: {
                        products: prods,
                        detail_yn: true,
                        from_cart_yn: true,
                        cartID: cart.id,
                    },
                });
            } else {
                CustomToast.info('Bạn chưa đăng nhập, có muốn đăng nhập!');
            }
        } else {
            CustomToast.info('Chưa chọn sản phẩm!');
        }
    }

    const handleDeleteFromCard = async (cartId, productID) => {
        try {
            if (!cartId || !productID) {
                return;
            }
            var res = await removeFromCard({ productId: productID, cartId: cartId });
            if (res?.data?.success) {
                await fetchUserCart();
                CustomToast.success('Xóa thành công.');
            }
        } catch (error) {
            CustomToast.error('Xóa không thành công.');
        }
    };

    return (
        <>
            <BreadCrumb action_item="Giỏ hàng" />

            {/* Shopping Cart Section Begin */}
            <section className="section shopping-cart">
                <div className="container">
                    <div className="row">
                        <div className="col-lg-8">
                            <div className="shopping__cart__table">
                                {cart?.items?.length > 0 ? (
                                    <>
                                        <table>
                                            <thead>
                                                <tr>
                                                    <th>
                                                        <Form.Check checked={selectAll} onChange={handleSelectAll} />
                                                    </th>
                                                    <th>Sản phẩm</th>
                                                    <th>Số lượng</th>
                                                    <th>Tổng</th>
                                                    <th />
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {cart?.items?.map((cart, index) => {
                                                    return (
                                                        <tr key={index}>
                                                            <td>
                                                                <Form.Check
                                                                    checked={selectedItems.some(
                                                                        (item) => item.product_Id === cart.product_Id,
                                                                    )}
                                                                    onChange={() =>
                                                                        handleSelectedItems(
                                                                            cart.product_Id,
                                                                            cart.quantity,
                                                                            cart.product_Price,
                                                                        )
                                                                    }
                                                                />
                                                            </td>
                                                            <td className="product__cart__item">
                                                                <div className="product__cart__item__pic">
                                                                    <img src={cart.product_Image} alt="" />
                                                                </div>
                                                                <div className="product__cart__item__text">
                                                                    <h6>{cart.product_Name}</h6>

                                                                    <h5>{FormatPrice(cart.product_Price, 'VNĐ')}</h5>
                                                                </div>
                                                            </td>
                                                            <td className="quantity__item">
                                                                <div style={{ width: '100px' }}>
                                                                    <div className="input-number">
                                                                        <input
                                                                            value={cart.quantity}
                                                                            min={0}
                                                                            max={9999}
                                                                            type="number"
                                                                            onChange={(e) => {
                                                                                var qty = Number(e.target.value);
                                                                                cart.quantity = qty;
                                                                                console.log(cart.quantity);

                                                                                // ChangeQuantity(
                                                                                //     '',
                                                                                //     cart.product_Id,
                                                                                //     cart.cart_Id,
                                                                                //     qty,
                                                                                // );
                                                                            }}
                                                                        />
                                                                        <span
                                                                            onClick={() =>
                                                                                ChangeQuantity(
                                                                                    'add',
                                                                                    cart.product_Id,
                                                                                    cart.cart_Id,
                                                                                    cart.quantity + 1,
                                                                                )
                                                                            }
                                                                            className="qty-up"
                                                                        >
                                                                            +
                                                                        </span>
                                                                        <span
                                                                            onClick={() => {
                                                                                ChangeQuantity(
                                                                                    'sub',
                                                                                    cart.product_Id,
                                                                                    cart.cart_Id,
                                                                                    cart.quantity - 1,
                                                                                );
                                                                            }}
                                                                            className="qty-down"
                                                                        >
                                                                            -
                                                                        </span>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                            <td className="cart__price">
                                                                {FormatPrice(cart.totalItemPrice, 'VNĐ')}
                                                            </td>
                                                            <td
                                                                onClick={() =>
                                                                    handleDeleteFromCard(
                                                                        cart?.cart_Id,
                                                                        cart?.product_Id,
                                                                        cart?.color,
                                                                        cart?.size,
                                                                    )
                                                                }
                                                                className="cart__close"
                                                            >
                                                                <FontAwesomeIcon icon={faClose} />
                                                            </td>
                                                        </tr>
                                                    );
                                                })}
                                            </tbody>
                                        </table>
                                    </>
                                ) : (
                                    <>
                                        <div className="text-center">Giỏ hàng trống</div>
                                    </>
                                )}
                            </div>
                            <div className="row">
                                <div className="col-lg-6 col-md-6 col-sm-6">
                                    <div className="continue__btn">
                                        <NavLink to={'/'}>Tiếp tục mua sắm</NavLink>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="col-lg-4">
                            <div className="cart__total">
                                <h6>Tổng</h6>
                                <ul>
                                    <li>
                                        Thành tiền <span>{FormatPrice(t_priceSelected, 'VNĐ')}</span>
                                    </li>
                                </ul>
                                <button onClick={() => handleCheckOut(selectedItems)} className="primary-btn">
                                    Đặt hàng
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            {/* Shopping Cart Section End */}
        </>
    );
};

export default ShoppingCart;
