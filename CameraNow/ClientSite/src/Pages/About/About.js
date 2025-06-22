import './About.scss';
import aboutUs from '../../assets/imgs/sony-alpha-7-iv-4286.jpeg';
import testimonialPic from '../../assets/imgs/about1.jpg';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faQuoteRight } from '@fortawesome/free-solid-svg-icons';
import CountUp from 'react-countup';

const About = () => {
    return (
        <>
            {/* About Section Begin */}
            <section className="about spad">
                <div className="container">
                    <div className="row">
                        <div className="col-lg-12">
                            <div className="about__pic">
                                <img style={{ width: '100%', border: '1px solid' }} src={aboutUs} alt="" />
                            </div>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-lg-4 col-md-4 col-sm-6">
                            <div className="about__item">
                                <h4>Về Chúng Tôi</h4>
                                <p>
                                    Chúng tôi là cửa hàng chuyên cung cấp máy ảnh và thiết bị ghi hình chính hãng, phục
                                    vụ nhu cầu từ người mới bắt đầu đến nhiếp ảnh gia chuyên nghiệp. Với đội ngũ giàu
                                    kinh nghiệm và đam mê, chúng tôi cam kết mang đến sản phẩm chất lượng cùng dịch vụ
                                    đáng tin cậy.
                                </p>
                            </div>
                        </div>
                        <div className="col-lg-4 col-md-4 col-sm-6">
                            <div className="about__item">
                                <h4>Chúng Tôi Làm Gì</h4>
                                <p>
                                    Chúng tôi phân phối các dòng máy ảnh cùng các phụ kiện như ống kính, tripod, thẻ
                                    nhớ, đèn flash,... Ngoài ra, chúng tôi còn hỗ trợ tư vấn lựa chọn sản phẩm, chia sẻ
                                    kiến thức nhiếp ảnh và cung cấp dịch vụ hậu mãi chu đáo.
                                </p>
                            </div>
                        </div>
                        <div className="col-lg-4 col-md-4 col-sm-6">
                            <div className="about__item">
                                <h4>Vì Sao Chọn Chúng Tôi</h4>
                                <p>
                                    Sản phẩm chính hãng, đầy đủ bảo hành, giá cả cạnh tranh, ưu đãi hấp dẫn, giao hàng
                                    toàn quốc nhanh chóng, hỗ trợ kỹ thuật và chăm sóc khách hàng tận tâm
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
                                    “Đi chụp ảnh sau giờ làm? Mang theo chiếc máy ảnh nhỏ gọn bên mình, sẵn sàng bắt
                                    trọn mọi khoảnh khắc tuyệt đẹp ngay sau khi tan ca – không cần quay về nhà chuẩn
                                    bị!”
                                </p>
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
        </>
    );
};

export default About;
