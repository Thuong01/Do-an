import { useEffect, useState } from 'react';
import { useDispatch } from 'react-redux';
import { useLocation, useNavigate } from 'react-router-dom';
import styled from 'styled-components';
import { Col, Container, Row } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAngleDown, faAngleLeft, faAngleRight, faList } from '@fortawesome/free-solid-svg-icons';

import { GetProductCategories } from '../../Services/ProductCategoryService';
import { GetProducts } from '../../Services/ProductService';
import Product from '../../Components/Product/Product';
import useTitle from '../../Context/useTitle';
import { setLoading } from '../../Redux/Slices/LoadingSlice';

//#region  Styled Components
const SearchPageWrapper = styled.div`
    padding: 40px 0;
    background: #f9f9f9;
    min-height: 100vh;
`;

const CategorySection = styled.div`
    background: white;
    padding: 25px;
    border-radius: 12px;
    box-shadow: 0 2px 15px rgba(0, 0, 0, 0.05);
`;

const CategoryTitle = styled.h2`
    font-size: 1.25rem;
    color: #2d2d2d;
    margin-bottom: 1.5rem;
    display: flex;
    align-items: center;
    gap: 10px;
`;

const CategoryFilterItem = styled.button`
    display: flex;
    align-items: center;
    width: 100%;
    padding: 12px 15px;
    margin: 5px 0;
    border: none;
    background: ${(props) => (props.active ? '#fff5f6' : 'transparent')};
    color: ${(props) => (props.active ? '#ff4d4f' : '#555')};
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.3s ease;
    position: relative;
    font-weight: ${(props) => (props.active ? '600' : '400')};
    padding-left: ${(props) => props.level * 20}px;
    position: relative;

    &:hover {
        background: #fff5f6;
        color: #ff4d4f;
    }

    svg {
        width: 12px;
        height: 12px;
        margin-right: 10px;
        fill: ${(props) => (props.active ? '#ff4d4f' : '#ccc')};
    }
`;

const ProductGrid = styled.div`
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    gap: 25px;
    margin: 30px 0;
`;

const SortingDropdown = styled.select`
    padding: 10px 15px;
    border: 1px solid #eee;
    border-radius: 8px;
    background: white;
    font-size: 0.9rem;
    width: 250px;
    appearance: none;
    background-image: url("data:image/svg+xml;charset=UTF-8,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3e%3cpolyline points='6 9 12 15 18 9'%3e%3c/polyline%3e%3c/svg%3e");
    background-repeat: no-repeat;
    background-position: right 1rem center;
    background-size: 1em;

    &:focus {
        outline: none;
        border-color: #ff4d4f;
        box-shadow: 0 0 0 2px rgba(255, 77, 79, 0.2);
    }
`;

const PaginationWrapper = styled.div`
    display: flex;
    justify-content: center;
    gap: 10px;
    margin-top: 40px;
`;

const PaginationButton = styled.button`
    padding: 8px 15px;
    border: 1px solid #eee;
    background: ${(props) => (props.active ? '#ff4d4f' : 'white')};
    color: ${(props) => (props.active ? 'white' : '#555')};
    border-radius: 6px;
    cursor: pointer;
    transition: all 0.2s ease;

    &:hover {
        border-color: #ff4d4f;
        color: ${(props) => (props.active ? 'white' : '#ff4d4f')};
    }

    &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
    }
`;

const ResultsHeader = styled.div`
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 30px;
    background: white;
    padding: 20px;
    border-radius: 12px;
    box-shadow: 0 2px 15px rgba(0, 0, 0, 0.05);
`;

const ResultsInfo = styled.div`
    display: flex;
    gap: 20px;
    color: #666;
    font-size: 0.9rem;
`;
//#endregion

const CategoryChildren = styled.div`
    margin-left: 20px;
    border-left: 1px solid #eee;
`;

