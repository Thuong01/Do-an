import { createSlice } from '@reduxjs/toolkit';

const initialState = {
    getInfoSuccess: false,
    id: null,
    fullName: null,
    address: null,
    avartar: null,
    userName: null,
    birthday: null,
    email: null,
    phoneNumber: null,
    cartId: null,
    wishListId: null,
};

const userSlide = createSlice({
    name: 'user',
    initialState,
    reducers: {
        saveUser: function (state, action) {
            state.getInfoSuccess = true;
            state.id = action.payload.id ?? null;
            state.fullName = action.payload.fullName ?? null;
            state.address = action.payload.address ?? null;
            state.avartar = action.payload.avartar ?? null;
            state.userName = action.payload.userName ?? null;
            state.birthday = action.payload.birthday ?? null;
            state.email = action.payload.email ?? null;
            state.phoneNumber = action.payload.phoneNumber ?? null;
            state.cartId = action.payload.cartId ?? null;
            state.wishListId = action.payload.wishListId ?? null;
        },
        removeUser: function (state) {
            state.getInfoSuccess = false;
            state.id = null;
            state.fullName = null;
            state.address = null;
            state.avartar = null;
            state.userName = null;
            state.birthday = null;
            state.email = null;
            state.phoneNumber = null;
            state.cartId = null;
            state.wishListId = null;
        },
    },
});

export const { saveUser, removeUser } = userSlide.actions;
export default userSlide.reducer;
