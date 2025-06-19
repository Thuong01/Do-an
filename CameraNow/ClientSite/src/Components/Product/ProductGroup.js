import clsx from 'clsx';
import './ProductGroup.scss';
import Product from './Product';
import '../../slick.css';
import '../../slick-theme.css';
import Slider from 'react-slick';
import { toSnakeCase } from '../../Untils/CommonUntil';
import { NavLink } from 'react-router-dom';
// import 'jquery-zoom';

const ProductGroup = ({ title, tabs = [], products = [], flag = '' }) => {
    const settings = {
        dots: false,
        autoplay: false,
        autoplaySpeed: 2000,
        cssEase: 'linear',
        infinite: true,
        speed: 300,
        asNavFor: undefined,
        arrows: true,
        nextArrow: null,
        prevArrow: null,
        responsive: [
            {
                breakpoint: 991,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                },
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                },
            },
        ],
        slidesToShow: 4,
        slidesToScroll: 1,
    };

    return (
        <>
            <div className="col-md-12">
                <div className={clsx('section-title')}>
                    <h5 className={clsx('title')}>{title}</h5>
                    <NavLink to={`/search`} className={clsx('see-more', 'ms-3')}>
                        Xem thêm
                    </NavLink>

                    <div className={clsx('section-nav')}>
                        {/* <ul className={clsx('section-tab-nav', 'tab-nav')}>
                            {tabs?.map((val, index) => {
                                return (
                                    <li key={index} className={clsx('active')}>
                                        <a data-toggle="tab" href="#tab1">
                                            {val}
                                        </a>
                                    </li>
                                );
                            })}
                        </ul> */}
                    </div>
                </div>
            </div>

            <div className="col-md-12">
                <div className="row">
                    <div className={clsx('products-tabs')}>
                        <div id="tab1" className={clsx('tab-pane', 'active')}>
                            <div data-nav="#slick-nav-1">
                                <Slider className={clsx('products-slick')} {...settings}>
                                    {products?.map((prod, index) => {
                                        return <Product flag={flag} key={index} product={prod} />;
                                    })}
                                </Slider>
                            </div>

                            <div id="slick-nav-1" className={clsx('products-slick-nav')}>
                                {/* <button className="slick-prev slick-arrow" aria-label="Previous" type="button">
                                    Previous
                                </button>
                                <button className="slick-next slick-arrow" aria-label="Next" type="button">
                                    Next
                                </button> */}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
};

export default ProductGroup;