// Component
const SearchPage = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const dispatch = useDispatch();
    const queryParams = new URLSearchParams(location.search);

    const [categories, setCategories] = useState([]);
    const [products, setProducts] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [totalPages, setTotalPages] = useState(1);

    const keyword = queryParams.get('keyword') || '';
    const category = queryParams.get('category') || '';
    const page = parseInt(queryParams.get('page')) || 1;
    const sort = queryParams.get('sort') || 'name';

    useTitle(`Tìm kiếm - ${keyword}`);

    // Hàm chuyển đổi mảng phẳng thành cấu trúc cây
    const buildCategoryTree = (categories, parentId = null) => {
        return categories
            .filter((category) => category.parent_ID === parentId)
            .map((category) => ({
                ...category,
                children: buildCategoryTree(categories, category.id),
            }));
    };

    const fetchData = async () => {
        try {
            dispatch(setLoading(true));

            // Fetch categories
            const categoriesRes = await GetProductCategories({
                filter: null,
                Status: -1,
                Sorting: 'name',
            });
            // Xây dựng cấu trúc cây
            const categoryTree = buildCategoryTree(categoriesRes?.data || []);
            setCategories(categoryTree);

            // Fetch products
            const productsRes = await GetProducts({
                Status: -1,
                PageNumber: page,
                PageSize: 21,
                Sorting: sort,
                Category: category,
                Filter: keyword,
            });

            setProducts(productsRes?.data?.data || []);
            setTotalCount(productsRes?.data?.totalCount || 0);
            setTotalPages(productsRes?.data?.totalPage || 1);
        } finally {
            dispatch(setLoading(false));
        }
    };

    useEffect(() => {
        fetchData();

        console.log(categories);
    }, [location.search]);

    const handleSearchParams = (params) => {
        navigate(`/search?${new URLSearchParams(params).toString()}`);
    };

    const handleSortChange = (value) => {
        handleSearchParams({ ...Object.fromEntries(queryParams), sort: value, page: 1 });
    };

    const renderPagination = () => {
        const pages = [];
        const range = 2;

        let start = Math.max(1, page - range);
        let end = Math.min(totalPages, page + range);

        return (
            <PaginationWrapper>
                <PaginationButton
                    disabled={page === 1}
                    onClick={() => handleSearchParams({ ...Object.fromEntries(queryParams), page: page - 1 })}
                >
                    <FontAwesomeIcon icon={faAngleLeft} />
                </PaginationButton>

                {Array.from({ length: end - start + 1 }, (_, i) => start + i).map((p) => (
                    <PaginationButton
                        key={p}
                        active={page === p}
                        onClick={() => handleSearchParams({ ...Object.fromEntries(queryParams), page: p })}
                    >
                        {p}
                    </PaginationButton>
                ))}

                <PaginationButton
                    disabled={page === totalPages}
                    onClick={() => handleSearchParams({ ...Object.fromEntries(queryParams), page: page + 1 })}
                >
                    <FontAwesomeIcon icon={faAngleRight} />
                </PaginationButton>
            </PaginationWrapper>
        );
    };

    const CategoryItem = ({ category, level = 0, currentCategory, onSelect }) => {
        const [expanded, setExpanded] = useState(false);

        return (
            <div>
                <CategorySection>
                    <CategoryTitle>
                        <FontAwesomeIcon icon={faList} />
                        Danh mục sản phẩm
                    </CategoryTitle>

                    <CategoryFilterItem
                        onClick={() =>
                            handleSearchParams({ ...Object.fromEntries(queryParams), category: '', page: 1 })
                        }
                        active={!category}
                    >
                        <svg viewBox="0 0 4 7">
                            <polygon points="4 3.5 0 0 0 7" />
                        </svg>
                        Tất cả sản phẩm
                    </CategoryFilterItem>

                    {categories.map((item) => (
                        <CategoryFilterItem
                            key={item.alias}
                            onClick={() =>
                                handleSearchParams({
                                    ...Object.fromEntries(queryParams),
                                    category: item.alias,
                                    page: 1,
                                })
                            }
                            active={category === item.alias}
                        >
                            <svg viewBox="0 0 4 7">
                                <polygon points="4 3.5 0 0 0 7" />
                            </svg>
                            {item.name}
                        </CategoryFilterItem>
                    ))}
                </CategorySection>
            </div>
        );
    };

    return (
        <SearchPageWrapper>
            <Container>
                <Row>
                    <Col md={3}>
                        <CategorySection>
                            <CategoryTitle>
                                <FontAwesomeIcon icon={faList} />
                                Danh mục sản phẩm
                            </CategoryTitle>

                            <CategoryFilterItem
                                onClick={() =>
                                    handleSearchParams({
                                        ...Object.fromEntries(queryParams),
                                        category: '',
                                        page: 1,
                                    })
                                }
                                active={!category}
                            >
                                <svg viewBox="0 0 4 7">
                                    <polygon points="4 3.5 0 0 0 7" />
                                </svg>
                                Tất cả sản phẩm
                            </CategoryFilterItem>

                            {categories.map((item) => (
                                <>
                                    {item?.children?.length <= 0 ? (
                                        <>
                                            <CategoryFilterItem
                                                onClick={() =>
                                                    handleSearchParams({
                                                        ...Object.fromEntries(queryParams),
                                                        category: item.alias,
                                                        page: 1,
                                                    })
                                                }
                                                active={category === item.alias}
                                                key={item.alias}
                                            >
                                                <svg viewBox="0 0 4 7">
                                                    <polygon points="4 3.5 0 0 0 7" />
                                                </svg>
                                                {item.name}
                                            </CategoryFilterItem>
                                        </>
                                    ) : (
                                        <>
                                            <CategoryFilterItem key={item.alias}>
                                                <svg viewBox="0 0 4 7">
                                                    <polygon points="4 3.5 0 0 0 7" />
                                                </svg>
                                                {item.name}
                                            </CategoryFilterItem>
                                        </>
                                    )}

                                    <div style={{ marginLeft: '20px' }}>
                                        {item?.children?.map((cate, i) => (
                                            <CategoryFilterItem
                                                key={cate.alias}
                                                onClick={() =>
                                                    handleSearchParams({
                                                        ...Object.fromEntries(queryParams),
                                                        category: cate.alias,
                                                        page: 1,
                                                    })
                                                }
                                                active={category === cate.alias}
                                            >
                                                <svg viewBox="0 0 4 7">
                                                    <polygon points="4 3.5 0 0 0 7" />
                                                </svg>
                                                {cate.name}
                                            </CategoryFilterItem>
                                        ))}
                                    </div>
                                </>
                            ))}
                        </CategorySection>
                    </Col>

                    <Col md={9}>
                        <ResultsHeader>
                            <div>
                                <span>Sắp xếp theo: </span>
                                <SortingDropdown value={sort} onChange={(e) => handleSortChange(e.target.value)}>
                                    <option value="name">Mặc định</option>
                                    <option value="price">Giá: Thấp đến cao</option>
                                    <option value="price_desc">Giá: Cao đến thấp</option>
                                </SortingDropdown>
                            </div>

                            <ResultsInfo>
                                <span>{totalCount} kết quả</span>
                                <span>
                                    Trang {page} / {totalPages}
                                </span>
                            </ResultsInfo>
                        </ResultsHeader>

                        {products.length > 0 ? (
                            <>
                                <ProductGrid>
                                    {products.map((product) => (
                                        <Product key={product.id} product={product} hoverEffect showQuickView />
                                    ))}
                                </ProductGrid>

                                <div>{totalPages > 1 && renderPagination()}</div>
                            </>
                        ) : (
                            <div className="text-center">Không có sản phẩm nào</div>
                        )}
                    </Col>
                </Row>
            </Container>
        </SearchPageWrapper>
    );
};

export default SearchPage;
