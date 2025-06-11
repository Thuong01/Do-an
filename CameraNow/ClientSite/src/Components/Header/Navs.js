import { Link, NavLink } from 'react-router-dom';

import clsx from 'clsx';

const Navbar = () => {
    return (
        <>
            <div id={clsx('navigation')}>
                <div id={clsx('responsive-nav')} className="">
                    <nav className="navbar navbar-expand-lg">
                        <div className="container">
                            <button
                                className="navbar-toggler"
                                type="button"
                                data-bs-toggle="collapse"
                                data-bs-target="#navbarNav"
                                aria-controls="navbarNav"
                                aria-expanded="false"
                                aria-label="Toggle navigation"
                            >
                                <span className="navbar-toggler-icon" />
                            </button>
                            <div className="collapse navbar-collapse" id="navbarNav">
                                <ul className={clsx('main-nav', 'navbar-nav')}>
                                    <li className="nav-item">
                                        <NavLink
                                            className="nav-link"
                                            aria-current="page"
                                            exact="true"
                                            to="/"
                                            active="true"
                                        >
                                            Home
                                        </NavLink>
                                    </li>

                                    <li className="nav-item">
                                        <NavLink className="nav-link" to="/about-us">
                                            About us
                                        </NavLink>
                                    </li>

                                    <li className="nav-item">
                                        <NavLink className="nav-link" to="/blogs">
                                            Blogs
                                        </NavLink>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </nav>
                </div>
            </div>
        </>
    );
};

export default Navbar;
