import { NavLink, useLocation, useNavigate, useParams } from 'react-router-dom';
import clsx from 'clsx';
import { toast } from 'react-toastify';
import Slider from 'react-slick';
import '../../slick.css';
import '../../slick-theme.css';
import './ProductDetails.scss';
import { FormatLongDate, FormatLongDateTime, FormatPrice } from '../../Untils/CommonUntil';
import { useContext, useEffect, useRef, useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEnvelope, faShoppingCart, faStar, faStarHalfStroke } from '@fortawesome/free-solid-svg-icons';
import { faStar as faStarRegular } from '@fortawesome/free-regular-svg-icons';
import { faStar as faStarO } from '@fortawesome/free-regular-svg-icons';
import { faFacebookF, faGooglePlus, faTwitter } from '@fortawesome/free-brands-svg-icons';
import { Tab, Tabs } from 'react-bootstrap';
import ProductGroup from '../../Components/Product/ProductGroup';
import BreadCrumb from '../../Components/BreadCrumb';
import { useDispatch, useSelector } from 'react-redux';
import {
    GetProductBreadcrumbs,
    GetProductById,
    GetQuantityFromSize,
    GetRelativeProduct,
    RecommentsProducts,
} from '../../Services/ProductService';
import { setLoading } from '../../Redux/Slices/LoadingSlice';
import { CartContext } from '../../Context/cartContext';
import { GetProductFeedback } from '../../Services/FeedbackService';
import CustomToast from '../../Untils/CustomToast';
import Product from '../../Components/Product/Product';
import useTitle from '../../Context/useTitle';

