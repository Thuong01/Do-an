// src/Components/FeedbackModal.jsx
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useState } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import { faStar as faStarSolid } from '@fortawesome/free-solid-svg-icons';
import { faStar as faStarRegular } from '@fortawesome/free-regular-svg-icons';
import clsx from 'clsx';

const FeedbackModal = ({ show, onClose, onSubmit, productId }) => {
    const [feedback, setFeedbacks] = useState({
        product_ID: productId,
        user_ID: '',
        order_ID: '',
        subject: '',
        message: '',
        date: new Date().toISOString(),
        rating: 1,
        images: [],
    });

    const handleSubmit = () => {
        onSubmit(feedback);
    };

    return (
        <Modal show={show} onHide={onClose} centered>
            <Modal.Header closeButton>
                <Modal.Title>Đánh giá đơn hàng</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form>
                    <Form.Group>
                        <Form.Label>Chọn số sao</Form.Label>
                        <div>
                            {[...Array(5)].map((_, i) => (
                                <FontAwesomeIcon
                                    key={i}
                                    icon={i < feedback.rating ? faStarSolid : faStarRegular}
                                    className="me-2"
                                    style={{ cursor: 'pointer', color: i < feedback.rating ? '#ffc107' : '#ccc' }}
                                    onClick={() => setFeedbacks((prev) => ({ ...prev, rating: i + 1 }))}
                                />
                            ))}
                        </div>
                    </Form.Group>
                    <Form.Group className="mt-3">
                        <Form.Label>Bình luận</Form.Label>
                        <Form.Control
                            as="textarea"
                            rows={3}
                            value={feedback.message}
                            onChange={(e) => setFeedbacks({ ...feedback, message: e.target.value })}
                            placeholder="Nội dung đánh giá"
                        />
                    </Form.Group>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={onClose}>
                    Hủy
                </Button>
                <Button variant="primary" onClick={handleSubmit}>
                    Gửi đánh giá
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default FeedbackModal;
