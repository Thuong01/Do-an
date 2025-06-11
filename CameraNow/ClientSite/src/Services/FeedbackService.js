import axios from '../axios';

const GetProductFeedback = (id, page, PageSize) => {
    return axios.get(
        `FeedbackAPIs/feedbacks/product-feedbacks?productId=${id}&PageNumber=${page}&PageSize=${PageSize}`,
    );
};

const CreateProductFeedback = (params) => {
    return axios.post(`FeedbackAPIs/feedbacks`, params);
};

export { GetProductFeedback, CreateProductFeedback };
