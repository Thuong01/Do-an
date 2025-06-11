import './Loading.scss';
import loadingGif from '../../assets/imgs/loading.gif';
import { useSelector } from 'react-redux';

const Loading = () => {
    const isLoading = useSelector((state) => state.loading.isLoading);

    return (
        <>
            {isLoading ? (
                <div id="loading">
                    <div id="gif-wrap">
                        <img src={loadingGif} />
                    </div>
                </div>
            ) : (
                <div></div>
            )}
        </>
    );
};

export default Loading;
