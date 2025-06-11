import axios from '../axios';

const GetBlogsService = ({ CategoryId = null, Status = -1, Sorting = 'name', PageNumber = 1, PageSize = 10 }) => {
    return axios.get('BlogAPIs/blogs', {
        params: {
            CategoryId,
            Status,
            Sorting,
            PageNumber,
            PageSize,
        },
    });
};

const GetBlogByIdService = (id) => {
    return axios.get(`BlogAPIs/blogs/${id}`);
};

export { GetBlogsService, GetBlogByIdService };
