import clsx from 'clsx';
import './BreadCrumb.scss';
import { NavLink } from 'react-router-dom';
import { useEffect } from 'react';

const BreadCrumb = ({ brecrumbs = [], action_item = 'action item' }) => {
    useEffect(() => {
        console.log(brecrumbs);
    }, []);

    return (
        <>
            <div id={clsx('breadcrumb')} className="section">
                <div className="container">
                    <div className="row">
                        <div className="col-md-12">
                            <ul className={clsx('breadcrumb-tree')}>
                                <li>
                                    <NavLink to={'/'}>Trang chủ</NavLink>
                                </li>
                                {brecrumbs?.map((item) => (
                                    <li>
                                        <NavLink to={`/search?keyword=${item.link}`}>{item.title}</NavLink>
                                    </li>
                                ))}
                                <li className="active">{action_item}</li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
};

export default BreadCrumb;
