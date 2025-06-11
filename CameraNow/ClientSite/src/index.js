import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import './index.scss';
import { Provider } from 'react-redux';
import { store } from './Redux/Store';
import { CartProvider } from './Context/cartContext';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
    <Provider store={store}>
        <CartProvider>
            <React.StrictMode>
                <App />
            </React.StrictMode>
        </CartProvider>
    </Provider>,
);
