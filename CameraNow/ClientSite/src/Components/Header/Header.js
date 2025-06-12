import { useState, useEffect, useRef, useContext } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import {
    faEnvelope,
    faMagnifyingGlass,
    faMapMarker,
    faPhone,
    faShoppingBag,
    faUser,
    faBars,
} from '@fortawesome/free-solid-svg-icons';
import styled from 'styled-components';
import { useDispatch, useSelector } from 'react-redux';
import Swal from 'sweetalert2';
import { userLogOut } from '../../Redux/Actions/authAction';
import { CartContext } from '../../Context/cartContext';
import Notice from '../Notices/Notice';
import CategoryMenu from './CategoryMenu';
import logo from '../../assets/imgs/logo11.jpg';

const Header = () => {
    const dispatch = useDispatch();
    const auth = useSelector((state) => state.auth);
    const user = useSelector((state) => state.user);
    const { cart, fetchUserCart } = useContext(CartContext);
    const navigate = useNavigate();
    const [showMenu, setShowMenu] = useState(false);
    const [showSearchModal, setShowSearchModal] = useState(false);
    const [searchKeyword, setSearchKeyword] = useState('');
    const menuRef = useRef();

    const handleLogOut = () => {
        Swal.fire({
            title: 'Đăng xuất?',
            text: 'Bạn có chắc muốn đăng xuất!',
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#000',
            cancelButtonColor: '#6b7280',
            cancelButtonText: 'Hủy',
            confirmButtonText: 'Đăng xuất',
            background: '#ffffff',
        }).then((result) => {
            if (result.isConfirmed) {
                dispatch(userLogOut());
                window.location.href = '/';
            }
        });
    };

    useEffect(() => {
        if (auth.login_Successed) {
            fetchUserCart();
        }
    }, [auth.login_Successed]);

    const handleSearch = () => {
        if (searchKeyword.trim() !== '') {
            navigate(`/search?keyword=${searchKeyword}&page=1`);
            setShowSearchModal(false);
            setSearchKeyword('');
        }
    };

    useEffect(() => {
        const handleClickOutside = (event) => {
            if (menuRef.current && !menuRef.current.contains(event.target)) {
                setShowMenu(false);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    return (
        <>
            <TopHeader>
                <Container style={{ display: 'flex', justifyContent: 'space-between' }}>
                    <ContactInfo>
                        <InfoItem>
                            <FontAwesomeIcon icon={faPhone} />
                            <span>+021-95-51-84</span>
                        </InfoItem>
                        <InfoItem>
                            <FontAwesomeIcon icon={faEnvelope} />
                            <span>email@email.com</span>
                        </InfoItem>
                        <InfoItem>
                            <FontAwesomeIcon icon={faMapMarker} />
                            <span>1734 Stonecoal Road</span>
                        </InfoItem>
                    </ContactInfo>

                    <UserActions>
                        {auth?.login_Successed ? (
                            <AccountDropdown>
                                <AccountButton>
                                    <FontAwesomeIcon icon={faUser} />
                                    <span>{user.userName}</span>
                                </AccountButton>
                                <DropdownMenu>
                                    <DropdownItem>
                                        <NavLink to="/account-info">Thông tin tài khoản</NavLink>
                                    </DropdownItem>
                                    <DropdownItem>
                                        <NavLink to="/orders">Đơn hàng của tôi</NavLink>
                                    </DropdownItem>
                                    <DropdownItem>
                                        <NavLink to="/coupons">Ưu đãi</NavLink>
                                    </DropdownItem>
                                    <DropdownItem>
                                        <button onClick={handleLogOut}>Đăng xuất</button>
                                    </DropdownItem>
                                </DropdownMenu>
                            </AccountDropdown>
                        ) : (
                            <>
                                <NavLink to="/login">Đăng nhập</NavLink>
                                <NavLink to="/register">Đăng ký</NavLink>
                            </>
                        )}
                    </UserActions>
                </Container>
            </TopHeader>

            <MainHeader>
                <Container>
                    <NavContainer>
                        <LogoSection ref={menuRef}>
                            <Logo to="/">
                                <img src={logo} alt="Fashion Store" />
                            </Logo>
                            <CategoryButton onClick={() => setShowMenu(!showMenu)}>
                                <FontAwesomeIcon icon={faBars} />
                                <span>Danh mục</span>
                            </CategoryButton>
                            {showMenu && <CategoryMenu />}
                        </LogoSection>

                        <NavLinks>
                            <NavItem>
                                <StyledNavLink to="/">Trang chủ</StyledNavLink>
                            </NavItem>
                            <NavItem>
                                <StyledNavLink to="/search">Sản phẩm</StyledNavLink>
                            </NavItem>
                            <NavItem>
                                <StyledNavLink to="/about-us">Về chúng tôi</StyledNavLink>
                            </NavItem>
                        </NavLinks>

                        <ActionIcons>
                            <SearchButton onClick={() => setShowSearchModal(true)}>
                                <FontAwesomeIcon icon={faMagnifyingGlass} />
                            </SearchButton>
                            <Notice
                                title={'Giỏ hàng'}
                                items={cart}
                                icon={<FontAwesomeIcon icon={faShoppingBag} />}
                                isCart={true}
                            />
                        </ActionIcons>
                    </NavContainer>
                </Container>
            </MainHeader>

            {showSearchModal && (
                <SearchModalBackdrop onClick={() => setShowSearchModal(false)}>
                    <SearchModal onClick={(e) => e.stopPropagation()}>
                        <SearchTitle>Tìm kiếm sản phẩm</SearchTitle>

                        <SearchInputContainer>
                            <SearchInput
                                type="text"
                                placeholder="Tìm kiếm máy ảnh, Lens, phụ kiện..."
                                value={searchKeyword}
                                onChange={(e) => setSearchKeyword(e.target.value)}
                                autoFocus
                                onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
                            />
                            <SearchIcon>
                                <FontAwesomeIcon icon={faMagnifyingGlass} />
                            </SearchIcon>
                        </SearchInputContainer>

                        <SearchActions>
                            <CloseButton onClick={() => setShowSearchModal(false)}>
                                <span>Đóng</span>
                            </CloseButton>
                            <SearchButton onClick={handleSearch}>
                                <FontAwesomeIcon icon={faMagnifyingGlass} />
                                <span>Tìm kiếm</span>
                            </SearchButton>
                        </SearchActions>
                    </SearchModal>
                </SearchModalBackdrop>
            )}
        </>
    );
};

// Styled Components
const Container = styled.div`
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 15px;
`;

const TopHeader = styled.header`
    background-color: #f8f9fa;
    padding: 8px 0;
    font-size: 14px;
    border-bottom: 1px solid #e9ecef;
`;

const ContactInfo = styled.ul`
    display: flex;
    gap: 20px;
    margin: 0;
    padding: 0;
    list-style: none;
`;

const InfoItem = styled.li`
    display: flex;
    align-items: center;
    gap: 5px;
    color: #6c757d;

    svg {
        color: #000;
    }
`;

const UserActions = styled.div`
    display: flex;
    gap: 15px;

    a {
        color: #6c757d;
        text-decoration: none;
        transition: color 0.2s;

        &:hover {
            color: #000;
        }
    }
`;

const AccountDropdown = styled.div`
    position: relative;
`;

const AccountButton = styled.button`
    display: flex;
    align-items: center;
    gap: 5px;
    background: none;
    border: none;
    color: #6c757d;
    cursor: pointer;
    padding: 0;

    &:hover {
        color: #000;
    }
`;

const DropdownMenu = styled.ul`
    position: absolute;
    right: 0;
    top: 100%;
    background: #fff;
    border: 1px solid #e9ecef;
    border-radius: 4px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    padding: 8px 0;
    min-width: 180px;
    z-index: 100;
    display: none;

    ${AccountDropdown}:hover & {
        display: block;
    }
`;

const DropdownItem = styled.li`
    a,
    button {
        display: block;
        padding: 8px 16px;
        color: #212529;
        text-decoration: none;
        background: none;
        border: none;
        width: 100%;
        text-align: left;
        cursor: pointer;

        &:hover {
            background-color: #f8f9fa;
        }
    }
`;

const MainHeader = styled.header`
    background-color: #fff;
    padding: 15px 0;
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
    position: sticky;
    top: 0;
    z-index: 50;
`;

const NavContainer = styled.nav`
    display: flex;
    align-items: center;
    justify-content: space-between;
`;

const LogoSection = styled.div`
    display: flex;
    align-items: center;
    gap: 20px;
    position: relative;
`;

const Logo = styled(NavLink)`
    img {
        height: 60px;
    }
`;

const CategoryButton = styled.button`
    display: flex;
    align-items: center;
    gap: 8px;
    background-color: transparent;
    color: #333;
    border: none;
    padding: 10px 15px;
    border-radius: 4px;
    cursor: pointer;
    font-weight: 500;
    transition: background-color 0.2s;

    // &:hover {
    //     background-color: #333;
    // }
`;

const NavLinks = styled.ul`
    display: flex;
    gap: 25px;
    margin: 0;
    padding: 0;
    list-style: none;

    @media (max-width: 992px) {
        display: none;
    }
`;

const NavItem = styled.li``;

const StyledNavLink = styled(NavLink)`
    color: #000;
    text-decoration: none;
    font-weight: 500;
    position: relative;
    padding: 5px 0;

    &:after {
        content: '';
        position: absolute;
        bottom: 0;
        left: 0;
        width: 0;
        height: 2px;
        background-color: #000;
        transition: width 0.3s;
    }

    &:hover:after,
    &.active:after {
        width: 100%;
    }
`;

const ActionIcons = styled.div`
    display: flex;
    align-items: center;
    gap: 20px;
`;

const PrimaryButton = styled.button`
    background-color: #000;
    color: #fff;
    border: none;
    padding: 10px 20px;
    border-radius: 4px;
    cursor: pointer;
    font-weight: 500;
    transition: background-color 0.2s;

    &:hover {
        background-color: #333;
    }
`;

const SecondaryButton = styled.button`
    background-color: #f8f9fa;
    color: #000;
    border: 1px solid #ddd;
    padding: 10px 20px;
    border-radius: 4px;
    cursor: pointer;
    font-weight: 500;
    transition: background-color 0.2s;

    &:hover {
        background-color: #e9ecef;
    }
`;

const SearchModalBackdrop = styled.div`
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: rgba(0, 0, 0, 0.7);
    z-index: 1000;
    display: flex;
    align-items: flex-start;
    justify-content: center;
    padding-top: 100px;
    backdrop-filter: blur(5px);
    animation: fadeIn 0.3s ease-out;

    @keyframes fadeIn {
        from {
            opacity: 0;
        }
        to {
            opacity: 1;
        }
    }
`;

const SearchModal = styled.div`
    background-color: #fff;
    padding: 30px;
    border-radius: 0;
    width: 100%;
    max-width: 800px;
    box-shadow: 0 15px 30px rgba(0, 0, 0, 0.2);
    transform: translateY(-20px);
    animation: slideDown 0.3s ease-out forwards;
    position: relative;

    @keyframes slideDown {
        from {
            transform: translateY(-20px);
            opacity: 0;
        }
        to {
            transform: translateY(0);
            opacity: 1;
        }
    }

    &:before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        height: 4px;
        background: linear-gradient(90deg, #ff4d4d, #f9cb28, #4dff4d, #4d4dff);
    }
`;

const SearchTitle = styled.h2`
    margin: 0 0 20px 0;
    font-size: 24px;
    font-weight: 300;
    color: #333;
    text-transform: uppercase;
    letter-spacing: 2px;
    text-align: center;
`;

const SearchInputContainer = styled.div`
    position: relative;
    margin-bottom: 25px;
`;

const SearchInput = styled.input`
    width: 100%;
    padding: 18px 20px;
    border: none;
    border-bottom: 2px solid #eee;
    font-size: 18px;
    color: #333;
    transition: all 0.3s ease;

    &:focus {
        outline: none;
        border-bottom-color: #000;
    }

    &::placeholder {
        color: #aaa;
        font-style: italic;
    }
`;

const SearchIcon = styled.div`
    position: absolute;
    right: 15px;
    top: 50%;
    transform: translateY(-50%);
    color: #999;
`;

const SearchSuggestions = styled.div`
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
    margin-bottom: 30px;
`;

const SuggestionTag = styled.button`
    background: #f5f5f5;
    border: none;
    padding: 8px 15px;
    border-radius: 20px;
    font-size: 14px;
    color: #666;
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover {
        background: #000;
        color: #fff;
    }
`;

const SearchActions = styled.div`
    display: flex;
    justify-content: space-between;
    align-items: center;
`;

const CloseButton = styled.button`
    background: none;
    border: none;
    color: #999;
    font-size: 14px;
    cursor: pointer;
    display: flex;
    align-items: center;
    gap: 5px;
    transition: color 0.2s;

    &:hover {
        color: #000;
    }
`;

const SearchButton = styled.button`
    background-color: transparent;
    color: #333;
    border: none;
    padding: 12px 30px;
    border-radius: 0;
    cursor: pointer;
    font-weight: 500;
    letter-spacing: 1px;
    text-transform: uppercase;
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    gap: 10px;

    &:hover {
        transform: translateY(-2px);
    }
`;

export default Header;
