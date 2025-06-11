using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datas.ViewModels.Payment.Momo
{
    public class MoMoCreateLinkResponse
    {
        public string PartnerCode { get; set; }

        public string RequestId { get; set; }

        public string? Amount { get; set; }

        public string? OrderId { get; set; }

        public long ResponseTime { get; set; }

        public string Message { get; set; }

        public string ResultCode { get; set; }

        public string PayUrl { get; set; }

        public string DeepLink { get; set; }

        public string QRCodeUrl { get; set; }
    }
}