const ProductDetails = () => {
    const dispatch = useDispatch();
    const location = useLocation();
    const auth = useSelector((state) => state.auth);
    const user = useSelector((state) => state.user);
    const { id } = useParams();
    const [mainImg, setMainImg] = useState(null);
    const [secondImg, setSecondImg] = useState(null);
    let mainImgSliderRef = useRef(null);
    let secondImgSliderRef = useRef(null);
    let [quantity, setQuantity] = useState(1);
    const [product, setProduct] = useState();
    const [relatedProducts, setRelatedProducts] = useState([]);
    const { handleAddToCart } = useContext(CartContext);
    const [prodBreadcrumbs, setProdBreadCrumb] = useState([]);
    const [feedbacks, setFeedbacks] = useState({
        rating_Average: 0,
        feedbacks_Count: 0,
        stars: [],
        data: {
            page: 1,
            count: 0,
            totalPage: 0,
            totalCount: 10,
            data: [],
        },
    });
    const [fbPage, setFbPage] = useState(1);
    const [fbPageSize, setFbPageSize] = useState(10);
    const navigate = useNavigate();
    const [recomments, setRecomments] = useState([]);

    const [sizeQuantity, setSizeQuantity] = useState(0);
    useTitle('Chi tiết sản phẩm');

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [location]);

    const fetchProductById = async () => {
        dispatch(setLoading(true));

        try {
            const productData = await GetProductById(id);

            if (productData) {
                setProduct(productData?.data);
                setSizeQuantity(productData?.data.quantity);
            }
        } catch (error) {
            console.log(error);
        } finally {
            dispatch(setLoading(false));
        }
    };

    const fetchProductBreadcrumb = async () => {
        const res = await GetProductBreadcrumbs(id);

        if (res) {
            setProdBreadCrumb(res?.data?.result);
        }
    };

    const fetchRecommentsProducts = async () => {
        const res = await RecommentsProducts({ product_id: id });
        setRecomments(res?.data?.result?.recommendations);
    };

    const fetchProductFeedback = async () => {
        const res = await GetProductFeedback(id, fbPage, fbPageSize);

        if (res?.data?.success) {
            setFeedbacks(res?.data?.result);
        }
    };

    const fetchRelativeProduct = async () => {
        try {
            var res = await GetRelativeProduct({
                Category: product?.category_ID,
                Status: -1,
                PageNumber: 1,
                PageSize: 10,
                Sorting: 'name',
            });
            setRelatedProducts(res?.data?.data);
        } catch (error) {
            console.error(error);
        }
    };

    useEffect(() => {
        fetchProductById();
        fetchProductBreadcrumb();
        fetchProductFeedback();
        fetchRecommentsProducts();
    }, [id]);

    useEffect(() => {
        fetchRelativeProduct();
    }, [product?.category_ID]);

    useEffect(() => {
        setMainImg(mainImgSliderRef);
        setSecondImg(secondImgSliderRef);
    }, []);

    const productMainImgSettings = {
        dots: false,
        infinite: true,
        speed: 300,
        asNavFor: secondImg,
        arrows: true,
        fade: true,
        slidesToShow: 1,
        slidesToScroll: 1,
    };

    const productSecondImgSettings = {
        slidesToShow: 3,
        slidesToScroll: 1,
        arrows: true,
        centerMode: true,
        focusOnSelect: true,
        asNavFor: mainImg,
        centerPadding: 0,
        vertical: true,
        swipeToSlide: true,
        responsive: [
            {
                breakpoint: 991,
                settings: {
                    vertical: false,
                    arrows: false,
                    dots: true,
                },
            },
        ],
    };

    function ChangeQuantity(type) {
        setQuantity((prevVal) => {
            let newQuantity = prevVal;

            if (type === 'add') {
                if (prevVal + 1 > sizeQuantity) {
                    toast.warning('Số lượng vượt quá số lượng còn lại trong kho!');
                    return sizeQuantity;
                }
                newQuantity = prevVal + 1;
            } else if (type === 'sub') {
                newQuantity = Math.max(prevVal - 1, 1); // Đảm bảo không nhỏ hơn 1
            }

            return newQuantity;
        });
    }

    const handleOrder = (prod_id) => {
        if (quantity > sizeQuantity) {
            CustomToast.warning('Số lượng sản phẩm đang lớn hơn số lựng trong kho');
            return false;
        }

        if (auth.login_Successed) {
            product.quantity_order = quantity;

            navigate('/order', {
                state: {
                    products: [product],
                    detail_yn: true,
                },
            });
        } else {
            CustomToast.info('Bạn chưa đăng nhập, có muốn đăng nhập!');
        }
    };

    const [currentIndex, setCurrentIndex] = useState(0);
    const [thumbnailScrollPosition, setThumbnailScrollPosition] = useState(0);
    const thumbnailContainerRef = useRef(null);

    // Handle thumbnail click
    const handleThumbnailClick = (index) => {
        setCurrentIndex(index);
        scrollThumbnailIntoView(index);
    };

    // Scroll thumbnail into view
    const scrollThumbnailIntoView = (index) => {
        if (thumbnailContainerRef.current) {
            const thumbnails = thumbnailContainerRef.current.children;
            if (thumbnails[index]) {
                const thumbnail = thumbnails[index];
                const container = thumbnailContainerRef.current;
                const containerWidth = container.offsetWidth;
                const thumbnailLeft = thumbnail.offsetLeft;
                const thumbnailWidth = thumbnail.offsetWidth;

                // Calculate scroll position
                const scrollTo = thumbnailLeft - containerWidth / 2 + thumbnailWidth / 2;
                container.scrollTo({
                    left: scrollTo,
                    behavior: 'smooth',
                });
            }
        }
    };

    // Auto-scroll thumbnails when currentIndex changes
    useEffect(() => {
        scrollThumbnailIntoView(currentIndex);
    }, [currentIndex]);

    return (
        <div>
            <>
                <BreadCrumb brecrumbs={prodBreadcrumbs} action_item={product?.name} />

                {/* SECTION */}
                <div className="section">
                    {/* container */}
                    <div className="container">
                        {/* row */}
                        <div className="row">
                            {/* <div className="col-md-5 col-md-push-2">
                                <div id={clsx('product-main-img')}>
                                    <Slider ref={(slider) => (mainImgSliderRef = slider)} {...productMainImgSettings}>
                                        {product?.images?.map((img, index) => {
                                            return (
                                                <div key={index} className={clsx('product-preview')}>
                                                    <img src={img.link} alt={product?.name} />
                                                </div>
                                            );
                                        })}
                                    </Slider>
                                </div>
                            </div> */}

                            {/* <div className="col-md-2 col-md-pull-5">
                                <div style={{}} id={clsx('product-imgs')}>
                                    <Slider
                                        ref={(slider) => (secondImgSliderRef = slider)}
                                        {...productSecondImgSettings}
                                    >
                                        {product?.images?.map((img, index) => {
                                            return (
                                                <div key={index} className={clsx('product-preview')}>
                                                    <img width={186} height={186} src={img.link} alt="" />
                                                </div>
                                                // width={186} height={186}
                                            );
                                        })}
                                    </Slider>
                                </div>
                            </div> */}

                            <div className="row col-md-5">
                                {/* Product main img */}
                                <div className="col-md-10 col-md-push-2">
                                    <div id={clsx('product-main-img')}>
                                        <div className="product-preview">
                                            {product?.images?.length > 0 && (
                                                <img
                                                    src={product.images[currentIndex].link}
                                                    alt={product?.name}
                                                    className="img-responsive"
                                                />
                                            )}
                                        </div>
                                    </div>
                                </div>
                                {/* /Product main img */}

                                {/* Product thumb imgs */}
                                <div className="col-md-2 col-md-pull-5">
                                    <div
                                        id={clsx('product-imgs')}
                                        ref={thumbnailContainerRef}
                                        style={{
                                            height: '100%',
                                            overflowY: 'auto',
                                            display: 'flex',
                                            flexDirection: 'column',
                                            gap: '10px',
                                            scrollBehavior: 'smooth',
                                            padding: '5px 0',
                                        }}
                                    >
                                        {product?.images?.map((img, index) => (
                                            <div
                                                key={index}
                                                className={clsx('product-preview', {
                                                    'product-preview-active': index === currentIndex,
                                                })}
                                                onClick={() => handleThumbnailClick(index)}
                                                style={{
                                                    cursor: 'pointer',
                                                    border:
                                                        index === currentIndex ? '2px solid #D10024' : '1px solid #ddd',
                                                    padding: '2px',
                                                    transition: 'border 0.3s ease',
                                                    flexShrink: 0,
                                                }}
                                            >
                                                <img
                                                    src={img.link}
                                                    alt=""
                                                    width={186}
                                                    height={186}
                                                    style={{
                                                        objectFit: 'cover',
                                                        width: '100%',
                                                        height: 'auto',
                                                        aspectRatio: '1/1',
                                                    }}
                                                />
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            </div>

                            <div className="col-md-7">
                                <div className="product-details">
                                    <h2 className="product-name">{product?.name}</h2>
                                    <div>
                                        {[...Array(Math.floor(feedbacks?.rating_Average)).keys()].map((i) => (
                                            <FontAwesomeIcon
                                                style={{ color: '#f5c518', marginRight: 2 }}
                                                key={`number-of-rates-${i}`}
                                                icon={faStar}
                                            />
                                        ))}
                                        {feedbacks?.rating_Average > Math.floor(feedbacks?.rating_Average) && (
                                            <FontAwesomeIcon
                                                style={{ color: '#f5c518', marginRight: 2 }}
                                                icon={faStarHalfStroke}
                                            />
                                        )}
                                        {[...Array(5 - Math.ceil(feedbacks?.rating_Average)).keys()].map((i) => (
                                            <FontAwesomeIcon
                                                style={{ color: '#f5c518', marginRight: 2 }}
                                                key={`number-of-rates-reject-${i}`}
                                                icon={faStarRegular}
                                            />
                                        ))}
                                        <span className="review-link">{feedbacks?.feedbacks_Count || 0} Review(s)</span>
                                    </div>
                                    <div>
                                        <h3 className="product-price">
                                            {!product?.promotion_Price || product?.promotion_Price == 0 ? (
                                                <>{FormatPrice(product?.price, 'VNĐ')}</>
                                            ) : (
                                                <>{FormatPrice(product?.promotion_Price, 'VNĐ')}</>
                                            )}

                                            {!product?.promotion_Price || product?.promotion_Price == 0 ? (
                                                <></>
                                            ) : (
                                                <>
                                                    <del className="product-old-price">
                                                        {FormatPrice(product?.price, 'VNĐ')}
                                                    </del>
                                                </>
                                            )}
                                        </h3>
                                        <span className="product-available">Còn hàng</span>
                                    </div>

                                    <div>
                                        <label>
                                            <div className="qty-label">
                                                Số lượng
                                                <div className="input-number">
                                                    <input
                                                        onChange={(e) => {
                                                            const value = Number(e.target.value);
                                                            if (value >= sizeQuantity) {
                                                                CustomToast.error(
                                                                    'Số lượng đang lớn hơn số lượng còn lại',
                                                                );
                                                                setQuantity(1);
                                                            } else {
                                                                setQuantity(value);
                                                            }
                                                        }}
                                                        value={quantity}
                                                        min={1}
                                                        max={9999}
                                                        type="number"
                                                    />

                                                    <span onClick={() => ChangeQuantity('add')} className="qty-up">
                                                        +
                                                    </span>
                                                    <span
                                                        onClick={() => {
                                                            ChangeQuantity('sub');
                                                        }}
                                                        className="qty-down"
                                                    >
                                                        -
                                                    </span>
                                                </div>
                                            </div>
                                        </label>
                                    </div>

                                    <div className="d-flex">
                                        <div className="add-to-cart me-3">
                                            <button
                                                onClick={() =>
                                                    handleAddToCart(product?.id, user?.cartId, quantity, auth)
                                                }
                                                className={clsx('add-to-cart-btn1')}
                                            >
                                                Thêm vào giỏ hàng
                                            </button>
                                        </div>

                                        <div className="add-to-cart">
                                            <button
                                                onClick={() => handleOrder(product?.id)}
                                                className={clsx('add-to-cart-btn')}
                                            >
                                                <FontAwesomeIcon icon={faShoppingCart} /> Mua ngay
                                            </button>
                                        </div>
                                    </div>
                                    <ul className={clsx('product-links')}>
                                        <li>Phân loại:</li>
                                        <li className="badge text-bg-light">
                                            <NavLink to={`/search?category=${product?.category_Alias}&page=1`}>
                                                {product?.category_Name}
                                            </NavLink>
                                        </li>
                                    </ul>
                                </div>
                            </div>

                            <div className="col-md-12">
                                <div id="product-tab">
                                    <Tabs defaultActiveKey="description">
                                        <Tab eventKey="description" title="Mô tả sản phẩm">
                                            <div className="row">
                                                <div className="col-md-12">
                                                    <p dangerouslySetInnerHTML={{ __html: product?.description }}></p>
                                                </div>
                                            </div>
                                        </Tab>

                                        <Tab eventKey="reviews" title="Đánh giá sản phẩm">
                                            {feedbacks?.data?.data?.length <= 0 ? (
                                                <>
                                                    <div className="text-center">
                                                        <p>Không có bình luận nào!</p>
                                                    </div>
                                                </>
                                            ) : (
                                                <>
                                                    <div className="row">
                                                        {/* Rating */}
                                                        <div className="col-md-3">
                                                            <div id="rating">
                                                                <div className="rating-avg">
                                                                    <span>{product?.rating}</span>
                                                                    <div className="rating-stars">
                                                                        {[
                                                                            ...Array(
                                                                                Math.floor(product?.rating) || 0,
                                                                            ).keys(),
                                                                        ].map((i) => (
                                                                            <FontAwesomeIcon
                                                                                key={`total-rating-${i}`}
                                                                                icon={faStar}
                                                                            />
                                                                        ))}

                                                                        {product?.rating >
                                                                            Math.floor(product?.rating) && (
                                                                            <FontAwesomeIcon icon={faStarHalfStroke} />
                                                                        )}

                                                                        {[
                                                                            ...Array(
                                                                                5 - Math.ceil(product?.rating || 0),
                                                                            ).keys(),
                                                                        ].map((i) => (
                                                                            <FontAwesomeIcon
                                                                                key={`total-rating-${i}`}
                                                                                icon={faStarO}
                                                                            />
                                                                        ))}
                                                                    </div>
                                                                </div>
                                                                <ul className="rating">
                                                                    {feedbacks?.stars?.map((item, index) => (
                                                                        <li key={`fb-star-percent-${index}`}>
                                                                            <div className="rating-stars">
                                                                                {/* Render số sao đã được đánh giá */}
                                                                                {[
                                                                                    ...Array(parseInt(item?.star) || 0),
                                                                                ].map((_, i) => (
                                                                                    <FontAwesomeIcon
                                                                                        key={`star-filled-${index}-${i}`}
                                                                                        icon={faStar}
                                                                                    />
                                                                                ))}

                                                                                {/* Render số sao còn lại chưa được đánh giá */}
                                                                                {[...Array(5 - (item?.star || 0))].map(
                                                                                    (_, i) => (
                                                                                        <FontAwesomeIcon
                                                                                            key={`star-empty-${index}-${i}`}
                                                                                            icon={faStarO}
                                                                                        />
                                                                                    ),
                                                                                )}
                                                                            </div>
                                                                            <div className="rating-progress">
                                                                                <div
                                                                                    style={{
                                                                                        width: `${Math.ceil(
                                                                                            item?.percent,
                                                                                        )}%`,
                                                                                    }}
                                                                                />
                                                                            </div>
                                                                            <span className="sum">{item?.sumStar}</span>
                                                                        </li>
                                                                    ))}
                                                                </ul>
                                                            </div>
                                                        </div>

                                                        <div className="col-md-6">
                                                            <div id="reviews">
                                                                <ul className="reviews">
                                                                    {feedbacks?.data?.data?.map((item, index) => (
                                                                        <li key={`prod-feedback-id-${item?.id}`}>
                                                                            <div className="review-heading">
                                                                                <h5 className="name">
                                                                                    {item?.user_Name}
                                                                                </h5>
                                                                                <p className="date">
                                                                                    {FormatLongDateTime(item?.date)}
                                                                                </p>
                                                                                <div className="review-rating">
                                                                                    {[
                                                                                        ...Array(
                                                                                            Math.floor(item?.rating),
                                                                                        ).keys(),
                                                                                    ].map((i) => (
                                                                                        <FontAwesomeIcon
                                                                                            key={`faStar-${i}`}
                                                                                            icon={faStar}
                                                                                        />
                                                                                    ))}

                                                                                    {item?.rating >
                                                                                        Math.floor(item?.rating) && (
                                                                                        <FontAwesomeIcon
                                                                                            icon={faStarHalfStroke}
                                                                                        />
                                                                                    )}

                                                                                    {[
                                                                                        ...Array(
                                                                                            5 - Math.ceil(item?.rating),
                                                                                        ).keys(),
                                                                                    ].map((i) => (
                                                                                        <FontAwesomeIcon
                                                                                            key={`faStarO-${i}`}
                                                                                            icon={faStarO}
                                                                                        />
                                                                                    ))}
                                                                                </div>
                                                                            </div>
                                                                            <div className="review-body">
                                                                                <b>{item?.subject}</b>
                                                                                <p>{item?.message}</p>
                                                                            </div>
                                                                        </li>
                                                                    ))}
                                                                </ul>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </>
                                            )}
                                        </Tab>
                                    </Tabs>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="section">
                    {recomments.length > 0 ? (
                        <>
                            <div className="container">
                                <p>Gợi ý sản phẩm đi kèm</p>

                                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(5, 1fr)', gap: '1rem' }}>
                                    {recomments.map((i, ind) => (
                                        <div>
                                            <Product key={ind} product={i} />
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </>
                    ) : (
                        <></>
                    )}
                </div>

                <div className="section">
                    <div className="container">
                        <ProductGroup title={'Sản phẩm cùng danh mục'} products={relatedProducts} />
                    </div>
                </div>
            </>
        </div>
    );
};

export default ProductDetails;
