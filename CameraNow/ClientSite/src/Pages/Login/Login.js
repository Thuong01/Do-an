import { useEffect, useRef, useState } from 'react';
import styled from 'styled-components';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCamera, faClose, faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';
import { useDispatch, useSelector } from 'react-redux';
import { userLogin } from '../../Redux/Actions/authAction';
import { NavLink, useNavigate } from 'react-router-dom';
import CustomToast from '../../Untils/CustomToast';

// Styled Components
const LoginContainer = styled.div`
    background: linear-gradient(rgba(0, 0, 0, 0.5)),
        url('https://images.unsplash.com/photo-1516035069371-29a1b244cc32?ixlib=rb-1.2.1&auto=format&fit=crop&w=1350&q=80');
    background-size: cover;
    background-position: center;
    min-height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
    padding: 20px;
    font-family: 'Inter', sans-serif;
`;

const LoginWrapper = styled.div`
    max-width: 1000px;
    width: 100%;
`;

const LoginMain = styled.div`
    background: white;
    border-radius: 16px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
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
        background-color: #f8f9fa;
        background-image: url('https://images.unsplash.com/photo-1510127034890-ba27508e9f1c?ixlib=rb-1.2.1&auto=format&fit=crop&w=1350&q=80');
        background-size: cover;
        background-position: center;
        color: white;

        &::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(0, 0, 0, 0.6);
        }
    }
`;

const LoginHeader = styled.h1`
    text-align: center;
    color: #2c3e50;
    font-size: 32px;
    margin-bottom: 30px;
    font-weight: 700;
    position: relative;
`;

const CameraIcon = styled.div`
    font-size: 40px;
    color: #3498db;
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
    }

    p {
        font-size: 16px;
        line-height: 1.6;
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
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    font-size: 16px;
    transition: all 0.3s;
    background-color: #f8f9fa;

    &:focus {
        border-color: #3498db;
        outline: none;
        box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.2);
        background-color: white;
    }
`;

const FormLabel = styled.label`
    position: absolute;
    top: 15px;
    left: 20px;
    color: #7f8c8d;
    transition: all 0.3s;
    pointer-events: none;
    background-color: #f8f9fa;
    padding: 0 5px;

    ${FormInput}:focus + &,
    ${FormInput}:not(:placeholder-shown) + & {
        top: -10px;
        left: 15px;
        font-size: 12px;
        background-color: white;
        color: #3498db;
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
    color: #95a5a6;

    &:hover {
        color: #3498db;
    }
`;

const ForgotPassword = styled.p`
    text-align: right;
    color: #7f8c8d;
    font-size: 14px;
    cursor: pointer;
    margin-top: -15px;

    &:hover {
        color: #3498db;
        text-decoration: underline;
    }
`;

const SubmitButton = styled.button`
    background-color: #3498db;
    color: white;
    border: none;
    padding: 16px;
    border-radius: 8px;
    font-size: 16px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s;
    margin-top: 10px;
    box-shadow: 0 4px 6px rgba(52, 152, 219, 0.2);

    &:hover {
        background-color: #2980b9;
        transform: translateY(-2px);
        box-shadow: 0 6px 8px rgba(52, 152, 219, 0.3);
    }

    &:disabled {
        background-color: #bdc3c7;
        cursor: not-allowed;
        transform: none;
        box-shadow: none;
    }
`;

const RegisterPrompt = styled.h3`
    color: #2c3e50;
    font-size: 18px;
    margin-bottom: 25px;
    text-align: center;
    font-weight: 500;
`;

const RegisterButton = styled(NavLink)`
    display: block;
    text-align: center;
    background-color: #2c3e50;
    color: white;
    padding: 16px;
    border-radius: 8px;
    text-decoration: none;
    font-weight: 600;
    transition: all 0.3s;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);

    &:hover {
        background-color: #1a252f;
        transform: translateY(-2px);
        box-shadow: 0 6px 8px rgba(0, 0, 0, 0.15);
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
    border-radius: 12px;
    width: 450px;
    max-width: 90%;
    position: relative;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
`;

const CloseButton = styled.button`
    position: absolute;
    top: 15px;
    right: 15px;
    background: none;
    border: none;
    cursor: pointer;
    color: #7f8c8d;
    font-size: 20px;

    &:hover {
        color: #e74c3c;
    }
`;

const DialogTitle = styled.h3`
    color: #2c3e50;
    font-size: 24px;
    margin-bottom: 20px;
    text-align: center;
`;

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
