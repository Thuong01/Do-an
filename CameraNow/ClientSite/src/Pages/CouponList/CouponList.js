import React, { useState, useEffect } from 'react';
import { Card, Badge, Button, Alert, Container } from 'react-bootstrap';
import axios from '../../axios';
import './CouponList.scss'; // Tạo file CSS riêng nếu cần
import { useDispatch } from 'react-redux';
import { setLoading } from '../../Redux/Slices/LoadingSlice';
import { FormatPrice } from '../../Untils/CommonUntil';

const CouponList = ({ userId }) => {
    const dispatch = useDispatch();
    const [coupons, setCoupons] = useState([]);
    const [error, setError] = useState(null);
    const [successMessage, setSuccessMessage] = useState('');

    useEffect(() => {
        dispatch(setLoading(true));
        const fetchCoupons = async () => {
            try {
                const response = await axios.get('CouponAPIs/coupons', {
                    params: {
                        Status: -1,
                        Sorting: 'name',
                        PageNumber: 1,
                        PageSize: 60,
                        Filter: '',
                    },
                });

                console.log(response.data?.result?.data);
                dispatch(setLoading(false));

                setCoupons(response.data?.result?.data);
            } catch (err) {
                setError(err.response?.data?.message || 'Lỗi khi tải mã giảm giá');
                dispatch(setLoading(false));
            }
        };

        fetchCoupons();
    }, []);

    if (error) return <Alert variant="danger">{error}</Alert>;

    return (
        <Container className="section">
            <div className="coupon-container">
                <h3 className="mb-4">Mã Giảm Giá Của Bạn</h3>

                {successMessage && (
                    <Alert variant="success" onClose={() => setSuccessMessage('')} dismissible>
                        {successMessage}
                    </Alert>
                )}

                {coupons.length === 0 ? (
                    <p>Hiện không có mã giảm giá khả dụng</p>
                ) : (
                    <div className="row">
                        {coupons.map((coupon) => (
                            <div key={coupon.id} className="col-md-6 col-lg-4 mb-4">
                                <Card className={`h-100 ${coupon.isUsed ? 'used-coupon' : ''}`}>
                                    <Card.Body>
                                        <div className="d-flex justify-content-between align-items-center mb-2">
                                            <Card.Title className="mb-0">
                                                <Badge bg="success" className="me-2">
                                                    {coupon?.type == 0
                                                        ? `${coupon.value} %`
                                                        : `${FormatPrice(coupon.value, 'VNĐ')}`}
                                                </Badge>
                                                {coupon.code}
                                            </Card.Title>
                                            {coupon.isUsed && <Badge bg="secondary">Đã sử dụng</Badge>}
                                        </div>

                                        <Card.Text>
                                            {coupon.description || coupon?.type == 0
                                                ? `Giảm ${coupon.value} % cho đơn hàng`
                                                : `Giảm ${FormatPrice(coupon.value, 'VNĐ')}  cho đơn hàng`}
                                        </Card.Text>

                                        <div className="d-flex justify-content-between align-items-center">
                                            <small className="text-muted">
                                                HSD: {new Date(coupon.endDate).toLocaleDateString()}
                                            </small>
                                        </div>
                                    </Card.Body>
                                </Card>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </Container>
    );
};

export default CouponList;
