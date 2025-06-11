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
                            Premium photography equipment and lenses for professionals and enthusiasts alike.
                        </p>
                        <div className="social-links">
                            <a href="#" aria-label="Facebook">
                                <FontAwesomeIcon icon={faFacebookF} />
                            </a>
                            <a href="#" aria-label="Instagram">
                                <FontAwesomeIcon icon={faInstagram} />
                            </a>
                            <a href="#" aria-label="Twitter">
                                <FontAwesomeIcon icon={faTwitter} />
                            </a>
                            <a href="#" aria-label="YouTube">
                                <FontAwesomeIcon icon={faYoutube} />
                            </a>
                        </div>
                    </div>

                    {/* Contact Information */}
                    <div className="footer-column contact-section">
                        <h3 className="footer-heading">Contact Us</h3>
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
                        <h3 className="footer-heading">Quick Links</h3>
                        <ul className="footer-links">
                            <li>
                                <NavLink to="/about">About Us</NavLink>
                            </li>
                            <li>
                                <NavLink to="/products">Products</NavLink>
                            </li>
                            <li>
                                <NavLink to="/blog">Blog</NavLink>
                            </li>
                            <li>
                                <NavLink to="/contact">Contact</NavLink>
                            </li>
                            <li>
                                <NavLink to="/faq">FAQ</NavLink>
                            </li>
                        </ul>
                    </div>

                    {/* Newsletter */}
                    <div className="footer-column newsletter-section">
                        <h3 className="footer-heading">Newsletter</h3>
                        <p>Subscribe for updates and promotions</p>
                        <form className="newsletter-form">
                            <input type="email" placeholder="Your email address" required />
                            <button type="submit">Subscribe</button>
                        </form>
                    </div>
                </div>
            </div>
        </footer>
    );
};

export default Footer;
