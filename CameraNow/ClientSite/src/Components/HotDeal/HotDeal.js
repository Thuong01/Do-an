import { useState, useEffect } from 'react';
import clsx from 'clsx';
import styles from './HotDeal.module.scss';
import { NavLink } from 'react-router-dom';

const HotDeal = () => {
    // Set your target date for the deal (e.g., 24 hours from now)
    const [timeLeft, setTimeLeft] = useState({
        days: 0,
        hours: 0,
        minutes: 0,
        seconds: 0,
    });

    useEffect(() => {
        // Set the target date (e.g., 24 hours from now)
        const targetDate = new Date();
        targetDate.setDate(targetDate.getDate() + 2); // 2 days from now for example

        const calculateTimeLeft = () => {
            const now = new Date();
            const difference = targetDate - now;

            if (difference > 0) {
                const days = Math.floor(difference / (1000 * 60 * 60 * 24));
                const hours = Math.floor((difference / (1000 * 60 * 60)) % 24);
                const minutes = Math.floor((difference / 1000 / 60) % 60);
                const seconds = Math.floor((difference / 1000) % 60);

                setTimeLeft({
                    days: days.toString().padStart(2, '0'),
                    hours: hours.toString().padStart(2, '0'),
                    minutes: minutes.toString().padStart(2, '0'),
                    seconds: seconds.toString().padStart(2, '0'),
                });
            } else {
                // Timer has ended
                setTimeLeft({
                    days: '00',
                    hours: '00',
                    minutes: '00',
                    seconds: '00',
                });
            }
        };

        // Initial calculation
        calculateTimeLeft();

        // Update every second
        const timer = setInterval(calculateTimeLeft, 1000);

        // Clean up on unmount
        return () => clearInterval(timer);
    }, []);

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
                                            <h3>{timeLeft.days}</h3>
                                            <span>Days</span>
                                        </div>
                                    </li>
                                    <li>
                                        <div>
                                            <h3>{timeLeft.hours}</h3>
                                            <span>Hours</span>
                                        </div>
                                    </li>
                                    <li>
                                        <div>
                                            <h3>{timeLeft.minutes}</h3>
                                            <span>Mins</span>
                                        </div>
                                    </li>
                                    <li>
                                        <div>
                                            <h3>{timeLeft.seconds}</h3>
                                            <span>Secs</span>
                                        </div>
                                    </li>
                                </ul>
                                <h2 className="text-uppercase">Ưu đãi hót trong hôm nay</h2>
                                <NavLink
                                    to={'/search'}
                                    className={clsx(styles['primary-btn-ctum'], 'primary-btn cta-btn')}
                                    href="#"
                                >
                                    Mua sắm ngay
                                </NavLink>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default HotDeal;
