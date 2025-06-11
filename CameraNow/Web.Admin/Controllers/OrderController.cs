using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Datas.Extensions;
using Services.Interfaces.Services;
using Models.Models;
using System.Drawing.Imaging;
using QRCoder;
using Services.Services;
using System.Text.RegularExpressions;
using System.Globalization;
using DinkToPdf;
using DinkToPdf.Contracts;
using Datas.Extensions.Responses;

namespace Web.Admin.Controllers
{
    public class OrderController : BaseController
    {
        private readonly HttpClient _httpClient;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;
        private readonly IProductService _productService;
        private readonly IConverter _converter;

        public OrderController(HttpClient httpClient, IOrderService orderService, IMapper mapper, ILogger<OrderController> logger,
                            IProductService productService,
                            IConverter converter)
        {
            _httpClient = httpClient;
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
            _productService = productService;
            _converter = converter;
        }

        public async Task<IActionResult> Index(string? filter, [Bind(Prefix = "page")] int page, string sort = "name")
        {
            try
            {
                //var apiUrl = "https://localhost:7089/api/OrderAPIs/all-orders";

                //HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                //if (response.IsSuccessStatusCode)
                //{
                //    var data = await response.Content.ReadAsStringAsync();

                //    ViewBag.Orders = data;
                //}
                //else
                //{
                //    // Handle error
                //    ViewBag.Error = "Error retrieving data from API";
                //}

                ViewData["Current_Filter"] = filter;

                if (page == 0) page = 1;
                var pageParams = new PaginatedParams(page, 10);
                var res = await _orderService.GetAllOrdersAsync(pageParams, new[] { "OrderDetails", "User", "OrderDetails.Product" });

                ViewBag.PageNumber = page;
                ViewBag.TotalPages = res.TotalPage;
                ViewBag.TotalCount = res.TotalCount;

                return View(res.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View(ex.Message); // Return an error view
            }
        }

        public async Task<IActionResult> Details(Guid orderId, string userId)
        {
            try
            {
                var res = await _orderService.GetOrderById(_mapper.Map<Guid>(orderId), userId, new[] { "OrderDetails", "User", "OrderDetails.Product" });

                return View(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View(nameof(Index));
            }
        }

        public async Task<IActionResult> PrintInvoice(Guid orderId, string userId)
        {
            try
            {
                var order = await _orderService.GetOrderById(_mapper.Map<Guid>(orderId), userId, new[] { "OrderDetails", "User", "OrderDetails.Product" });
                var culture = CultureInfo.GetCultureInfo("vi-VN");

                if (order == null)
                {
                    return NotFound();
                }

                //// 2. Tạo QR Code từ mã đơn hàng
                //using var qrGenerator = new QRCodeGenerator();
                //using var qrData = qrGenerator.CreateQrCode(order.OrderCode, QRCodeGenerator.ECCLevel.Q);
                //using var qrCode = new QRCode(qrData);
                //using var qrImage = qrCode.GetGraphic(20);
                //using var ms = new MemoryStream();
                //qrImage.Save(ms, ImageFormat.Png);
                //var qrBase64 = Convert.ToBase64String(ms.ToArray());

                // 4. Render HTML từ View
                //string htmlContent = RenderViewToString("Invoice", model);

                //send mail cho khachs hang
                var strSanPham = "";
                var thanhtien = decimal.Zero;
                var TongTien = decimal.Zero;
                foreach (var sp in order.OrderDetails)
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
                string htmlContent = await System.IO.File.ReadAllTextAsync(Path.Combine(basePath, "Invoice.html"));
                htmlContent = htmlContent.Replace("{{ShippingCode}}", "GIAOHANGNHANH");
                htmlContent = htmlContent.Replace("{{OrderNo}}", order.OrderNo.ToString());
                htmlContent = htmlContent.Replace("{{thongtinsanpham}}", strSanPham);
                //htmlContent = htmlContent.Replace("{{NgayDat}}", order.Order_Date.ToString("dd/MM/yyyy"));
                htmlContent = htmlContent.Replace("{{FullName}}", order.FullName.ToString());
                htmlContent = htmlContent.Replace("{{Address}}", order.Address);
                htmlContent = htmlContent.Replace("{{PhoneNumber}}", order.Phone);
                //htmlContent = htmlContent.Replace("{{Email}}", order.Email);
                htmlContent = htmlContent.Replace("{{TotalItems}}", order.OrderDetails.Count().ToString());
                htmlContent = htmlContent.Replace("{{Total_Amount}}", thanhtien.ToString("C0", culture));
                htmlContent = htmlContent.Replace("{{TongTien}}", TongTien.ToString("C0", culture));
                htmlContent = htmlContent.Replace("{{Weight}}", "50KG");

                // 5. Convert HTML to PDF
                var doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        PaperSize = PaperKind.A4,
                        Orientation = Orientation.Portrait
                    },
                    Objects = {
                        new ObjectSettings {
                            HtmlContent = htmlContent
                        }
                    }
                };

                byte[] pdf = _converter.Convert(doc);
                return File(pdf, "application/pdf", $"HoaDon_{order.OrderNo}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeOrderStatus(string orderId, string userId, OrderStatusEnum status)
        {
            try
            {
                var res = await _orderService.UpdateOrderStatus(_mapper.Map<Guid>(orderId), userId, status);
                if (res == 0)
                {
                    return new JsonResult(new { success = false, message = "Error updating order status" });
                }
                return new JsonResult(new { success = true, message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new JsonResult(new { success = false, message = "Error updating order status" });
            }
        }
    }
}
