import About from './Pages/About';
import ShoppingCart from './Pages/ShoppingCart';
import Home from './Pages/Home/Home';
import Login from './Pages/Login';
import NoNavLayout from './Layouts/NoNavLayout';
import {
    CouponList,
    Order,
    OrderDetail,
    OrderList,
    OrderResult,
    PaymentVnpayReturn,
    ProductDetail,
    Register,
    SearchPage,
} from './Pages';
import AccountInfo from './Pages/AccountInfo/AccountInfo';

var routes = [
    { path: '/', element: Home },
    { path: '/login', element: Login, layout: null },
    { path: '/register', element: Register, layout: null },
    { path: '/about-us', element: About },
    { path: '/order', element: Order, layout: NoNavLayout },
    { path: '/orders', element: OrderList, layout: NoNavLayout },
    { path: '/order-detail', element: OrderDetail, layout: NoNavLayout },
    { path: '/order-result', element: OrderResult, layout: NoNavLayout },
    { path: '/shopping-cart', element: ShoppingCart, layout: NoNavLayout },
    { path: '/search', element: SearchPage, layout: NoNavLayout },
    { path: '/coupons', element: CouponList, layout: NoNavLayout },
    { path: '/product-detail/:id', element: ProductDetail, layout: NoNavLayout },
    { path: '/payment-vnpay-return', element: PaymentVnpayReturn, layout: null },
    { path: '/account-info', element: AccountInfo },
];

export default routes;
