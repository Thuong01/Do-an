import clsx from 'clsx';
import styles from './HotDeal.module.scss';

const HotDeal = () => {
    return (
        <div>
            <div id={clsx(styles['hot-deal'])} className={clsx(styles['section'])}>
                <div className="container">
                    <div className="row">
                        <div className="col-md-12">
                            <div className={clsx(styles['hot-deal'])}>
                                <ul className={clsx(styles['hot-deal-countdown'])}>
                                    <li>
                                        <div>
                                            <h3>02</h3>
                                            <span>Days</span>
                                        </div>
                                    </li>
                                    <li>
                                        <div>
                                            <h3>10</h3>
                                            <span>Hours</span>
                                        </div>
                                    </li>
                                    <li>
                                        <div>
                                            <h3>34</h3>
                                            <span>Mins</span>
                                        </div>
                                    </li>
                                    <li>
                                        <div>
                                            <h3>60</h3>
                                            <span>Secs</span>
                                        </div>
                                    </li>
                                </ul>
                                <h2 className="text-uppercase">Ưu đãi hót trong hôm nay</h2>
                                <p>Sản phẩm mới giảm 50%</p>
                                <a className="primary-btn cta-btn" href="#">
                                    Mua sắm ngay
                                </a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default HotDeal;
