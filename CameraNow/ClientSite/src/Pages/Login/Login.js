import { useEffect, useRef, useState } from 'react';
import styled from 'styled-components';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCamera, faClose, faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';
import { useDispatch, useSelector } from 'react-redux';
import { userLogin } from '../../Redux/Actions/authAction';
import { NavLink, useNavigate } from 'react-router-dom';
import CustomToast from '../../Untils/CustomToast';
import loginbanner from '../../assets/imgs/login_banner.jpg';
import axios from 'axios';

// Màu chủ đạo và bảng màu phối hợp
const primaryColor = '#c9f0d6';
const primaryDark = '#a8d8bb';
const primaryLight = '#e8f8ee';
const textColor = '#2a4a3a';
const secondaryColor = '#f0f7f3';

//#region  // Styled Components
const LoginContainer = styled.div`
    background: linear-gradient(to bottom, ${primaryLight}, white);
    min-height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
    padding: 20px;
    font-family: 'Montserrat', sans-serif;
`;

const LoginWrapper = styled.div`
    max-width: 1000px;
    width: 100%;
`;

const LoginMain = styled.div`
    background: white;
    border-radius: 16px;
    box-shadow: 0 10px 30px rgba(42, 74, 58, 0.1);
    overflow: hidden;
    display: flex;
    min-height: 600px;
`;

const LoginColumn = styled.div`
    flex: 1;
    padding: 50px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    position: relative;

    &:first-child {
        background: linear-gradient(rgba(42, 74, 58, 0.7), rgba(42, 74, 58, 0.7)), url('${loginbanner}');
        background-size: cover;
        background-position: center;
        color: white;
    }
`;

const LoginHeader = styled.h1`
    text-align: center;
    color: ${textColor};
    font-size: 32px;
    margin-bottom: 30px;
    font-weight: 600;
    position: relative;

    &:after {
        content: '';
        display: block;
        width: 80px;
        height: 3px;
        background: ${primaryColor};
        margin: 15px auto 0;
        border-radius: 3px;
    }
`;

const CameraIcon = styled.div`
    font-size: 40px;
    color: ${primaryColor};
    margin-bottom: 20px;
    text-align: center;
`;

const WelcomeText = styled.div`
    position: relative;
    z-index: 1;
    color: white;
    text-align: center;

    h2 {
        color: white;
        font-size: 28px;
        margin-bottom: 15px;
        font-weight: 500;
    }

    p {
        font-size: 16px;
        line-height: 1.6;
        opacity: 0.9;
    }
`;

const LoginForm = styled.form`
    display: flex;
    flex-direction: column;
    gap: 25px;
`;

const FormGroup = styled.div`
    position: relative;
`;

const FormInput = styled.input`
    width: 100%;
    padding: 15px 20px;
    border: 1px solid #e0e8e3;
    border-radius: 10px;
    font-size: 16px;
    transition: all 0.3s;
    background-color: ${secondaryColor};

    &:focus {
        border-color: ${primaryColor};
        outline: none;
        box-shadow: 0 0 0 3px rgba(201, 240, 214, 0.3);
        background-color: white;
    }
`;

const FormLabel = styled.label`
    position: absolute;
    top: 15px;
    left: 20px;
    color: ${textColor};
    opacity: 0.7;
    transition: all 0.3s;
    pointer-events: none;
    background-color: ${secondaryColor};
    padding: 0 5px;
    font-size: 14px;

    ${FormInput}:focus + &,
    ${FormInput}:not(:placeholder-shown) + & {
        top: -10px;
        left: 15px;
        font-size: 12px;
        background-color: white;
        color: ${textColor};
        opacity: 1;
    }
`;

const PasswordWrapper = styled.div`
    position: relative;
`;

const ShowPassword = styled.span`
    position: absolute;
    right: 15px;
    top: 50%;
    transform: translateY(-50%);
    cursor: pointer;
    color: #b0c8ba;

    &:hover {
        color: ${textColor};
    }
`;

const ForgotPassword = styled.p`
    text-align: right;
    color: ${textColor};
    opacity: 0.7;
    font-size: 14px;
    cursor: pointer;
    margin-top: -15px;
    transition: all 0.3s;

    &:hover {
        color: ${textColor};
        opacity: 1;
        text-decoration: underline;
    }
`;

const SubmitButton = styled.button`
    background-color: ${primaryColor};
    color: ${textColor};
    border: none;
    padding: 16px;
    border-radius: 10px;
    font-size: 16px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s;
    margin-top: 10px;
    border: 1px solid ${primaryDark};

    &:hover {
        background-color: ${primaryDark};
        transform: translateY(-2px);
        box-shadow: 0 4px 10px rgba(42, 74, 58, 0.1);
    }

    &:disabled {
        background-color: #d0e0d6;
        cursor: not-allowed;
        transform: none;
        box-shadow: none;
    }
`;

const RegisterPrompt = styled.h3`
    color: ${textColor};
    opacity: 0.8;
    font-size: 16px;
    margin-bottom: 25px;
    text-align: center;
    font-weight: 500;
`;

const RegisterButton = styled(NavLink)`
    display: block;
    text-align: center;
    background-color: white;
    color: ${textColor};
    padding: 16px;
    border-radius: 10px;
    text-decoration: none;
    font-weight: 600;
    transition: all 0.3s;
    border: 1px solid ${primaryColor};

    &:hover {
        background-color: ${primaryLight};
        transform: translateY(-2px);
        box-shadow: 0 4px 10px rgba(42, 74, 58, 0.1);
    }
`;

const DialogOverlay = styled.div`
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
`;

