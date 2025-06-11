using Datas.ViewModels.Payment.Commons;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Datas.ViewModels.Payment.Momo
{
    public class MomoPaymentRequest
    {

        public string? Amount { get; set; }

        public string? OrderId { get; set; }

        public string? OrderInfo { get; set; }
    }
}
