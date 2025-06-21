using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Datas.Infrastructures.Interfaces;
using Datas.ViewModels.Cart;
using Models.Models;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using Services.Repositories;

namespace Services.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<List<Guid>> GetCartsData()
        {
            return _cartRepository.GetCartsData().Result;
        }

        public async Task<int> CreateAsync(CartItemCreateViewModel create, string userId)
        {
            var cartIsExist = _cartRepository.GetSingleByCondition(x => x.User_Id == userId);

            if (cartIsExist == null)
            {
                var cart = new Cart
                {
                    User_Id = userId,
                };

                cartIsExist = _cartRepository.Add(cart);
            }

            var itemIsExist = _cartItemRepository.GetSingleByCondition(x => x.Cart_Id == cartIsExist.Id && x.Product_Id == create.Product_Id);

            if (itemIsExist != null)
            {
                itemIsExist.Quantity += create.Quantity;
            }
            else
            {
                create.Cart_Id = cartIsExist.Id;
                var cartItems = _cartItemRepository.Add(_mapper.Map<CartItem>(create));
            }

            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> CreateCartAsync(string userId)
        {
            var cartIsExist = _cartRepository.GetSingleByCondition(x => x.User_Id == userId);

            if (cartIsExist == null)
            {
                var res = _cartRepository.Add(new Cart { User_Id = userId });

                return await _unitOfWork.CommitAsync();
            }

            return 0;

        }

        public async Task<int> DeleteAsync(string userId, Guid cartId, Guid productId)
        {
            var cartItem = _cartItemRepository.GetSingleByCondition(x => x.Product_Id == productId && x.Cart_Id == cartId);

            if (cartItem == null)
                return 0;

            _cartItemRepository.Delete(cartItem);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<Guid> GetCartId(string userId)
        {
            var cart = _cartRepository.GetSingleByCondition(x => x.User_Id == userId);

            return cart.Id;
        }

        public async Task<CartViewModel> GetCarts(string userId)
        {
            return _mapper.Map<CartViewModel>(_cartRepository.GetSingleByCondition(x => x.User_Id == userId, new[] { "CartItems", "CartItems.Product" }));
        }

        public async Task<int> UpdateAsync(Guid cartId, Guid productId, int quantity)
        {
            var item = _cartItemRepository.GetSingleByCondition(x => x.Cart_Id == cartId && x.Product_Id == productId);

            if (item == null)
                return 0;

            item.Quantity = quantity;

            return await _unitOfWork.CommitAsync();
        }
    }
}
