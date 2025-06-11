import './About.scss';
import aboutUs from '../../assets/imgs/about/about-us.jpg';
import testimonialAuthor from '../../assets/imgs/about/testimonial-author.jpg';
import testimonialPic from '../../assets/imgs/about/testimonial-pic.jpg';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faQuoteRight } from '@fortawesome/free-solid-svg-icons';
import CountUp from 'react-countup';

import client1 from '../../assets/imgs/about/clients/client-1.png';
import client2 from '../../assets/imgs/about/clients/client-2.png';
import client3 from '../../assets/imgs/about/clients/client-3.png';
import client4 from '../../assets/imgs/about/clients/client-4.png';
import client5 from '../../assets/imgs/about/clients/client-5.png';
import client6 from '../../assets/imgs/about/clients/client-6.png';
import client7 from '../../assets/imgs/about/clients/client-7.png';
import client8 from '../../assets/imgs/about/clients/client-8.png';

const About = () => {
    return (
        <>
            {/* About Section Begin */}
            <section className="about spad">
                <div className="container">
                    <div className="row">
                        <div className="col-lg-12">
                            <div className="about__pic">
                                <img src={aboutUs} alt="" />
                            </div>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-lg-4 col-md-4 col-sm-6">
                            <div className="about__item">
                                <h4>Who We Are ?</h4>
                                <p>
                                    Contextual advertising programs sometimes have strict policies that need to be
                                    adhered too. Let’s take Google as an example.
                                </p>
                            </div>
                        </div>
                        <div className="col-lg-4 col-md-4 col-sm-6">
                            <div className="about__item">
                                <h4>Who We Do ?</h4>
                                <p>
                                    In this digital generation where information can be easily obtained within seconds,
                                    business cards still have retained their importance.
                                </p>
                            </div>
                        </div>
                        <div className="col-lg-4 col-md-4 col-sm-6">
                            <div className="about__item">
                                <h4>Why Choose Us</h4>
                                <p>
                                    A two or three storey house is the ideal way to maximise the piece of earth on which
                                    our home sits, but for older or infirm people.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            {/* About Section End */}

            {/* Testimonial Section Begin */}
            <section className="testimonial">
                <div className="container">
                    <div className="row">
                        <div className="col-lg-6 p-0">
                            <div className="testimonial__text">
                                <FontAwesomeIcon className="icon_quotations" icon={faQuoteRight} />

                                <p>
                                    “Going out after work? Take your butane curling iron with you to the office, heat it
                                    up, style your hair before you leave the office and you won’t have to make a trip
                                    back home.”
                                </p>
                                <div className="testimonial__author">
                                    <div className="testimonial__author__pic">
                                        <img src={testimonialAuthor} alt="" />
                                    </div>
                                    <div className="testimonial__author__text">
                                        <h5>Augusta Schultz</h5>
                                        <p>Fashion Design</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="col-lg-6 p-0">
                            <div className="testimonial__pic">
                                <img className="set-bg" src={testimonialPic} alt="" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            {/* Testimonial Section End */}

            {/* Counter Section Begin */}
            <section className="counter spad">
                <div className="container">
                    <div className="row">
                        <div className="col-lg-3 col-md-6 col-sm-6">
                            <div className="counter__item">
                                <div className="counter__item__number">
                                    <h2 className="cn_num">
                                        <CountUp end={102} start={0} duration={3} />
                                    </h2>
                                </div>
                                <span>
                                    Our <br />
                                    Clients
                                </span>
                            </div>
                        </div>
                        <div className="col-lg-3 col-md-6 col-sm-6">
                            <div className="counter__item">
                                <div className="counter__item__number">
                                    <h2 className="cn_num">
                                        <CountUp start={0} end={30} duration={3} />
                                    </h2>
                                </div>
                                <span>
                                    Total <br />
                                    Categories
                                </span>
                            </div>
                        </div>
                        <div className="col-lg-3 col-md-6 col-sm-6">
                            <div className="counter__item">
                                <div className="counter__item__number">
                                    <h2 className="cn_num">
                                        <CountUp start={0} end={102} duration={3} />
                                    </h2>
                                </div>
                                <span>
                                    In <br />
                                    Country
                                </span>
                            </div>
                        </div>
                        <div className="col-lg-3 col-md-6 col-sm-6">
                            <div className="counter__item">
                                <div className="counter__item__number">
                                    <h2 className="cn_num">
                                        <CountUp start={0} end={98} suffix=" %" duration={3} />
                                    </h2>
                                </div>
                                <span>
                                    Happy <br />
                                    Customer
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            {/* Counter Section End */}

            {/* Client Section Begin */}
            <section className="clients spad">
                <div className="container">
                    <div className="row">
                        <div className="col-lg-12">
                            <div className="section-title text-center">
                                <span>Partner</span>
                                <h2>Happy Clients</h2>
                            </div>
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-lg-3 col-md-4 col-sm-4 col-6">
                            <a href="#" className="client__item">
                                <img src={client1} alt="" />
                            </a>
                        </div>
                        <div className="col-lg-3 col-md-4 col-sm-4 col-6">
                            <a href="#" className="client__item">
                                <img src={client2} alt="" />
                            </a>
                        </div>
                        <div className="col-lg-3 col-md-4 col-sm-4 col-6">
                            <a href="#" className="client__item">
                                <img src={client3} alt="" />
                            </a>
                        </div>
                        <div className="col-lg-3 col-md-4 col-sm-4 col-6">
                            <a href="#" className="client__item">
                                <img src={client4} alt="" />
                            </a>
                        </div>
                        <div className="col-lg-3 col-md-4 col-sm-4 col-6">
                            <a href="#" className="client__item">
                                <img src={client5} alt="" />
                            </a>
                        </div>
                        <div className="col-lg-3 col-md-4 col-sm-4 col-6">
                            <a href="#" className="client__item">
                                <img src={client6} alt="" />
                            </a>
                        </div>
                        <div className="col-lg-3 col-md-4 col-sm-4 col-6">
                            <a href="#" className="client__item">
                                <img src={client7} alt="" />
                            </a>
                        </div>
                        <div className="col-lg-3 col-md-4 col-sm-4 col-6">
                            <a href="#" className="client__item">
                                <img src={client8} alt="" />
                            </a>
                        </div>
                    </div>
                </div>
            </section>
            {/* Client Section End */}
        </>
    );
};

export default About;
