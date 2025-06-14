import './Footer.scss';
import logo from '../../assets/imgs/logo11.jpg';
import { NavLink } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faFacebookF, faInstagram, faTwitter, faYoutube } from '@fortawesome/free-brands-svg-icons';
import { faEnvelope, faMapMarkerAlt, faPhone } from '@fortawesome/free-solid-svg-icons';

const Footer = () => {
    return (
        <footer className="modern-footer">
            <div className="footer-container">
                <div className="footer-grid">
                    {/* Logo and About Section */}
                    <div className="footer-column logo-section">
                        <NavLink to="/" className="footer-logo">
                            <img width={180} src={logo} alt="Company Logo" />
                        </NavLink>
                        <p className="footer-about">
                            Thiết bị và ống kính chụp ảnh cao cấp dành cho cả chuyên gia và người đam mê.
                        </p>
                    </div>

                    {/* Contact Information */}
                    <div className="footer-column contact-section">
                        <h3 className="footer-heading">Thông tin liên hệ</h3>
                        <ul className="contact-list">
                            <li>
                                <FontAwesomeIcon icon={faMapMarkerAlt} className="contact-icon" />
                                <span>
                                    28 White Tower, Street Name
                                    <br />
                                    New York City, USA
                                </span>
                            </li>
                            <li>
                                <FontAwesomeIcon icon={faPhone} className="contact-icon" />
                                <span>+91 987 654 3210</span>
                            </li>
                            <li>
                                <FontAwesomeIcon icon={faEnvelope} className="contact-icon" />
                                <span>yourmain@gmail.com</span>
                            </li>
                        </ul>
                    </div>

                    {/* Quick Links */}
                    <div className="footer-column links-section">
                        <h3 className="footer-heading">Chuyển trang nhanh</h3>
                        <ul className="footer-links">
                            <li>
                                <NavLink to="/about-us">Về chúng tôi</NavLink>
                            </li>
                            <li>
                                <NavLink to="/search">Sản phẩm</NavLink>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </footer>
    );
};

export default Footer;
