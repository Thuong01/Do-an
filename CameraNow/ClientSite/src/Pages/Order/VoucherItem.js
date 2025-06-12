import clsx from 'clsx';
import styles from './VoucherItem.module.scss';
import ico_promotion from '../../assets/icons/ico_promotion.svg';
import { FormatLongDate, FormatLongDateTime } from '../../Untils/CommonUntil';

const VoucherItem = ({
    voucherId,
    name = '',
    desc = '',
    code = '',
    status = '',
    endDate = '',
    startDate = '',
    handleSelectVoucher,
}) => {
    return (
        <>
            <div className={clsx(styles['promo-list-item'])}>
                <svg
                    xmlns="http://www.w3.org/2000/svg"
                    xmlnsXlink="http://www.w3.org/1999/xlink"
                    className={clsx(styles['coupon_bg'])}
                    viewBox="7.5 -2 479.5 148"
                    style={{ filter: 'drop-shadow(rgba(0, 0, 0, 0.15) 0px 1px 3px)' }}
                >
                    <g fill="none" fillRule="evenodd">
                        <g>
                            <g>
                                <g>
                                    <g transform="translate(-544 -3050) translate(80 2072) translate(0 930) translate(464 48)">
                                        <path
                                            id="Frame_voucher_Web"
                                            d="M 110 144 h -98 a 12 12 0 0 1 -12 -12 v -122 a 12 12 0 0 1 12 -12 H 110 a 12.02 12 0 0 0 24 0 H 481.5 a 12 12 0 0 1 12 12 V 132 a 12 12 0 0 1 -12 12 H 134 v 0 a 12 12 0 0 0 -24 0 v 0 Z"
                                            transform="translate(0.5 0.5)"
                                            fill="#fff"
                                            stroke="rgba(0,0,0,0)"
                                            strokeMiterlimit={10}
                                            strokeWidth={1}
                                        />
                                    </g>
                                </g>
                            </g>
                        </g>
                    </g>
                </svg>

                <div className={clsx(styles['promo-item-image'])}>
                    <img src={ico_promotion} alt="ico promotion" />
                </div>

                <div className={clsx(styles['promo-item'])}>
                    <div>
                        {/* First of type */}
                        <div className={clsx(styles['promo-list-item-content'])}>
                            <div>
                                <div className={clsx(styles['promo-list-item-content-title'])}>{name}</div>
                            </div>

                            <div className={clsx(styles['promo-list-item-content-description'])}>{desc}</div>
                        </div>
                    </div>

                    <div>
                        {/* nth-child-2 */}
                        <div className={clsx('voucher-status')}>
                            <span>{FormatLongDateTime(endDate)}</span>
                        </div>

                        {status !== 'Hết hạn sử dụng' ? (
                            <button
                                type="button"
                                title="Áp dụng"
                                coupon="FHS10KT09S1"
                                apply={1}
                                className={clsx(styles['btn-view-promo-coupon'])}
                                onClick={() => handleSelectVoucher(code)}
                            >
                                <span>Áp dụng</span>
                            </button>
                        ) : (
                            <> </>
                        )}
                    </div>
                </div>
            </div>
        </>
    );
};

export default VoucherItem;
