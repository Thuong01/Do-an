using AutoMapper;
using Datas.Infrastructures.Interfaces;
using Datas.ViewModels;
using Datas.ViewModels.Order;
using Models.Enums;
using Models.Models;
using Datas.Extensions;
using Services.Interfaces.Repositories;
using Services.Interfaces.Services;
using Datas.Extentions;
using Commons.Commons;
using CloudinaryDotNet.Core;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using Datas.Data;

namespace Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICouponService _couponService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly INotificationRepository _notificationRepository;
        private readonly CameraNowContext _db;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository, 
            ICouponService couponService, 
            IUnitOfWork unitOfWork, 
            IProductService productService,
            ICartService cartService,
            INotificationRepository notificationRepository,
            CameraNowContext db,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _couponService = couponService;
            _unitOfWork = unitOfWork;
            _productService = productService;
            _cartService = cartService;
            _notificationRepository = notificationRepository;
            _db = db;
            _mapper = mapper;
        }

        // Hàm lấy dữ liệu transactions từ database
        public List<List<Guid>> GetTransactionData()
        {
            return _orderRepository.GetTransactionData().Result;
        }

        public async Task<int> CancelledOrder(Guid orderId, string userId)
        {
            var order = _orderRepository.GetSingleByCondition(x => x.User_Id == userId && x.ID == orderId, new[] { "OrderDetails" });

           order.Status = OrderStatusEnum.DaHuy;           

            return await _unitOfWork.CommitAsync();
        }

        public async Task<OrderViewModel> CreateAsync(OrderCreateViewModel create)
        {
            var order = _mapper.Map<Order>(create);
            var culture = CultureInfo.GetCultureInfo("vi-VN");

            decimal totalAmount = 0;
            foreach (var item in order.OrderDetails)
            {
                var book = await _productService.GetByIdAsync(item.Product_ID);
                totalAmount = book.Price * item.Quantity;
            }
            if (create.Coupon_Id != null)
            {
                var coupon = await _couponService.GetByIdAsync(order.Coupon_Id.Value);
                if (coupon != null)
                {
                    if (coupon.Type == CouponType.Percentage)
                    {
                        totalAmount = totalAmount * (coupon.Value / 100);
                    }
                    else if (coupon.Type == CouponType.FixedAmount)
                    {
                        totalAmount = totalAmount - coupon.Value;
                    }

                    var a = _db.Coupons.FirstOrDefault(x => x.Id == coupon.Id);
                    a.UsedCount += 1;

                    _db.UsedCoupons.Add(new UsedCoupon
                    {
                        CouponCode = coupon.Code,
                        UserId = create.User_Id,
                    });
                    _db.SaveChanges();
                }

            }
            order.Total_Amount = totalAmount;

            var res = _orderRepository.Add(order);
            var result = await _unitOfWork.CommitAsync();

            var orderParse = _mapper.Map<OrderViewModel>(order);

            if (result > 0)
            {
                foreach (var item in create.OrderDetails)
                {
                    if (create.FromCart_YN)
                    {
                        await _cartService.DeleteAsync(create.User_Id, create.CartID, item.Product_ID);
                    }
                    await _productService.UpdateBuyCount(item.Product_ID);
                    await _productService.UpdateQuantity(item.Product_ID, item.Quantity);
                }
            }
            else return null;

            // Thêm thông báo khi tạo đơn hàng mới
            _notificationRepository.Add(new Notification
            {
                Title = "Bạn có đơn hàng mới",
                Content = $"Có đơn đơn hàng #{orderParse.OrderNo} đã được đặt.",
                CreatedDate = DateTime.UtcNow,
                IsRead = false,
                User_ID = "8331b15b-d66a-4bce-ae04-acb18bb5927c",
                Role = "ADMIN"
            });
            await _unitOfWork.CommitAsync();

            #region send mail cho khachs hang
            var strSanPham = "";
            var thanhtien = decimal.Zero;
            var TongTien = decimal.Zero;
            foreach (var sp in orderParse.OrderDetails)
            {
                var product = await _productService.GetByIdAsync(sp.Product_ID);
                strSanPham += $@"<tr>
                                    <td>
                                        <div style=""display: flex; justify-content: left"">
                                            <div style=""margin-right: 20px"">
                                                <img width=""80"" height=""80"" src=""{product.Images.ToString()}"" alt=""{product.Name}"" />
                                            </div>
                                            <p>{product.Name}</p>
                                        </div>
                                    </td>
                                    <td style=""text-align: center"">{sp.Quantity}</td>
                                    <td>{(sp.Quantity * product.Price).ToString("C0", culture)}</td>
                                </tr>";
                thanhtien += product.Price * sp.Quantity;
            }

            TongTien = thanhtien;
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Commons", "Templates");

            // Đọc file send2.html
            string contentCustomer = await System.IO.File.ReadAllTextAsync(Path.Combine(basePath, "send2.html"));
            contentCustomer = contentCustomer.Replace("{{MaDon}}", orderParse.OrderNo.ToString());
            contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
            contentCustomer = contentCustomer.Replace("{{NgayDat}}", order.Order_Date.ToString("dd/MM/yyyy"));
            contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", orderParse.FullName.ToString());
            contentCustomer = contentCustomer.Replace("{{Phone}}", order.Phone);
            contentCustomer = contentCustomer.Replace("{{Email}}", order.Email);
            contentCustomer = contentCustomer.Replace("{{Message}}", order.Message);
            contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.Address);
            contentCustomer = contentCustomer.Replace("{{ThanhTien}}", thanhtien.ToString("C0", culture));
            contentCustomer = contentCustomer.Replace("{{TongTien}}", TongTien.ToString("C0", culture));
            CommonExtensions.SendMail(CommonConstant.AppName, @$"Đơn hàng của bạn", contentCustomer.ToString(), create.Email);

            // Đọc file send1.html
            string contentAdmin = await System.IO.File.ReadAllTextAsync(Path.Combine(basePath, "send1.html"));
            contentAdmin = contentAdmin.Replace("{{MaDon}}", orderParse.OrderNo.ToString());
            contentAdmin = contentAdmin.Replace("{{SanPham}}", strSanPham);
            contentAdmin = contentAdmin.Replace("{{NgayDat}}", order.Order_Date.ToString("dd/MM/yyyy"));
            contentAdmin = contentAdmin.Replace("{{TenKhachHang}}", orderParse.FullName.ToString());
            contentAdmin = contentAdmin.Replace("{{Phone}}", order.Phone);
            contentAdmin = contentAdmin.Replace("{{Email}}", create.Email);
            contentAdmin = contentAdmin.Replace("{{Message}}", order.Message);
            contentAdmin = contentAdmin.Replace("{{DiaChiNhanHang}}", order.Address);
            contentAdmin = contentAdmin.Replace("{{ThanhTien}}", thanhtien.ToString());
            contentAdmin = contentAdmin.Replace("{{TongTien}}", TongTien.ToString());
            CommonExtensions.SendMail(CommonConstant.AppName, @$"Bạn có đơn hàng mới #{orderParse.OrderNo}", contentAdmin.ToString(), CommonExtensions.Email);

            #endregion

            return orderParse;
        }

        public async Task<PaginationSet<OrderViewModel>> GetAllOrdersAsync(PaginatedParams pageParams, string[] includes = null)
        {
            var orders = await _orderRepository.GetAllAsync(includes);

            var paginationList = PaginatedList<Order>.Create(orders.AsQueryable(), pageParams.PageNumber, pageParams.PageSize);

            var paginationListVM = _mapper.Map<PaginatedList<OrderViewModel>>(paginationList);

            foreach (var item in paginationListVM)
            {
                if (item.Coupon_Id.HasValue)
                {
                    var coupon = await _couponService.GetByIdAsync(item.Coupon_Id.Value);

                    if (coupon != null)
                    {
                        item.CouponPercent = coupon.Value;
                    }
                }
            }

            return new PaginationSet<OrderViewModel>(pageParams.PageNumber, paginationListVM.TotalPages, paginationListVM.TotalCount, paginationListVM);
        }

        public async Task<OrderViewModel> GetOrderById(Guid orderId, string userId, string[] includes = null)
        {
            var order = _orderRepository.GetSingleByCondition(x => x.User_Id == userId && x.ID == orderId, includes);

            return _mapper.Map<OrderViewModel>(order);
        }

        public async Task<PaginationSet<OrderViewModel>> GetOrdersUserAsync(string userId, PaginatedParams pageParams, OrderStatusEnum status, string[] includes = null, string keyword = null)
        {
            var orders = await _orderRepository.GetOrdersUserAsync(userId, status, includes, keyword);
            var paginationList = PaginatedList<Order>.Create(orders.AsQueryable(), pageParams.PageNumber, pageParams.PageSize);
            var paginationListVM = _mapper.Map<PaginatedList<OrderViewModel>>(paginationList);

            foreach (var item in paginationListVM)
            {
                if (item.Coupon_Id.HasValue)
                {
                    var coupon = await _couponService.GetByIdAsync(item.Coupon_Id.Value);

                    if (coupon != null)
                    {
                        item.CouponPercent = coupon.Value;
                    }
                }
            }

            return new PaginationSet<OrderViewModel>(pageParams.PageNumber, paginationListVM.TotalPages, paginationListVM.TotalCount, paginationListVM.OrderByDescending(x => x.Order_Date));
        }

        public async Task<int> UpdateOrderStatus(Guid orderId, string userId, OrderStatusEnum status)
        {
            var order = _orderRepository.GetSingleByCondition(x => x.ID == orderId && x.User_Id == userId);

            if (order == null)
            {
                return 0;
            }

            order.Status = status;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> UpdateOrderIsCommented(Guid orderId, string userId)
        {
            var order = _orderRepository.GetSingleByCondition(x => x.ID == orderId && x.User_Id == userId);

            if (order == null)
            {
                return 0;
            }
            order.IsCommented = true;

            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> IsPaymentOrder(Guid orderId, string userId, string transactionNo)
        {
            var order = _orderRepository.GetSingleByCondition(x => x.ID == orderId && x.User_Id == userId);
            if (order == null)  return 0;
            order.TransactionNo = transactionNo;
            return await _unitOfWork.CommitAsync();
        }
    }
}
