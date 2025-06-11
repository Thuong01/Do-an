import { faCalendar, faEdit, faEnvelope, faEye, faEyeSlash, faSave, faX } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import styled from 'styled-components';
import { EditProfile } from '../../Services/UserService';
import CustomToast from '../../Untils/CustomToast';

//#region Styles
const AccountContainer = styled.div`
    margin: 0rem auto;
    padding: 2rem;
    background: white;
    border-radius: 16px;
    box-shadow: 0 8px 30px rgba(0, 0, 0, 0.08);
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
`;

const Header = styled.div`
    display: flex;
    align-items: center;
    margin-bottom: 2rem;
`;

const Avatar = styled.div`
    width: 80px;
    height: 80px;
    border-radius: 50%;
    background: linear-gradient(135deg, #6e8efb, #a777e3);
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-size: 2rem;
    font-weight: bold;
    margin-right: 1.5rem;
`;

const UserfullName = styled.h2`
    margin: 0;
    color: #333;
    font-size: 1.8rem;
`;

const UserStatus = styled.span`
    display: inline-block;
    background: #e3f7e8;
    color: #2ecc71;
    padding: 0.3rem 0.8rem;
    border-radius: 20px;
    font-size: 0.8rem;
    margin-top: 0.5rem;
    font-weight: 600;
`;

const InfoGrid = styled.div`
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1.5rem;
`;

const InfoCard = styled.div`
    background: #f9f9f9;
    padding: 1.2rem;
    border-radius: 12px;
    display: flex;
    align-items: center;
`;

const IconWrapper = styled.div`
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background: ${(props) => props.color};
    display: flex;
    align-items: center;
    justify-content: center;
    margin-right: 1rem;
    color: white;
`;

const InfoContent = styled.div``;

const InfoLabel = styled.p`
    margin: 0;
    color: #888;
    font-size: 0.8rem;
    margin-bottom: 0.3rem;
`;

const InfoValue = styled.p`
    margin: 0;
    color: #333;
    font-weight: 500;
`;

const Divider = styled.div`
    height: 1px;
    background: #eee;
    margin: 1.5rem 0;
`;

const PlanCard = styled.div`
    background: linear-gradient(135deg, #6e8efb, #a777e3);
    padding: 1.5rem;
    border-radius: 12px;
    color: white;
    display: flex;
    align-items: center;
    justify-content: space-between;
`;

const PlanInfo = styled.div``;

const PlanfullName = styled.h3`
    margin: 0 0 0.5rem 0;
`;

const PlanExpiry = styled.p`
    margin: 0;
    opacity: 0.9;
    font-size: 0.9rem;
`;

const UpgradeButton = styled.button`
    background: white;
    color: #6e8efb;
    border: none;
    padding: 0.6rem 1.2rem;
    border-radius: 20px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;

    &:hover {
        transform: translateY(-2px);
        box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
    }
`;

const AccountPage = styled.div`
    max-width: 1320px;
    margin: 2rem auto;
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 2rem;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;

    @media (max-width: 768px) {
        grid-template-columns: 1fr;
    }
`;

// Thêm các styled components mới
const EditSection = styled.div`
    background: white;
    border-radius: 16px;
    box-shadow: 0 8px 30px rgba(0, 0, 0, 0.08);
    padding: 2rem;
`;

const SectionTitle = styled.h3`
    color: #333;
    margin-top: 0;
    margin-bottom: 1.5rem;
    font-size: 1.3rem;
    display: flex;
    align-items: center;

    svg {
        margin-right: 0.5rem;
    }
`;

const FormGroup = styled.div`
    margin-bottom: 1.2rem;
    position: relative;
`;

const FormLabel = styled.label`
    display: block;
    color: #666;
    font-size: 0.9rem;
    margin-bottom: 0.5rem;
`;

const FormInput = styled.input`
    width: 100%;
    padding: 0.8rem;
    border: 1px solid #ddd;
    border-radius: 8px;
    font-size: 1rem;
    transition: border 0.2s;

    &:focus {
        outline: none;
        border-color: #6e8efb;
    }
`;

