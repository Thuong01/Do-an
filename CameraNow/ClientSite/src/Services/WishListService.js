import axios from '../axios';

const getWishListSerice = () => {
    return axios.get('WishListAPIs/wish-list');
};

const addToWishListService = (productId, wishListId) => {
    return axios.post('WishListAPIs/wish-list', {
        product_Id: productId,
        wish_List_Id: wishListId,
    });
};

export { getWishListSerice, addToWishListService };
