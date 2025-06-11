using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datas.ViewModels.Payment.Vnpay
{
    public class VnpayConfig
    {
        public static string ConfigName => "Vnpay";
        public string Url { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string TMNCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string NotifyUrl { get; set; } = string.Empty;
    }
}
