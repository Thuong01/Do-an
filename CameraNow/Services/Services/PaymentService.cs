using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using Services.Interfaces.Services;
using Datas.ViewModels.Payment.Momo;
using Datas.ViewModels.Payment.Vnpay;
using Datas.ViewModels.Payment.Commons;
using Models.Enums;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Models.Models;
using System.Net.Http;
using Services.Interfaces.Repositories;
using System.Security.Cryptography;

namespace BookStore.Bussiness.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly MomoConfig _momoConfig;
        private readonly VnpayConfig _vnpayConfig;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public PaymentService(IOptions<MomoConfig> momoConfigOptions, IOptions<VnpayConfig> vnpayConfigOptions, 
                            IOrderService orderService,
                            IMapper mapper)
        {
            _momoConfig = momoConfigOptions.Value;
            _orderService = orderService;
            _mapper = mapper;
            _vnpayConfig = vnpayConfigOptions.Value;
        }

        public async Task<string> SendMoMoPaymentRequestAsync(string endpoint, string postJsonString)
        {
            try
            {
                using HttpClient client = new HttpClient();
                var content = new StringContent(postJsonString, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(endpoint, content);

                response.EnsureSuccessStatusCode();

                var test = await response.Content.ReadAsStringAsync();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Request failed: {e.Message}");
            }
        }

        public async Task<(bool, string?)> GetLinkMoMoPaymentAsync(string paymentUrl, string request)
        {
            using HttpClient client = new HttpClient();

            var requestContent = new StringContent(request, System.Text.Encoding.UTF8, "application/json");

            var createPaymentLinkRes = await client.PostAsync(paymentUrl, requestContent);

            if (createPaymentLinkRes.IsSuccessStatusCode)
            {
                var responseContent = createPaymentLinkRes.Content.ReadAsStringAsync().Result;
                var responseData = JsonConvert.DeserializeObject<MoMoCreateLinkResponse>(responseContent);

                if (responseData.ResultCode == "")
                {
                    return (true, responseData.PayUrl);
                }
                else
                {
                    return (false, responseData.Message);
                }
            }
            else
            {
                return (false, createPaymentLinkRes.ReasonPhrase);
            }
        }

        public async Task<PaymentResultViewModel> HandleMoMoPaymentResultAsync(MomoPaymentResult resultDto, string userId)
        {
            var resultData = new PaymentResultViewModel();
            var isValidSignature = resultDto.IsValidSignature(_momoConfig.AccessKey, _momoConfig.SecretKey);

            if (isValidSignature)
            {
                string orderId = resultDto.OrderId.Split("_")[0];

                // Kiểm tra kết quả thanh toán
                if (resultDto.ResultCode == 0) // 0 là mã cho thanh toán thành công
                {

                    await _orderService.UpdateOrderStatus(_mapper.Map<Guid>(orderId), userId, OrderStatusEnum.DaThanhToan);

                    resultData.Amount = resultDto.Amount;
                    resultData.PaymentMessage = resultDto.Message;
                    resultData.PaymentStatus = "00";
                    resultData.PaymentId = resultDto.OrderId;
                    resultData.Signature = Guid.NewGuid().ToString();
                }
                else
                {
                    await _orderService.UpdateOrderStatus(_mapper.Map<Guid>(orderId), userId, OrderStatusEnum.DaThanhToan);

                    resultData.PaymentStatus = "10";
                    resultData.PaymentMessage = "Payment process failed";
                }
            }
            else
            {
                resultData.PaymentStatus = "99";
                resultData.PaymentMessage = "Invalid signature in response";
            }

            return resultData;
        }

        public async Task<PaymentResultViewModel> HandleVnpayPaymentResultAsync(VnpayPaymentResponse resultDto, string userId)
        {
            //{
            //    "vnp_Amount": "39000000",
            //    "vnp_BankCode": "NCB",
            //    "vnp_BankTranNo": "VNP14974169",
            //    "vnp_CardType": "ATM",
            //    "vnp_OrderInfo": "Thanh toán 390000 cho đơn hàng 250523X6BBKIK9 có ID: fc584175-d8be-4c2b-9fa0-cc4d1f9b53b7",
            //    "vnp_PayDate": "20250523004003",
            //    "vnp_ResponseCode": "00",
            //    "vnp_TmnCode": "XCWKVEYJ",
            //    "vnp_TransactionNo": "14974169",
            //    "vnp_TransactionStatus": "00",
            //    "vnp_TxnRef": "fc584175-d8be-4c2b-9fa0-cc4d1f9b53b7",
            //    "vnp_SecureHash": "35d12945db51b37130bd707d219beb2970ea8abb0638313cfbf18aac15302fc352a90f964fe91a833836f3becf1a57d563c874b9ad61e0be30d78dd1e902a592"
            //}

            string returnUrl = string.Empty;
            var resultData = new PaymentResultViewModel();
            var isValidSignature = resultDto.IsValidSignature(_vnpayConfig.HashSecret);

            if (isValidSignature)
            {
                string orderId = Regex.Match(resultDto.vnp_OrderInfo, @"[a-fA-F0-9]{8}-([a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}").Value;

                if (resultDto.vnp_ResponseCode == "00")
                {
                    await _orderService.UpdateOrderStatus(_mapper.Map<Guid>(orderId), userId, OrderStatusEnum.DaThanhToan);
                    resultData.PaymentStatus = "00";
                    resultData.PaymentId = orderId;
                    resultData.Signature = Guid.NewGuid().ToString();
                    resultData.PaymentMessage = "Payment successful";

                    var orderID = resultDto.vnp_TxnRef;
                    var res = await _orderService.IsPaymentOrder(_mapper.Map<Guid>(orderID), userId, resultDto.vnp_TransactionNo);
                }
                else
                {
                    resultData.PaymentStatus = "10";
                    resultData.PaymentMessage = "Payment process failed";
                }
            }
            else
            {
                resultData.PaymentStatus = "99";
                resultData.PaymentMessage = "Invalid signature in response";
            }

            return resultData;
        }

        //public async Task<RefundResult> RefundAsync(Order order)
        //{
            //// Tạo request hoàn tiền theo API VNPay
            //var requestData = new Dictionary<string, string>
            //{
            //    ["vnp_RequestId"] = Guid.NewGuid().ToString(),
            //    ["vnp_Version"] = "2.1.0",
            //    ["vnp_Command"] = "refund",
            //    ["vnp_TmnCode"] = _vnpayConfig.TMNCode,
            //    ["vnp_TransactionType"] = "02", // Hoàn tiền toàn phần
            //    ["vnp_TxnRef"] = "", //order.VnPayTransactionId,
            //    ["vnp_Amount"] = (order.Total_Amount * 100).ToString(), // VNPay yêu cầu amount là số tiền nhân 100
            //    ["vnp_TransactionNo"] = "", // order.VnPayTransactionNo,
            //    ["vnp_TransactionDate"] = order.Order_Date.ToString("yyyyMMddHHmmss"),
            //    ["vnp_CreateBy"] = "system", // Hoặc username admin thực hiện
            //    ["vnp_CreateDate"] = DateTime.Now.ToString("yyyyMMddHHmmss"),
            //    ["vnp_IpAddr"] = "127.0.0.1", // IP server gọi API
            //    ["vnp_OrderInfo"] = $"Hoan tien don hang {order.OrderNo}"
            //}; 

            //// Tạo chuỗi hash
            //var dataHash = string.Join("|", requestData.OrderBy(x => x.Key).Select(x => x.Value));
            //var secureHash = HashHMACSHA512(_vnpayConfig.HashSecret, dataHash);

            //requestData.Add("vnp_SecureHash", secureHash);

            //// Gọi API hoàn tiền của VNPay
            //var response = await _httpClient.PostAsync(_vnPaySettings.RefundUrl,
            //    new FormUrlEncodedContent(requestData));

            //if (!response.IsSuccessStatusCode)
            //{
            //    return new RefundResult { Success = false, Message = "Failed to connect to VNPay" };
            //}

            //var responseContent = await response.Content.ReadAsStringAsync();
            //var responseData = ParseResponseData(responseContent);

            //// Kiểm tra kết quả trả về
            //if (responseData["vnp_ResponseCode"] == "00")
            //{
            //    return new RefundResult { Success = true, Message = "Refund successful" };
            //}
            //else
            //{
            //    return new RefundResult
            //    {
            //        Success = false,
            //        Message = $"Refund failed. Error: {responseData["vnp_Message"]}"
            //    };
            //}
        //}

        private Dictionary<string, string> ParseResponseData(string response)
        {
            // Phân tích response từ VNPay (thường là query string hoặc JSON)
            // Tùy thuộc vào API thực tế của VNPay
            return response.Split('&')
                .Select(x => x.Split('='))
                .ToDictionary(x => x[0], x => x.Length > 1 ? x[1] : "");
        }

        private string HashHMACSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        //public Task<VNPayRefundResponse> RefundAsync(VNPayRefundRequest request)
        //{
        //    var vnpay = new SortedList<string, string>
        //    {
        //        ["vnp_RequestId"] = Guid.NewGuid().ToString(),
        //        ["vnp_Version"] = _vnpayConfig.Version,
        //        //["vnp_Command"] = _vnpayConfig.Command,
        //        ["vnp_TmnCode"] = _vnpayConfig.TMNCode,
        //        ["vnp_TransactionType"] = "02", // Hoàn tiền toàn phần
        //        ["vnp_TxnRef"] = request.TransactionId,
        //        ["vnp_Amount"] = request.Amount.ToString(),
        //        ["vnp_OrderInfo"] = $"Hoan tien GD {request.TransactionId}",
        //        ["vnp_TransactionNo"] = request.VnpTransactionNo,
        //        ["vnp_TransactionDate"] = request.TransactionDate.ToString("yyyyMMddHHmmss"),
        //        ["vnp_CreateBy"] = request.CreatedBy,
        //        ["vnp_CreateDate"] = DateTime.Now.ToString("yyyyMMddHHmmss"),
        //        ["vnp_IpAddr"] = request.IpAddress,
        //    };

        //    // Tạo chuỗi hash
        //    var signData = string.Join("&", vnpay.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        //    var secureHash = HashHMACSHA512(_vnpayConfig.HashSecret, signData);

        //    vnpay.Add("vnp_SecureHash", secureHash);

        //    // Gọi API hoàn tiền của VNPay
        //    var httpClient = _httpClientFactory.CreateClient();
        //    var content = new FormUrlEncodedContent(vnpay);
        //    var response = await httpClient.PostAsync(_vnpayConfig.RefundUrl, content);
        //}
    }
}
