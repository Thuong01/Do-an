import axios from '../axios';

const getCartsService = () => {
    return axios.get('CartAPIs/carts');
};

const addToCartService = ({ productId, cartId, quantity, size, color }) => {
    return axios.post('CartAPIs/carts', {
        product_Id: productId,
        cart_Id: cartId,
        quantity: quantity,
        size: size,
        color: color,
    });
};

const removeFromCard = ({ productId, cartId }) => {
    return axios.delete(`CartAPIs/carts/remove-cart-item?productId=${productId}&cartId=${cartId}`);
};

const UpdateCartItemQuantity = ({ productId, cartId, quantity }) => {
    return axios.put(`CartAPIs/carts/item-quantity?quantity=${quantity}&productId=${productId}&cartId=${cartId}`);
};

export { getCartsService, addToCartService, removeFromCard, UpdateCartItemQuantity };
