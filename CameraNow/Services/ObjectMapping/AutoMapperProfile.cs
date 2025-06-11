using AutoMapper;
using Commons.Commons;
using Datas.Extentions;
using Datas.ViewModels;
using Datas.ViewModels.Auth;
using Datas.ViewModels.Cart;
using Datas.ViewModels.Coupon;
using Datas.ViewModels.Feedback;
using Datas.ViewModels.Image;
using Datas.ViewModels.Order;
using Datas.ViewModels.Permissions;
using Datas.ViewModels.Product;
using Models.Models;

namespace Services.ObjectMapping
{
    public class MyShopAutoMapperProfile : Profile
    {
        public MyShopAutoMapperProfile()
        {
            CreateMap<string, Guid>().ConvertUsing(s => Guid.Parse(s));
            CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>))
                .ConvertUsing(typeof(PaginatedListConverter<,>));

            #region Product Category
            CreateMap<CreateProductCategoryViewModel, ProductCategory>()
                .ForMember(x => x.Alias, e => e.MapFrom(x => CommonExtensions.GenerateSEOTitle(x.Name)));
            CreateMap<ProductCategoryViewModel, ProductCategory>();
            CreateMap<ProductCategoryViewModel, UpdateProductCategoryViewModel>();
            CreateMap<UpdateProductCategoryViewModel, ProductCategory>()
                .ForMember(x => x.Alias, e => e.MapFrom(x => CommonExtensions.GenerateSEOTitle(x.Name)))
                .ForMember(dest => dest.ID, opt => opt.Ignore());
            CreateMap<ProductCategory, ProductCategoryViewModel>();
            #endregion

            #region Product
            CreateMap<CreateProductViewModel, Product>()
                .ForMember(x => x.Alias, e => e.MapFrom(x => CommonExtensions.GenerateSEOTitle(x.Name)));
            CreateMap<ProductViewModel, Product>();
            CreateMap<ProductViewModel, UpdateProductViewModel>()
                .ForMember(x => x.Categories, e => e.MapFrom(x => x.Category_ID))
                .ForMember(x => x.Images_existed, e => e.MapFrom(x => x.Images.Select(img => img.Link)));
            CreateMap<UpdateProductViewModel, Product>()
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ForMember(x => x.Category_ID, e => e.MapFrom(x => x.Categories))
                .ForMember(x => x.Alias, e => e.MapFrom(x => CommonExtensions.GenerateSEOTitle(x.Name)));
            CreateMap<Product, ProductViewModel>()
                .ForMember(x => x.Category_Name, e => e.MapFrom(x => x.ProductCategory.Name))
                .ForMember(x => x.Image, e => e.MapFrom(x =>
                    x.Image.Contains("http://res.cloudinary.com")
                        ? x.Image
                        : "https://localhost:7110" + x.Image))
                .ForMember(x => x.Category_Alias, e => e.MapFrom(x => x.ProductCategory.Alias));
            //.ForMember(x => x.Brand_Name, e => e.MapFrom(x => x.Brand.Name));

            #endregion

            CreateMap<CreateImageViewModel, ProductImage>();
            CreateMap<ProductImage, ImageViewModel>();
            CreateMap<ProductImage, CreateImageViewModel>();

            CreateMap<FeedbackImage, FeedbackImageViewModel>();
            CreateMap<FeedbackImageCreateViewModel, FeedbackImage>();

            //CreateMap<Brand, BrandViewModel>();
            //CreateMap<BrandViewModel, Brand>();
            //CreateMap<CreateBrandViewModel, Brand>();

            #region Coupon
            CreateMap<Coupon, CouponViewModel>();
            CreateMap<CouponViewModel, Coupon>();
            CreateMap<CouponCreateViewModel, Coupon>();
            CreateMap<CouponUpdateViewModel, Coupon>();
            CreateMap<CouponViewModel, CouponUpdateViewModel>();
            #endregion

            #region Order
            CreateMap<Order, OrderViewModel>();
            CreateMap<OrderViewModel, Order>();
            CreateMap<OrderCreateViewModel, Order>();
            CreateMap<OrderUpdateViewModel, Order>();
            CreateMap<OrderViewModel, OrderUpdateViewModel>();
            #endregion

            CreateMap<OrderDetail, OrderDetailViewModel>()
                .ForMember(x => x.Product_Name, e => e.MapFrom(x => x.Product.Name))
                .ForMember(x => x.Product_Image, e => e.MapFrom(x => x.Product.Image))
                .ForMember(x => x.Product_Price, e => e.MapFrom(x => x.Product.Price));
            CreateMap<OrderDetailViewModel, OrderDetail>();
            CreateMap<OrderDetailCreateViewModel, OrderDetail>();
            CreateMap<OrderDetailUpdateViewModel, OrderDetail>();
            CreateMap<OrderDetailViewModel, OrderDetailUpdateViewModel>();

            CreateMap<Feedback, Data>()
                .ForMember(x => x.Date, e => e.MapFrom(x => x.Date.ToLocalTime()))
                .ForMember(x => x.User_Name, e => e.MapFrom(x => x.User.UserName))
                .ForMember(x => x.User_Avatar, e => e.MapFrom(x => x.User.Avatar));
            CreateMap<FeedbackCreateViewModel, Feedback>();

            CreateMap<Cart, CartViewModel>()
                .ForMember(x => x.Items, e => e.MapFrom(x => x.CartItems))
                .ForMember(x => x.TotalPrice, e => e.MapFrom(x => x.CartItems.Sum(p => p.Product.Price * p.Quantity)));
            CreateMap<CartViewModel, Cart>();
            CreateMap<CartCreateViewModel, Cart>();
            CreateMap<CartUpdateViewModel, Cart>();

            CreateMap<CartItemViewModel, CartItem>();
            CreateMap<CartItemCreateViewModel, CartItem>();
            CreateMap<CartItemUpdateViewModel, CartItem>();
            CreateMap<CartItem, CartItemViewModel>()
                .ForMember(x => x.Product_Name, e => e.MapFrom(x => x.Product.Name))
                .ForMember(x => x.Product_Image, e => e.MapFrom(x => x.Product.Image))
                .ForMember(x => x.Product_Price, e => e.MapFrom(x => x.Product.Price))
                .ForMember(x => x.TotalItemPrice, e => e.MapFrom(x => x.Product.Price * x.Quantity));

            CreateMap<AppUser, AppUserViewModel>()
                .ForMember(x => x.Avartar, e => e.MapFrom(x => x.Avatar));

            CreateMap<Permissions, PermissionViewModel>();
            CreateMap<PermissionViewModel, Permissions>();
        }
    }
}
