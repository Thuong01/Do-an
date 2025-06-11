import { toast } from 'react-toastify';
import { getUserInforService } from '../../Services/UserService';
import { setLoading } from '../Slices/LoadingSlice';
import { removeUser, saveUser } from '../Slices/userSlide';
import { useSelector } from 'react-redux';

const saveUserInfor = () => async (dispatch, getState) => {
    dispatch(setLoading(true));
    var auth = getState().auth;

    if (auth.login_Successed) {
        try {
            var user = await getUserInforService();

            if (user.status === 200) {
                dispatch(saveUser(user?.data?.result));
            }

            return user;
        } catch (error) {
            // toast.error(error.message);
            if (error && error?.response?.status === 401) {
                dispatch(removeUser());
            }
            dispatch(removeUser());
        } finally {
            dispatch(setLoading(false));
        }
    }
};

export { saveUserInfor };
