using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Datas.ViewModels.Payment.Commons;
using Datas.ViewModels.Payment.Momo;
using Datas.ViewModels.Payment.Vnpay;
using Models.Models;
using Services.Interfaces.Services;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Macs;

namespace WebApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]s")]
    [Authorize]
    public class PaymentAPIController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly MomoConfig _momoConfig;
        private readonly VnpayConfig _vnpayConfig;

        public PaymentAPIController(IOptions<MomoConfig> momo, IOptions<VnpayConfig> vnpay, 
                                IPaymentService paymentService, 
                                IHttpContextAccessor httpContextAccessor, 
                                UserManager<AppUser> userManager)
        {
            _paymentService = paymentService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _momoConfig = momo.Value;
            _vnpayConfig = vnpay.Value;
        }

        [HttpPost]
        [Route("momo-payment")]
        public async Task<IActionResult> MoMoPaymentAsync([FromBody] MomoPaymentRequest input)
        {
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = _momoConfig.PartnerCode;
            string accessKey = _momoConfig.AccessKey;
            string secretKey = _momoConfig.SecretKey;
            string orderInfo = input.OrderInfo ?? "";
            string redirectUrl = _momoConfig.ReturnUrl;
            string ipnUrl = _momoConfig.IpnUrl;
            string requestType = "captureWallet";
            string amount = input.Amount ?? "";
            string orderId = input.OrderId ?? "";
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";

            var rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";

            string Signature = PaymentHashSecurity.HmacSHA256(rawHash, secretKey);

            JObject message = new JObject
                {
                    { "partnerCode", partnerCode },
                    { "partnerName", "Test" },
                    { "storeId", "MomoTestStore" },
                    { "requestId", requestId },
                    { "amount", amount },
                    { "orderId", orderId },
                    { "orderInfo", orderInfo },
                    { "redirectUrl", redirectUrl },
                    { "ipnUrl", ipnUrl },
                    { "lang", "en" },
                    { "extraData", extraData },
                    { "requestType", requestType },
                    { "signature", Signature }
                };

            string responseFromMomo = await _paymentService.SendMoMoPaymentRequestAsync(endpoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);

            jmessage.Remove("partnerCode");
            jmessage.Remove("orderId");
            jmessage.Remove("requestId");
            jmessage.Remove("responseTime");
            jmessage.Remove("amount");

            return Ok(jmessage);
        }

        [HttpGet]
        [Route("momo-return")]
        public async Task<IActionResult> MoMoReturn([FromQuery] MomoPaymentResult response)
        {
            return Ok(await _paymentService.HandleMoMoPaymentResultAsync(response, _userManager.GetUserId(User)));
        }

        /// <summary>          
        /// </summary>
        /// <param name="request">
        /// Thông tin thanh toán
        /// Ngân hàng       NCB
        /// Số thẻ	        9704198526191432198
        /// Tên chủ thẻ     NGUYEN VAN A
        /// Ngày phát hành	07/15
        /// Mật khẩu OTP	123456
        /// 
        /// 
        /// Thông tin truy cập Merchant Admin để quản lý giao dịch:        
        /// Địa chỉ: https://sandbox.vnpayment.vn/merchantv2/
        /// Tên đăng nhập: anh038953 @gmail.com 
        /// Mật khẩu: @Anh12062003
        /// </param>
        /// <returns>            
        /// </returns>
        [HttpPost]
        [Route("vnpay-payment")]
        public async Task<IActionResult> VnpayPayment([FromBody] OrderRequestInfo request)
        {
            // Lấy thông tin cấu hình VNPAY từ appsettings
            var vnpayUrl = _vnpayConfig.Url;
            var version = _vnpayConfig.Version;
            var tmnCode = _vnpayConfig.TMNCode;
            var hashSecret = _vnpayConfig.HashSecret;
            var returnUrl = _vnpayConfig.ReturnUrl;
            var notifyUrl = _vnpayConfig.NotifyUrl;
            var ipAddrr = _httpContextAccessor?.HttpContext?.Connection?.LocalIpAddress?.ToString();
            var requestData = new VnpayPaymentRequest(version, tmnCode, DateTime.Now, ipAddrr, decimal.Parse(request.Amount), "VND", "other", request.OrderInfo, returnUrl, request.OrderId);
            var paymentUrl = requestData.GetLink(vnpayUrl, hashSecret);
            return Ok(new { PayUrl = paymentUrl });
        }

        [HttpPost]
        [Route("vnpay-refund")]
        public async Task<IActionResult> VnpayRefund([FromBody] OrderRequestInfo request)
        {
            // Lấy thông tin cấu hình VNPAY từ appsettings
            var vnpayUrl = _vnpayConfig.Url;
            var version = _vnpayConfig.Version;
            var tmnCode = _vnpayConfig.TMNCode;
            var hashSecret = _vnpayConfig.HashSecret;
            var returnUrl = _vnpayConfig.ReturnUrl;
            var notifyUrl = _vnpayConfig.NotifyUrl;
            var ipAddrr = _httpContextAccessor?.HttpContext?.Connection?.LocalIpAddress?.ToString();
            var requestData = new VnpayPaymentRequest(version, tmnCode, DateTime.Now, ipAddrr, decimal.Parse(request.Amount), "VND", "other", request.OrderInfo, returnUrl, request.OrderId);
            var paymentUrl = requestData.GetLink(vnpayUrl, hashSecret);
            return Ok(new { PayUrl = paymentUrl });
        }

        [HttpGet]
        [Route("vnpay-return")]
        [AllowAnonymous]
        public async Task<IActionResult> VnpayReturn([FromQuery] VnpayPaymentResponse response)
        {
            return Ok(await _paymentService.HandleVnpayPaymentResultAsync(response, _userManager.GetUserId(User)));
        }
    }
}
