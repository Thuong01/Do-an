import { useEffect, useState } from 'react';
import HotDeal from '../../Components/HotDeal';
import { Product, ProductGroup } from '../../Components/Product';
import { GetProducts } from '../../Services/ProductService';
import { useDispatch } from 'react-redux';
import { setLoading } from '../../Redux/Slices/LoadingSlice';
import { saveUserInfor } from '../../Redux/Actions/userAction';
import styled from 'styled-components';
import './Home.scss';
import backgroundImg from '../../assets/imgs/173711113572710643_Leica_SL3_S_Ambient_Hero_HiRes.jpg';
import useTitle from '../../Context/useTitle';

const HomeContainer = styled.div`
    background-color: #f8f9fa;
`;

const HeroContainer = styled.div`
    display: flex;
    background: url(${backgroundImg}) center/cover no-repeat;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 100vh;
    font-family: 'Helvetica Neue', Arial, sans-serif;
    position: relative;

    &::before {
        content: '';
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(0, 0, 0, 0.3);
    }
`;

const TitleWrapper = styled.div`
    background: rgba(255, 255, 255, 0.85);
    padding: 30px 40px;
    border-radius: 15px;
    text-align: center;
    z-index: 1;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.15);
    max-width: 80%;
`;

const Title = styled.h1`
    font-size: 3rem;
    font-weight: 300;
    letter-spacing: 2px;
    margin-bottom: 0.5rem;
    color: #333;
    text-transform: uppercase;

    @media (max-width: 768px) {
        font-size: 2rem;
    }
`;

const Subtitle = styled.h2`
    font-size: 2rem;
    font-weight: 300;
    letter-spacing: 1.5px;
    margin-top: 0;
    margin-bottom: 0;
    color: #333;
    text-transform: uppercase;

    @media (max-width: 768px) {
        font-size: 1.5rem;
    }
`;

const Section = styled.section`
    padding: 60px 0;
    background-color: #fff;

    &:nth-child(even) {
        background-color: #f8f9fa;
    }
`;

const SectionContainer = styled.div`
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 20px;
`;

const SectionHeader = styled.div`
    text-align: center;
    margin-bottom: 40px;
`;

const SectionTitle = styled.h2`
    font-size: 2rem;
    font-weight: 300;
    text-transform: uppercase;
    letter-spacing: 1px;
    color: #333;
    position: relative;
    display: inline-block;
    padding-bottom: 15px;

    &::after {
        content: '';
        position: absolute;
        bottom: 0;
        left: 50%;
        transform: translateX(-50%);
        width: 80px;
        height: 2px;
        background-color: #d4af37;
    }
`;

const ProductGrid = styled.div`
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    gap: 10px;
    margin-top: 30px;

    @media (max-width: 768px) {
        grid-template-columns: repeat(2, 1fr);
        gap: 15px;
    }

    @media (max-width: 480px) {
        grid-template-columns: 1fr;
    }
`;

const Home = () => {
    const dispatch = useDispatch();
    useTitle('Trang chủ');

    const [newProducts, setNewProducts] = useState({
        page: 1,
        count: 0,
        totalPage: 0,
        totalCount: 0,
        data: [],
    });

    const [topSellingProducts, setTopSellingProducts] = useState({
        page: 1,
        count: 0,
        totalPage: 0,
        totalCount: 0,
        data: [],
    });

    const fetchNewProductData = async () => {
        dispatch(setLoading(true));
        try {
            const productData = await GetProducts({
                Status: -1,
                PageNumber: 1,
                PageSize: 8,
                Sorting: 'date',
            });
            setNewProducts(productData.data);
        } catch (error) {
            console.log(error);
        } finally {
            dispatch(setLoading(false));
        }
    };

    const fetchTopSellingProductData = async () => {
        dispatch(setLoading(true));
        try {
            const productData = await GetProducts({
                Status: -1,
                PageNumber: 1,
                PageSize: 8,
                Sorting: 'name',
            });
            setTopSellingProducts(productData.data);
        } catch (error) {
            console.log(error);
        } finally {
            dispatch(setLoading(false));
        }
    };

    useEffect(() => {
        const fetchUserInfor = async () => {
            dispatch(setLoading(true));
            try {
                dispatch(saveUserInfor());
            } catch (error) {
                console.log(error);
            } finally {
                dispatch(setLoading(false));
            }
        };

        fetchUserInfor();
        fetchNewProductData();
        fetchTopSellingProductData();
    }, [dispatch]);

    return (
        <HomeContainer>
            <HeroContainer>
                <TitleWrapper>
                    <Title>WHICH LEICA LENS</Title>
                    <Subtitle>LEICA IS RIGHT FOR YOU?</Subtitle>
                </TitleWrapper>
            </HeroContainer>

            <Section>
                <SectionContainer>
                    <SectionHeader>
                        <SectionTitle>Sản phẩm mới</SectionTitle>
                    </SectionHeader>
                    <ProductGrid>
                        {newProducts?.data?.map((item, ind) => (
                            <Product product={item} flag="NEW" key={ind} />
                        ))}
                    </ProductGrid>
                </SectionContainer>
            </Section>

            <HotDeal />

            <Section>
                <SectionContainer>
                    <SectionHeader>
                        <SectionTitle>Sản phẩm bán chạy</SectionTitle>
                    </SectionHeader>
                    <ProductGrid>
                        {topSellingProducts?.data?.map((item, ind) => (
                            <Product product={item} flag="HOT" key={ind} />
                        ))}
                    </ProductGrid>
                </SectionContainer>
            </Section>
        </HomeContainer>
    );
};

export default Home;
