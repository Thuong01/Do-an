import axios from '../axios';

const GetProducts = ({ Status = -1, PageNumber = 1, PageSize = 10, Sorting = 'name', Category = '', Filter = '' }) => {
    return axios.get('ProductAPIs/products', {
        params: {
            Category,
            Status,
            Sorting,
            PageNumber,
            PageSize,
            Filter,
        },
    });
};

const RecommentsProducts = ({ product_id = '', useCartData = false }) => {
    return axios.get(
        `Recommendation/recommendations/product-recomments/${product_id}?useCartData=${useCartData}&limit=5`,
    );
};

const GetQuantityFromSize = ({ product_id = '', size = '' }) => {
    return axios.get(`ProductAPIs/products/quantity-by-size?productId=${product_id}&productSize=${size}`);
};

const GetRelativeProduct = ({ Category = null, Status = -1, PageNumber = 1, PageSize = 10, Sorting = 'name' }) => {
    return axios.get('ProductAPIs/products', {
        params: {
            Category,
            Status,
            Sorting,
            PageNumber,
            PageSize,
        },
    });
};

const GetProductById = (id) => {
    return axios.get(`ProductAPIs/products/${id}`);
};

const GetProductBreadcrumbs = (id) => {
    return axios.get(`ProductAPIs/products/breadcrumbs/${id}`);
};

export {
    GetProducts,
    GetProductById,
    GetRelativeProduct,
    GetProductBreadcrumbs,
    RecommentsProducts,
    GetQuantityFromSize,
};
