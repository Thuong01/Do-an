import axios from '../axios';

const getUserInforService = () => {
    return axios.get('UserAPI/user/infor');
};

const RegisterService = ({ userName, email, fullName, password, address, phoneNumber, confirmPassword }) => {
    return axios.post('https://localhost:7089/api/LoginAPI/register', {
        userName,
        email,
        fullName,
        password,
        address,
        phoneNumber,
        confirmPassword,
    });
};

const EditProfile = (userinfo) => {
    return axios.post('UserAPI/user/edit-profile', userinfo);
};

export { getUserInforService, RegisterService, EditProfile };
