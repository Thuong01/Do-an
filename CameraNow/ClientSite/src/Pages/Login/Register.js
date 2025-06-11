import { useState } from 'react';
import styled from 'styled-components';
import { faEye, faEyeSlash, faCamera } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useDispatch, useSelector } from 'react-redux';
import { NavLink, useNavigate } from 'react-router-dom';
import { Formik, Form, Field, ErrorMessage } from 'formik';
import * as Yup from 'yup';
import { RegisterService } from '../../Services/UserService';
import { setLoading } from '../../Redux/Slices/LoadingSlice';
import CustomToast from '../../Untils/CustomToast';

// Styled Components
const RegisterContainer = styled.div`
    background: linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.7)),
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

const RegisterWrapper = styled.div`
    max-width: 900px;
    width: 100%;
`;

const RegisterCard = styled.div`
    background: white;
    border-radius: 16px;
    box-shadow: 0 15px 40px rgba(0, 0, 0, 0.2);
    padding: 50px;
    position: relative;
    overflow: hidden;

    @media (max-width: 768px) {
        padding: 30px;
    }
`;

const CameraIcon = styled.div`
    font-size: 40px;
    color: #3498db;
    text-align: center;
    margin-bottom: 20px;
`;

const RegisterHeader = styled.h1`
    text-align: center;
    color: #2c3e50;
    font-size: 32px;
    margin-bottom: 30px;
    font-weight: 700;
    position: relative;

    &:after {
        content: '';
        display: block;
        width: 80px;
        height: 4px;
        background: #3498db;
        margin: 15px auto 0;
        border-radius: 2px;
    }
`;

const FormRow = styled.div`
    display: flex;
    gap: 25px;
    margin-bottom: 25px;

    @media (max-width: 768px) {
        flex-direction: column;
        gap: 20px;
    }
`;

const FormGroup = styled.div`
    flex: 1;
    position: relative;
`;

const FormLabel = styled.label`
    display: block;
    margin-bottom: 10px;
    font-size: 14px;
    color: #2c3e50;
    font-weight: 500;

    span {
        color: #e74c3c;
    }
`;

const StyledField = styled(Field)`
    width: 100%;
    padding: 14px 16px;
    border: 1px solid #e0e0e0;
    border-radius: 8px;
    font-size: 15px;
    transition: all 0.3s;
    background-color: #f8f9fa;

    &:focus {
        border-color: #3498db;
        outline: none;
        box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.2);
        background-color: white;
    }

    &[type='password'] {
        padding-right: 45px;
    }
`;

const PasswordToggle = styled.button`
    position: absolute;
    right: 15px;
    top: 50%;
    background: none;
    border: none;
    color: #95a5a6;
    cursor: pointer;
    font-size: 16px;
    transition: all 0.3s;
    transform: translateY(-50%);

    &:hover {
        color: #3498db;
    }
`;

const ErrorText = styled(ErrorMessage)`
    color: #e74c3c;
    font-size: 12px;
    margin-top: 6px;
    display: block;
`;

const SubmitButton = styled.button`
    width: 100%;
    background-color: #3498db;
    color: white;
    border: none;
    padding: 16px;
    border-radius: 8px;
    font-size: 16px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s;
    margin-top: 20px;
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

const LoginPrompt = styled.div`
    text-align: center;
    margin-top: 35px;
    padding-top: 25px;
    border-top: 1px solid #eee;
`;

const LoginText = styled.h3`
    color: #7f8c8d;
    font-size: 16px;
    margin-bottom: 20px;
    font-weight: 500;
`;

const LoginLink = styled(NavLink)`
    display: inline-block;
    padding: 14px 30px;
    background-color: #2c3e50;
    color: white;
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

const OrDivider = styled.div`
    position: relative;
    margin: 25px 0;
    text-align: center;
    color: #95a5a6;

    &:before {
        content: '';
        position: absolute;
        top: 50%;
        left: 0;
        right: 0;
        height: 1px;
        background: #eee;
        z-index: -1;
    }

    span {
        background: white;
        padding: 0 15px;
    }