const ButtonGroup = styled.div`
    display: flex;
    gap: 1rem;
    margin-top: 1.5rem;
`;

const PrimaryButton = styled.button`
    background: #6e8efb;
    color: white;
    border: none;
    padding: 0.8rem 1.5rem;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    display: flex;
    align-items: center;
    transition: all 0.2s;

    &:hover {
        background: #5a7df4;
        transform: translateY(-2px);
    }

    svg {
        margin-right: 0.5rem;
    }
`;

const SecondaryButton = styled(PrimaryButton)`
    background: #f5f5f5;
    color: #666;

    &:hover {
        background: #e9e9e9;
        color: #333;
    }
`;

const ErrorMessage = styled.span`
    color: #e74c3c;
    font-size: 0.8rem;
    margin-top: 0.3rem;
    display: block;
`;

// Thêm styled component cho tab
const TabContainer = styled.div`
    display: flex;
    margin-bottom: 1.5rem;
    border-bottom: 1px solid #eee;
`;

const Tab = styled.button`
    padding: 0.8rem 1.5rem;
    background: none;
    border: none;
    cursor: pointer;
    font-weight: 600;
    color: ${(props) => (props.active ? '#6e8efb' : '#666')};
    border-bottom: 2px solid ${(props) => (props.active ? '#6e8efb' : 'transparent')};
    transition: all 0.2s;

    &:hover {
        color: #6e8efb;
    }
`;

//#endregion

const ShowPassWrap = styled.div`
    position: absolute;
    top: 50%;
    right: 5%;
`;

