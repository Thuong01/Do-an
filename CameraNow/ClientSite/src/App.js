import { BrowserRouter, Route, Routes } from 'react-router-dom';
import routes from './routes';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import Loading from './Components/Loading';
import DefaultLayout from './Layouts/DefaultLaout';
import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { setLoading } from './Redux/Slices/LoadingSlice';
import { saveUserInfor } from './Redux/Actions/userAction';

function App() {
    var dispatch = useDispatch();
    const user = useSelector((state) => state.user);

    useEffect(() => {
        const fetchUserInfor = async () => {
            dispatch(setLoading(true));

            try {
                const token = localStorage.getItem('access_token');
                const expiresAt = localStorage.getItem('expiresAt');

                if (token) {
                    var user1 = await dispatch(saveUserInfor());
                }
            } catch (error) {
                console.log(error);
            } finally {
                dispatch(setLoading(false));
            }
        };

        fetchUserInfor();
    }, [dispatch]);

    return (
        <div className="main">
            <header className="App-header">
                <div>
                    <BrowserRouter>
                        <div>
                            <Routes>
                                {routes?.map((route, index) => {
                                    const Page = route.element;
                                    let Layout = DefaultLayout;

                                    if (route.layout) {
                                        Layout = route.layout;
                                    } else if (route.layout === null) {
                                        Layout = React.Fragment;
                                    }

                                    return (
                                        <Route
                                            key={`route-${index}`}
                                            path={route.path}
                                            element={
                                                <Layout>
                                                    <Page />
                                                </Layout>
                                            }
                                        ></Route>
                                    );
                                })}
                            </Routes>
                        </div>
                    </BrowserRouter>
                </div>
            </header>

            <Loading />
            <ToastContainer />
        </div>
    );
}

export default App;