`;

const Register = () => {
    const initialValues = {
        username: '',
        fullname: '',
        email: '',
        password: '',
        confirmPassword: '',
        phonenumber: '',
        address: '',
    };

    const validationSchema = Yup.object({
        username: Yup.string().required('Vui lòng nhập tên đăng nhập').min(4, 'Tên đăng nhập ít nhất 4 ký tự'),
        fullname: Yup.string().required('Vui lòng nhập họ và tên'),
        email: Yup.string().email('Email không hợp lệ').required('Vui lòng nhập email'),
        password: Yup.string().required('Vui lòng nhập mật khẩu').min(6, 'Mật khẩu ít nhất 6 ký tự'),
        confirmPassword: Yup.string()
            .oneOf([Yup.ref('password'), null], 'Mật khẩu xác nhận không khớp')
            .required('Vui lòng xác nhận mật khẩu'),
        phonenumber: Yup.string().matches(
            /^(0|\+84)[0-9]{9}$/,
            'Số điện thoại không hợp lệ (bắt đầu bằng 0 hoặc +84, và đủ 10 số)',
        ),
        address: Yup.string().required('Vui lòng nhập địa chỉ'),
    });

    const dispatch = useDispatch();
    const { loading } = useSelector((state) => state.loading);
    const navigator = useNavigate(null);
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);

    const handleSubmit = async (values) => {
        dispatch(setLoading(true));
        try {
            const res = await RegisterService({
                userName: values.username,
                fullName: values.fullname,
                email: values.email,
                password: values.password,
                address: values.address,
                phoneNumber: values.phonenumber,
                confirmPassword: values.confirmPassword,
            });

            if (res?.data?.success) {
                CustomToast.success('Tạo tài khoản thành công!');
                navigator('/login');
            } else {
                CustomToast.error(res?.data?.message || 'Đăng ký thất bại');
            }
        } catch (error) {
            if (!Array.isArray(error?.data?.result?.message)) {
                CustomToast.error(error?.data?.result?.message);
            } else {
                error?.data?.result?.message?.forEach((e) => {
                    CustomToast.error(e?.description);
                });
            }
        } finally {
            dispatch(setLoading(false));
        }
    };

    return (
        <RegisterContainer>
            <RegisterWrapper>
                <RegisterCard>
                    <CameraIcon>
                        <FontAwesomeIcon icon={faCamera} />
                    </CameraIcon>
                    <RegisterHeader>Đăng ký tài khoản</RegisterHeader>

                    <Formik initialValues={initialValues} validationSchema={validationSchema} onSubmit={handleSubmit}>
                        {({ errors, touched }) => (
                            <Form>
                                <FormRow>
                                    <FormGroup>
                                        <FormLabel>
                                            Tên đăng nhập <span>*</span>
                                        </FormLabel>
                                        <StyledField type="text" name="username" placeholder="Nhập tên đăng nhập" />
                                        <ErrorText name="username" component="div" />
                                    </FormGroup>

                                    <FormGroup>
                                        <FormLabel>
                                            Họ và tên <span>*</span>
                                        </FormLabel>
                                        <StyledField type="text" name="fullname" placeholder="Nhập họ và tên" />
                                        <ErrorText name="fullname" component="div" />
                                    </FormGroup>
                                </FormRow>

                                <FormRow>
                                    <FormGroup>
                                        <FormLabel>
                                            Email <span>*</span>
                                        </FormLabel>
                                        <StyledField type="email" name="email" placeholder="Nhập email" />
                                        <ErrorText name="email" component="div" />
                                    </FormGroup>

                                    <FormGroup>
                                        <FormLabel>Số điện thoại</FormLabel>
                                        <StyledField type="text" name="phonenumber" placeholder="Nhập số điện thoại" />
                                        <ErrorText name="phonenumber" component="div" />
                                    </FormGroup>
                                </FormRow>

                                <FormRow>
                                    <FormGroup>
                                        <FormLabel>
                                            Mật khẩu <span>*</span>
                                        </FormLabel>
                                        <div style={{ position: 'relative' }}>
                                            <StyledField
                                                type={showPassword ? 'text' : 'password'}
                                                name="password"
                                                placeholder="Nhập mật khẩu"
                                            />
                                            <PasswordToggle
                                                type="button"
                                                onClick={() => setShowPassword(!showPassword)}
                                            >
                                                <FontAwesomeIcon icon={showPassword ? faEyeSlash : faEye} />
                                            </PasswordToggle>
                                        </div>
                                        <ErrorText name="password" component="div" />
                                    </FormGroup>
                                    <FormGroup>
                                        <FormLabel>
                                            Địa chỉ <span>*</span>
                                        </FormLabel>
                                        <div style={{ position: 'relative' }}>
                                            <StyledField type={'text'} name="address" placeholder="Nhập địa chỉ" />
                                        </div>
                                        <ErrorText name="address" component="div" />
                                    </FormGroup>
                                </FormRow>

                                <FormRow>
                                    <FormGroup>
                                        <FormLabel>
                                            Xác nhận mật khẩu <span>*</span>
                                        </FormLabel>
                                        <div style={{ position: 'relative' }}>
                                            <StyledField
                                                type={showConfirmPassword ? 'text' : 'password'}
                                                name="confirmPassword"
                                                placeholder="Nhập lại mật khẩu"
                                            />
                                            <PasswordToggle
                                                type="button"
                                                onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                                            >
                                                <FontAwesomeIcon icon={showConfirmPassword ? faEyeSlash : faEye} />
                                            </PasswordToggle>
                                        </div>
                                        <ErrorText name="confirmPassword" component="div" />
                                    </FormGroup>
                                    <FormGroup></FormGroup>
                                </FormRow>

                                <SubmitButton type="submit" disabled={loading}>
                                    {loading ? 'Đang xử lý...' : 'Đăng ký ngay'}
                                </SubmitButton>
                            </Form>
                        )}
                    </Formik>

                    <LoginPrompt>
                        <LoginText>Đã có tài khoản? Đăng nhập ngay!</LoginText>
                        <LoginLink to="/login">Đăng nhập</LoginLink>
                    </LoginPrompt>
                </RegisterCard>
            </RegisterWrapper>
        </RegisterContainer>
    );
};

export default Register;
