import styles from './Shop.module.scss';
import clsx from 'clsx';

const ShopCard = ({ title, image, href }) => {
    return (
        <div className="col-md-4 col-xs-6">
            <div className={clsx(styles['shop'])}>
                <div className={clsx(styles['shop-img'])}>
                    <img src={image} alt="" />
                </div>
                <div className={clsx(styles['shop-body'])}>
                    <h3>{title}</h3>
                    <a href={href} className={clsx(styles['cta-btn'])}>
                        Shop now <i className="fa fa-arrow-circle-right"></i>
                    </a>
                </div>
            </div>
        </div>
    );
};

export default ShopCard;
