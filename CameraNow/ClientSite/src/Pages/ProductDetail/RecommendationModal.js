import React, { useState } from 'react';
import { Modal, Button, Row, Col, Spinner, Badge } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheckCircle, faInfoCircle, faShoppingCart, faFire } from '@fortawesome/free-solid-svg-icons';
import { NavLink, useNavigate } from 'react-router-dom';
import clsx from 'clsx';
import default_product_image from '../../assets/imgs/default_product_image.jpg';
import { FormatPrice } from '../../Untils/CommonUntil';
import './RecommendationModal.scss';

const RecommendationModal = ({ show, onHide, recommendations = [], handleAddToCart, currentProduct, navigate }) => {
    const [loadingProducts, setLoadingProducts] = useState([]);

    const handleAddRecommendation = async (productId) => {
        try {
            setLoadingProducts((prev) => [...prev, productId]);
            await handleAddToCart(productId);
        } finally {
            setLoadingProducts((prev) => prev.filter((id) => id !== productId));
        }
    };

    const handleGoToCart = () => {
        onHide();
        navigate('/shopping-cart');
    };

    return (
        <Modal
            show={show}
            onHide={onHide}
            size="lg"
            centered
            backdrop="static"
            className="recommendation-modal"
            animation={true}
        >
            <Modal.Header closeButton className="border-bottom-0 pb-0">
                <Modal.Title className="w-100">
                    <div className="d-flex align-items-center text-success mb-3">
                        <FontAwesomeIcon icon={faCheckCircle} className="me-2 fs-4" />
                        <h5 className="mb-0 fw-semibold">
                            Đã thêm{' '}
                            <Badge bg="light" text="dark" className="ms-1 fs-6">
                                {currentProduct?.name}
                            </Badge>{' '}
                            vào giỏ hàng
                        </h5>
                    </div>
                </Modal.Title>
            </Modal.Header>

            <Modal.Body className="pt-0">
                <h6 className="text-danger fw-bold mb-3 d-flex align-items-center">
                    <FontAwesomeIcon icon={faFire} className="me-2" />
                    Mọi người cũng thêm vào giỏ hàng
                </h6>

                {recommendations.length > 0 ? (
                    <Row className="g-3">
                        {recommendations.map((product) => (
                            <Col key={product.id} xs={6} md={4} lg={3}>
                                <div className="border rounded p-2 h-100 d-flex flex-column product-card">
                                    <NavLink
                                        to={`/product-detail/${product?.id}`}
                                        onClick={onHide}
                                        className="product-img-link"
                                    >
                                        <div className="product-img-container">
                                            <img
                                                src={product?.image || default_product_image}
                                                alt={product?.name}
                                                className="product-img"
                                                onError={(e) => {
                                                    e.target.src = default_product_image;
                                                }}
                                            />
                                        </div>
                                    </NavLink>

                                    <div className="product-body mt-2">
                                        <h3 className="product-name mb-1">
                                            <NavLink
                                                to={`/product-detail/${product?.id}`}
                                                onClick={onHide}
                                                className="text-decoration-none text-dark"
                                            >
                                                {product?.name}
                                            </NavLink>
                                        </h3>
                                        <h4 className="product-price mb-2">
                                            {product?.promotion_Price > 0 ? (
                                                <>
                                                    <span className="text-danger fw-bold">
                                                        {FormatPrice(product?.promotion_Price, 'VNĐ')}
                                                    </span>
                                                    {product?.price > product?.promotion_Price && (
                                                        <del className="text-muted ms-2 fs-6">
                                                            {FormatPrice(product?.price, 'VNĐ')}
                                                        </del>
                                                    )}
                                                </>
                                            ) : (
                                                <span className="fw-bold">{FormatPrice(product?.price, 'VNĐ')}</span>
                                            )}
                                        </h4>
                                    </div>

                                    <Button
                                        variant="outline-danger"
                                        size="sm"
                                        className="mt-auto add-to-cart-btn"
                                        onClick={() => handleAddRecommendation(product.id)}
                                        disabled={loadingProducts.includes(product.id)}
                                    >
                                        {loadingProducts.includes(product.id) ? (
                                            <>
                                                <Spinner
                                                    as="span"
                                                    animation="border"
                                                    size="sm"
                                                    className="me-2"
                                                    role="status"
                                                    aria-hidden="true"
                                                />
                                                <span>Đang thêm...</span>
                                            </>
                                        ) : (
                                            <>
                                                <FontAwesomeIcon icon={faShoppingCart} className="me-2" />
                                                <span>Thêm vào giỏ</span>
                                            </>
                                        )}
                                    </Button>
                                </div>
                            </Col>
                        ))}
                    </Row>
                ) : (
                    <div className="text-center py-4 text-muted no-recommendations">
                        <FontAwesomeIcon icon={faInfoCircle} className="fs-1 mb-2 text-muted" />
                        <p className="mb-0">Không có sản phẩm gợi ý nào</p>
                    </div>
                )}
            </Modal.Body>

            <Modal.Footer className="border-top-0 pt-0">
                <Button variant="outline-secondary" onClick={onHide} className="flex-grow-1">
                    Tiếp tục mua sắm
                </Button>
                <Button variant="danger" onClick={handleGoToCart} className="flex-grow-1">
                    Xem giỏ hàng
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default RecommendationModal;
