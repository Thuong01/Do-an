using Datas.ViewModels.Payment.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Datas.ViewModels.Payment.Momo
{
    [BindProperties]
    public class MomoPaymentResult
    {
        public string? PartnerCode { get; set; } = string.Empty;
        public string? OrderId { get; set; } = string.Empty;
        public string? RequestId { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string? OrderInfo { get; set; } = string.Empty;
        public string? OrderType { get; set; } = string.Empty;
        public string? Message { get; set; } = string.Empty;
        public int ResultCode { get; set; }
        public string? PayType { get; set; } = string.Empty;
        public long ResponseTime { get; set; }
        public string? ExtraData { get; set; } = string.Empty;
        public string? Signature { get; set; } = string.Empty;

        public bool IsValidSignature(string accessKey, string secretKey)
        {
            var rawHash = "accessKey=" + accessKey +
                   "&amount=" + this.Amount +
                   "&extraData=" + this.ExtraData +
                   "&message=" + this.Message +
                   "&orderId=" + this.OrderId +
                   "&orderInfo=" + this.OrderInfo +
                   "&orderType=" + this.OrderType +
                   "&partnerCode=" + this.PartnerCode +
                   "&payType=" + this.PayType +
                   "&requestId=" + this.RequestId +
                   "&responseTime=" + this.ResponseTime +
                   "&resultCode=" + this.ResultCode;
            var checkSignature = PaymentHashSecurity.HmacSHA256(rawHash, secretKey);
            return this.Signature.Equals(checkSignature);
        }
    }
}
