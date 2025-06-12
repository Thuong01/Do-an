import clsx from 'clsx';
import React, { useEffect, useState } from 'react';
import './CategoryMenu.scss';
import { setLoading } from '../../Redux/Slices/LoadingSlice';
import { useDispatch } from 'react-redux';
import { GetProductCategories } from '../../Services/ProductCategoryService';
import { NavLink } from 'react-router-dom';
import styled from 'styled-components';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronRight } from '@fortawesome/free-solid-svg-icons';

//#region Style
const CategoryContainer = styled.div`
    position: absolute;
    top: 100%;
    left: 125px;
    z-index: 1050;
    display: flex;
    background: white;
    border-radius: 12px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
    overflow: hidden;
    height: 400px;
`;

const ParentCategories = styled.div`
    width: 280px;
    border-right: 1px solid #f0f0f0;
    overflow-y: auto;
`;

const ParentCategoryItem = styled.div`
    padding: 14px 20px;
    cursor: pointer;
    font-weight: 500;
    color: ${(props) => (props.$active ? '#3b82f6' : '#333')};
    background: ${(props) => (props.$active ? '#f0f7ff' : 'transparent')};
    border-left: 3px solid ${(props) => (props.$active ? '#3b82f6' : 'transparent')};
    transition: all 0.2s ease;
    display: flex;
    justify-content: space-between;
    align-items: center;

    &:hover {
        background: #f5f5f5;
    }
`;

const ChildCategories = styled.div`
    flex: 1;
    padding: 20px;
    overflow-y: auto;
    max-width: 400px;
    width: 400px;
`;

const CategoryTitle = styled.h3`
    margin: 0 0 16px 0;
    color: #333;
    font-size: 1.1rem;
    padding-bottom: 8px;
    border-bottom: 1px solid #eee;
`;

const ChildCategoryList = styled.ul`
    list-style: none;
    padding: 0;
    margin: 0;
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
`;

const ChildCategoryItem = styled.li`
    padding: 0;
`;

const CategoryLink = styled(NavLink)`
    display: block;
    padding: 14px 20px;
    color: #555;
    text-decoration: none;
    border-radius: 6px;
    border-left: 3px solid transparent;
    transition: all 0.2s ease;

    &:hover {
        background: #f5f5f5;
        color: #3b82f6;
    }

    &.active {
        background: #f0f7ff;
        color: #3b82f6;
        font-weight: 500;
    }
`;

const NoCategories = styled.div`
    display: flex;
    align-items: center;
    justify-content: center;
    height: 100%;
    color: #888;
`;
//#endregion

export default function CategoryMenu() {
    const dispatch = useDispatch();
    const [productCategories, setProductCategories] = useState([]);
    const [selectedCategoryId, setSelectedCategoryId] = useState(productCategories[0]?.id || null);

    const handleSelect = (id) => {
        setSelectedCategoryId(id);
    };

    var selectedCategory = productCategories.find((cat) => cat.id === selectedCategoryId);

    const fetchProductcategories = async () => {
        dispatch(setLoading(true));
        try {
            var proCategData = await GetProductCategories({ filter: null, Status: -1, Sorting: 'name' });

            setProductCategories(groupCategories(proCategData?.data));
        } catch (error) {
            console.log(error);
        } finally {
            dispatch(setLoading(false));
        }
    };

    function groupCategories(data) {
        const map = {};
        const roots = [];

        // Bước 1: Tạo map từ id => category
        data.forEach((item) => {
            map[item.id] = { ...item, children: [] };
        });

        // Bước 2: Gán mỗi item vào parent của nó hoặc thành root
        data.forEach((item) => {
            if (item.parent_ID) {
                const parent = map[item.parent_ID];
                if (parent) {
                    parent.children.push(map[item.id]);
                } else {
                    // Nếu parent không tìm thấy, có thể log cảnh báo
                    console.warn('Không tìm thấy parent:', item);
                }
            } else {
                roots.push(map[item.id]);
            }
        });

        return roots;
    }

    useEffect(() => {
        fetchProductcategories();
    }, []);

    useEffect(() => {
        if (productCategories.length > 0) {
            setSelectedCategoryId(productCategories[0].id);
        }

        console.log(productCategories);
        console.log(selectedCategoryId);
    }, [productCategories]);

    return (
        <CategoryContainer>
            <ParentCategories>
                {productCategories?.map((cat) =>
                    cat?.children?.length > 0 ? (
                        <ParentCategoryItem
                            key={cat.id}
                            onClick={() => handleSelect(cat.id)}
                            $active={selectedCategoryId === cat.id}
                        >
                            {cat.name}
                            {selectedCategoryId === cat.id && <FontAwesomeIcon icon={faChevronRight} />}
                        </ParentCategoryItem>
                    ) : (
                        <CategoryLink
                            key={cat.id}
                            onClick={() => handleSelect(cat.id)}
                            $active={selectedCategoryId === cat.id}
                            to={`/search?category=${cat?.alias}&page=1`}
                        >
                            {cat.name}
                            {selectedCategoryId === cat.id}
                        </CategoryLink>
                    ),
                )}
            </ParentCategories>

            {selectedCategory?.children?.length > 0 ? (
                <ChildCategories>
                    <>
                        <CategoryTitle>{selectedCategory.name}</CategoryTitle>
                        <ChildCategoryList>
                            {selectedCategory.children.map((child) => (
                                <ChildCategoryItem key={child.id}>
                                    <CategoryLink
                                        to={`/search?category=${child?.alias}&page=1`}
                                        activeClassName="active"
                                    >
                                        {child.name}
                                    </CategoryLink>
                                </ChildCategoryItem>
                            ))}
                        </ChildCategoryList>
                    </>
                </ChildCategories>
            ) : (
                <></>
            )}
        </CategoryContainer>
    );
}
