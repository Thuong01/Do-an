using Datas.ViewModels.Payment.Commons;
using Datas.ViewModels.Payment.Momo;
using Datas.ViewModels.Payment.Vnpay;
using Models.Models;

namespace Services.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<(bool, string?)> GetLinkMoMoPaymentAsync(string paymentUrl, string request);
        Task<string> SendMoMoPaymentRequestAsync(string endpoint, string postJsonString);
        Task<PaymentResultViewModel> HandleMoMoPaymentResultAsync(MomoPaymentResult resultDto, string userId);
        Task<PaymentResultViewModel> HandleVnpayPaymentResultAsync(VnpayPaymentResponse resultDto, string userId);
        //Task<RefundResult> RefundAsync(Order order);

        //Task<VNPayRefundResponse> RefundAsync(VNPayRefundRequest request);

        //Task<string> SendVnpayPaymentRequestAsync();
    }    
}
