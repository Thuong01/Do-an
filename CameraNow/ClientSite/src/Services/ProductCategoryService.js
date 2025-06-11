import axios from '../axios';

const GetProductCategories = ({ filter = null, Status = -1, Sorting = 'name' }) => {
    return axios.get('ProductCategoryAPIs/product-categories', {
        params: {
            Status,
            Sorting,
        },
    });
};

export { GetProductCategories };