const AccountInfo = () => {
    const dispatch = useDispatch();
    const [activeTab, setActiveTab] = useState('info');
    const user = useSelector((state) => state.user);
    const [userData, setUserData] = useState({
        id: 'id',
        password: '',
        fullName: 'Nguyễn Văn A',
        email: 'nguyenvana@example.com',
        joinDate: '15/03/2020',
        address: 'Hà Nội, Việt Nam',
        phoneNumberNumber: '+84 123 456 789',
        plan: 'Premium',
        birthday: new Date(),
        membershipId: 'ID: MEM123456',
        delete_yn: false,
        image_yn: false,
    });
    const [editData, setEditData] = useState({
        fullName: userData?.fullName,
        email: userData?.email,
        address: userData?.address,
        phoneNumber: userData?.phoneNumber,
        password: '',
        newPassword: '',
        confirmPassword: '',
    });
    const [errors, setErrors] = useState({
        email: '',
        phoneNumber: '',
        password: '',
        newPassword: '',
        confirmPassword: '',
    });
    const [showPass, setShowPass] = useState('password');
    const [showNewPass, setShowNewPass] = useState('password');
    const [showCfPass, setShowCfPass] = useState('password');

    const validatePassword = (password) => password.length >= 6;

    useEffect(() => {
        if (user) {
            setUserData({
                id: user.id || '',
                password: '',
                fullName: user.fullName || '',
                email: user.email || '',
                joinDate: user.joinDate || '',
                address: user.address || '',
                phoneNumber: user.phoneNumber || '',
                plan: user.plan || 'Free',
                birthday: user.birthday || new Date(),
                membershipId: user.membershipId || '',
                delete_yn: user.delete_yn || false,
                image_yn: user.image_yn || false,
            });

            setEditData({
                fullName: user.fullName || '',
                email: user.email || '',
                address: user.address || '',
                phoneNumber: user.phoneNumber || '',
                password: '',
                newPassword: '',
                confirmPassword: '',
            });
        }
    }, [user]);

    // Hàm validate email
    const validateEmail = (email) => {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(String(email).toLowerCase());
    };

    // Hàm validate số điện thoại Việt Nam
    const validatephoneNumber = (phoneNumber) => {
        const re = /^(?:\+84|0)(?:\d{9,10})$/;
        return re.test(phoneNumber.replace(/\s/g, ''));
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setEditData((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleSave = async () => {
        try {
            // Validate tất cả trước khi lưu
            const newErrors = {
                email: validateEmail(editData.email) ? '' : 'Email không hợp lệ',
                phoneNumber: validatephoneNumber(editData.phoneNumber) ? '' : 'Số điện thoại không hợp lệ',
                password: editData.password != '' && editData.newPassword != '' ? '' : '',
                newPassword:
                    editData.password != '' && editData.newPassword != ''
                        ? validatePassword(editData.newPassword)
                            ? ''
                            : 'Mật khẩu phải có ít nhất 6 ký tự'
                        : '',
                confirmPassword: editData.newPassword === editData.confirmPassword ? '' : 'Mật khẩu không khớp',
            };

            setErrors(newErrors);

            // Chỉ lưu nếu không có lỗi
            if (!newErrors.email && !newErrors.phoneNumber && !newErrors.password && !newErrors.newPassword) {
                // setUserData((prev) => ({
                //     ...prev,
                //     ...editData,
                // }));

                const updatedData = {
                    ...userData,
                    ...editData,
                };

                var res = await EditProfile(updatedData);

                if (res.data.success) {
                    CustomToast.success(res?.data?.result);
                }
            }
        } catch (error) {
            const result = error?.data?.result;
            if (Array.isArray(result)) {
                result.forEach((item) => {
                    if (item?.code == 'DuplicateEmail') {
                        CustomToast.error('Email đã tồn tại.');
                    }
                });
            } else {
                CustomToast.error(result);
            }
        }
    };

    const handleCancel = () => {
        setEditData({
            fullName: userData?.fullName,
            email: userData?.email,
            address: userData?.address,
            phoneNumber: userData?.phoneNumber,
        });
        setErrors({
            email: '',
            phoneNumber: '',
        });
    };

    return (
        <AccountPage>
            <AccountContainer>
                <Header>
                    <Avatar>{userData?.fullName.charAt(0)}</Avatar>
                    <div>
                        <UserfullName>{userData?.fullName}</UserfullName>
                        <UserStatus>Đang hoạt động</UserStatus>
                    </div>
                </Header>

                <InfoGrid>
                    <InfoCard>
                        <IconWrapper color="#6e8efb">
                            <FontAwesomeIcon icon={faEnvelope} />
                        </IconWrapper>
                        <InfoContent>
                            <InfoLabel>Email</InfoLabel>
                            <InfoValue>{userData?.email}</InfoValue>
                        </InfoContent>
                    </InfoCard>

                    <InfoCard>
                        <IconWrapper color="#4fc3f7">
                            <FontAwesomeIcon icon={faCalendar} />
                        </IconWrapper>
                        <InfoContent>
                            <InfoLabel>Ngày tham gia</InfoLabel>
                            <InfoValue>{userData?.joinDate}</InfoValue>
                        </InfoContent>
                    </InfoCard>

                    <InfoCard>
                        <IconWrapper color="#66bb6a">{/* <FiMapPin /> */}</IconWrapper>
                        <InfoContent>
                            <InfoLabel>Địa chỉ</InfoLabel>
                            <InfoValue>{userData?.address}</InfoValue>
                        </InfoContent>
                    </InfoCard>

                    <InfoCard>
                        <IconWrapper color="#ffa726">{/* <FiphoneNumber /> */}</IconWrapper>
                        <InfoContent>
                            <InfoLabel>Điện thoại</InfoLabel>
                            <InfoValue>{userData?.phoneNumber}</InfoValue>
                        </InfoContent>
                    </InfoCard>
                </InfoGrid>
            </AccountContainer>

            {/* Phần thay đổi thông tin (bên phải) */}
            <EditSection>
                <TabContainer>
                    <Tab active={activeTab === 'info'} onClick={() => setActiveTab('info')}>
                        Thông tin cá nhân
                    </Tab>
                    <Tab active={activeTab === 'password'} onClick={() => setActiveTab('password')}>
                        Đổi mật khẩu
                    </Tab>
                </TabContainer>

                {activeTab === 'info' ? (
                    <>
                        <SectionTitle>
                            <FontAwesomeIcon icon={faEdit} /> Thay đổi thông tin
                        </SectionTitle>

                        <FormGroup>
                            <FormLabel>Họ và tên</FormLabel>
                            <FormInput
                                type="text"
                                name="fullName"
                                value={editData.fullName}
                                onChange={handleInputChange}
                            />
                        </FormGroup>

                        <FormGroup>
                            <FormLabel>Email</FormLabel>
                            <FormInput type="email" name="email" value={editData.email} onChange={handleInputChange} />
                            {errors.email && <ErrorMessage>{errors.email}</ErrorMessage>}
                        </FormGroup>

                        <FormGroup>
                            <FormLabel>Địa chỉ</FormLabel>
                            <FormInput
                                type="text"
                                name="address"
                                value={editData.address}
                                onChange={handleInputChange}
                            />
                        </FormGroup>

                        <FormGroup>
                            <FormLabel>Số điện thoại</FormLabel>
                            <FormInput
                                type="tel"
                                name="phoneNumber"
                                value={editData.phoneNumber}
                                onChange={handleInputChange}
                            />
                            {errors.phoneNumber && <ErrorMessage>{errors.phoneNumber}</ErrorMessage>}
                        </FormGroup>
                    </>
                ) : (
                    <>
                        <FormGroup>
                            <FormLabel>Mật khẩu hiện tại</FormLabel>
                            <FormInput
                                type={showPass}
                                name="password"
                                value={editData?.password}
                                onChange={handleInputChange}
                                className={errors.password ? 'error' : ''}
                            />
                            <ShowPassWrap>
                                {showPass == 'text' ? (
                                    <FontAwesomeIcon onClick={() => setShowPass('password')} icon={faEyeSlash} />
                                ) : (
                                    <FontAwesomeIcon onClick={() => setShowPass('text')} icon={faEye} />
                                )}
                            </ShowPassWrap>
                            {errors.password && <ErrorMessage>{errors.password}</ErrorMessage>}
                        </FormGroup>

                        <FormGroup>
                            <FormLabel>Mật khẩu mới</FormLabel>
                            <FormInput
                                type={showNewPass}
                                name="newPassword"
                                autoComplete="off"
                                value={editData?.newPassword}
                                onChange={handleInputChange}
                                className={errors.newPassword ? 'error' : ''}
                            />
                            <ShowPassWrap>
                                {showNewPass == 'text' ? (
                                    <FontAwesomeIcon onClick={() => setShowNewPass('password')} icon={faEyeSlash} />
                                ) : (
                                    <FontAwesomeIcon onClick={() => setShowNewPass('text')} icon={faEye} />
                                )}
                            </ShowPassWrap>
                            {errors.newPassword && <ErrorMessage>{errors.newPassword}</ErrorMessage>}
                        </FormGroup>

                        <FormGroup>
                            <FormLabel>Xác nhận mật khẩu</FormLabel>
                            <FormInput
                                type={showCfPass}
                                name="confirmPassword"
                                autoComplete="off"
                                value={editData?.confirmPassword}
                                onChange={handleInputChange}
                                className={errors.confirmPassword ? 'error' : ''}
                            />
                            <ShowPassWrap>
                                {showCfPass == 'text' ? (
                                    <FontAwesomeIcon onClick={() => setShowCfPass('password')} icon={faEyeSlash} />
                                ) : (
                                    <FontAwesomeIcon onClick={() => setShowCfPass('text')} icon={faEye} />
                                )}
                            </ShowPassWrap>
                            {errors.confirmPassword && <ErrorMessage>{errors.confirmPassword}</ErrorMessage>}
                        </FormGroup>
                    </>
                )}

                <ButtonGroup>
                    <PrimaryButton onClick={handleSave}>
                        <FontAwesomeIcon icon={faSave} /> Lưu thay đổi
                    </PrimaryButton>
                    <SecondaryButton onClick={handleCancel}>
                        <FontAwesomeIcon icon={faX} /> Hủy bỏ
                    </SecondaryButton>
                </ButtonGroup>
            </EditSection>
        </AccountPage>
    );
};

export default AccountInfo;
