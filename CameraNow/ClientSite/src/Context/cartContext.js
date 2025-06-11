import { createContext, useState } from 'react';
import { addToCartService, getCartsService } from '../Services/CartService';
import CustomToast from '../Untils/CustomToast';

const CartContext = createContext();

const CartProvider = ({ children }) => {
    const [cart, setCart] = useState({
        id: null,
        user_Id: null,
        totalPrice: 0,
        items: [],
    });

    const fetchUserCart = async () => {
        try {
            var res = await getCartsService();
            if (res && res?.data?.success === true) {
                setCart(res?.data?.result);
            }
        } catch (error) {
            console.log(error);
            CustomToast.error('Hiển thị giỏ hàng thất bại!');
        }
    };

    const handleAddToCart = async (productId, cartId, quantity, auth) => {
        try {
            if (auth.login_Successed) {
                var res = await addToCartService({ productId, cartId, quantity });

                if (res?.data?.success == true) {
                    var cartResult = await getCartsService();
                    setCart(cartResult?.data?.result);
                    CustomToast.success('Thêm giỏ hàng thành công!');
                }
            } else {
                CustomToast.error('Vui lòng đăng nhập!');
            }
        } catch (error) {
            console.log(error);
            CustomToast.error('Thêm vào giỏ hàng thất bại!');
        }
    };

    return <CartContext.Provider value={{ cart, fetchUserCart, handleAddToCart }}>{children}</CartContext.Provider>;
};

export { CartProvider, CartContext };
