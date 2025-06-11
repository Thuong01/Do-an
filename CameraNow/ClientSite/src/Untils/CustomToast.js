import { toast } from 'react-toastify';

const toastConfig = {
    position: 'top-center',
    autoClose: 5000,
    hideProgressBar: false,
    closeOnClick: false,
    pauseOnHover: true,
    draggable: true,
    progress: undefined,
    theme: 'light',
    // transition: Bounce,
};

const CustomToast = {
    info(message) {
        return toast.info(message, toastConfig);
    },
    success(message) {
        return toast.success(message, toastConfig);
    },
    warning(message) {
        return toast.warn(message, toastConfig);
    },
    error(message) {
        return toast.error(message, toastConfig);
    },
};

export default CustomToast;
