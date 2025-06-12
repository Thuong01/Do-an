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

const styles = {
    gridContainer: {
        display: 'grid',
        gridTemplateColumns: 'repeat(4, 1fr)', // 5 cột
        gap: '16px', // khoảng cách giữa các item
    },
    productCard: {
        padding: '12px',
        border: '1px solid #ccc',
        textAlign: 'center',
        borderRadius: '8px',
    },
};

const Container = styled.div`
    display: flex;
    background: url(${backgroundImg});
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 100vh;
    font-family: 'Helvetica Neue', Arial, sans-serif;
`;

const TitleWrapper = styled.div`
    background: rgba(255, 255, 255, 50%);
    padding: 10px 15px;
    border-radius: 10px;
`;

const Title = styled.h1`
    font-size: 2.5rem;
    font-weight: 300;
    letter-spacing: 1px;
    margin-bottom: 0.5rem;
    color: #333;
    text-transform: uppercase;
`;

const Subtitle = styled.h2`
    font-size: 1.8rem;
    font-weight: 300;
    letter-spacing: 1px;
    margin-top: 0;
    margin-bottom: 3rem;
    color: #333;
    text-transform: uppercase;
`;

const BrandContainer = styled.div`
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 1.5rem;
`;

const Brand = styled.div`
    font-size: 1.5rem;
    font-weight: 300;
    letter-spacing: 1px;
    color: #333;
    text-transform: uppercase;
    cursor: pointer;
    transition: all 0.3s ease;

    &:hover {
        color: #666;
        transform: scale(1.05);
    }
`;

const Home = () => {
    var dispatch = useDispatch();

    var [newProducts, setNewProducts] = useState({
        page: 1,
        count: 0,
        totalPage: 0,
        totalCount: 0,
        data: [],
    });
    var [topSellingProducts, setTopSellingProducts] = useState({
        page: 1,
        count: 0,
        totalPage: 0,
        totalCount: 0,
        data: [],
    });

    const fetchNewProductData = async () => {
        dispatch(setLoading(true));
        try {
            var productData = await GetProducts({ Status: -1, PageNumber: 1, PageSize: 8, Sorting: 'date' });

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
            var productData = await GetProducts({ Status: -1, PageNumber: 1, PageSize: 10, Sorting: 'name' });

            console.log(productData);

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
        <div>
            <Container>
                <TitleWrapper>
                    <Title>WHICH LEICA LENS</Title>
                    <Subtitle>LEICA IS RIGHT FOR YOU?</Subtitle>
                </TitleWrapper>
            </Container>

            <div className="section">
                <div className="container">
                    <h5>Sản phẩm mới</h5>
                    <div style={styles.gridContainer}>
                        {newProducts?.data?.map((item, ind) => (
                            <Product product={item} flag="NEW" key={ind}></Product>
                        ))}
                    </div>
                </div>
            </div>

            <HotDeal />

            <div className="section">
                <div className="container">
                    <h5>Sản phẩm bán chạy</h5>
                    <div style={styles.gridContainer}>
                        {topSellingProducts?.data?.map((item, ind) => (
                            <Product product={item} flag="TOP" key={ind}></Product>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Home;