const Dialog = styled.div`
    background: white;
    padding: 30px;
    border-radius: 16px;
    width: 450px;
    max-width: 90%;
    position: relative;
    box-shadow: 0 10px 30px rgba(42, 74, 58, 0.2);
`;

const CloseButton = styled.button`
    position: absolute;
    top: 15px;
    right: 15px;
    background: none;
    border: none;
    cursor: pointer;
    color: ${textColor};
    opacity: 0.7;
    font-size: 20px;
    transition: all 0.3s;

    &:hover {
        opacity: 1;
        color: ${primaryColor};
    }
`;

const DialogTitle = styled.h3`
    color: ${textColor};
    font-size: 24px;
    margin-bottom: 20px;
    text-align: center;
    font-weight: 600;
`;

//#endregion

const Login = () => {
    const dispatch = useDispatch();
    const { error } = useSelector((state) => state.auth);
    const { loading } = useSelector((state) => state.loading);
    const [passwordType, setPasswordType] = useState('password');
    const [userName, setUserName] = useState('');
    const [password, setPassword] = useState('');
    const navigator = useNavigate(null);
    const [showForgotPasswordForm, setShowForgotPasswordForm] = useState(false);
    const [email, setEmail] = useState('');
    const dialogRef = useRef(null);

    const handleToggleShowPassword = () => {
        setPasswordType((prev) => (prev === 'password' ? 'text' : 'password'));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        await dispatch(userLogin({ userName, password }, navigator));
    };

    const handleForgotPassword = async (e) => {
        e.preventDefault();

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            CustomToast.success('Email không hợp lệ. Vui lòng kiểm tra lại.');
            return;
        }

        try {
            try {
                var res = axios.post(`https://localhost:7085/api/LoginAPI/forget-password?email=${email}`);
                console.log(res);
            } catch {
                CustomToast.success('Email không hợp lệ hoặc có lỗi xảy ra.');
            }

            CustomToast.success('Mật khẩu mới đã được gửi đến email của bạn.');
            setShowForgotPasswordForm(false);
        } catch (error) {
            CustomToast.success('Email không hợp lệ hoặc có lỗi xảy ra.');
        }
    };

    useEffect(() => {
        const handleClickOutside = (event) => {
            if (dialogRef.current && !dialogRef.current.contains(event.target)) {
                setShowForgotPasswordForm(false);
                setEmail('');
            }
        };

        if (showForgotPasswordForm) {
            document.addEventListener('mousedown', handleClickOutside);
        }

        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, [showForgotPasswordForm]);

    return (
        <LoginContainer>
            <LoginWrapper>
                <LoginMain>
                    <LoginColumn>
                        <WelcomeText>
                            <CameraIcon>
                                <FontAwesomeIcon icon={faCamera} />
                            </CameraIcon>
                            <h2>Chào mừng đến với CameraNow</h2>
                            <p>Đăng nhập để khám phá thế giới nhiếp ảnh với các sản phẩm chất lượng cao từ chúng tôi</p>
                        </WelcomeText>
                    </LoginColumn>

                    <LoginColumn>
                        <CameraIcon>
                            <FontAwesomeIcon icon={faCamera} />
                        </CameraIcon>
                        <LoginHeader>Đăng nhập</LoginHeader>
                        <LoginForm onSubmit={handleSubmit}>
                            <FormGroup>
                                <FormInput
                                    value={userName}
                                    type="text"
                                    id="username"
                                    placeholder=" "
                                    required
                                    onChange={(e) => setUserName(e.target.value)}
                                />
                                <FormLabel htmlFor="username">Tên đăng nhập</FormLabel>
                            </FormGroup>

                            <PasswordWrapper>
                                <FormGroup>
                                    <FormInput
                                        type={passwordType}
                                        id="password"
                                        value={password}
                                        placeholder=" "
                                        required
                                        onChange={(e) => setPassword(e.target.value)}
                                    />
                                    <FormLabel htmlFor="password">Mật khẩu</FormLabel>
                                </FormGroup>
                                <ShowPassword onClick={handleToggleShowPassword}>
                                    {passwordType === 'password' ? (
                                        <FontAwesomeIcon icon={faEye} />
                                    ) : (
                                        <FontAwesomeIcon icon={faEyeSlash} />
                                    )}
                                </ShowPassword>
                            </PasswordWrapper>

                            <ForgotPassword onClick={() => setShowForgotPasswordForm(true)}>
                                Quên mật khẩu?
                            </ForgotPassword>

                            <SubmitButton disabled={loading}>Đăng nhập</SubmitButton>
                        </LoginForm>

                        <RegisterPrompt>Bạn chưa có tài khoản? Đăng ký ngay</RegisterPrompt>
                        <RegisterButton to="/register">Đăng ký</RegisterButton>
                    </LoginColumn>
                </LoginMain>
            </LoginWrapper>

            {showForgotPasswordForm && (
                <DialogOverlay>
                    <Dialog ref={dialogRef}>
                        <DialogTitle>Quên mật khẩu</DialogTitle>
                        <LoginForm onSubmit={handleForgotPassword}>
                            <FormGroup>
                                <FormInput
                                    type="email"
                                    placeholder=" "
                                    value={email}
                                    required
                                    onChange={(e) => setEmail(e.target.value)}
                                />
                                <FormLabel>Email</FormLabel>
                            </FormGroup>
                            <SubmitButton type="submit">Gửi yêu cầu</SubmitButton>
                        </LoginForm>
                        <CloseButton onClick={() => setShowForgotPasswordForm(false)}>
                            <FontAwesomeIcon icon={faClose} />
                        </CloseButton>
                    </Dialog>
                </DialogOverlay>
            )}
        </LoginContainer>
    );
};

export default Login;
