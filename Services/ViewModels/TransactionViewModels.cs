using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class TransactionViewModels
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public int PointValue { get; set; }
        public string? TransactionCode { get; set; }
        public int Type { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class MomoPaymentResponse
    {
        public string RequestId { get; set; }
        public int ErrorCode { get; set; }
        public string Message { get; set; }
        public string LocalMessage { get; set; } 
        public string RequestType { get; set; }
        public string PayUrl { get; set; }
        public string Signature { get; set; }
        public string QrCodeUrl { get; set; }
        public string Deeplink { get; set; }
        public string DeeplinkWebInApp { get; set; }
    }

    public class MomoModel
    {
        public string MomoApiUrl { get; set; }
        public string SecretKey { get; set; }
        public string AccessKey { get; set; }
        public string ReturnUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string PartnerCode { get; set; }
        public string RequestType { get; set; }
    }

    public class TransactionResponse
    {
        public Guid TransactionId { get; set; }
        public int PointValue { get; set; }
        public string? TransactionCode { get; set; }
    }

    public class TransactionViaMomo
    {
        public int PointValue { get; set; }
    }

    public class MomoOneTimePaymentResultRequest
    {
        public string partnerCode { get; set; } = "";
        public string requestId { get; set; } = "";
        public int amount { get; set; }
        public string orderId { get; set; } = "";
        public string orderInfo { get; set; } = "";
        public string redirectUrl { get; set; } = "";
        public string ipnUrl { get; set; } = "";
        public string requestType { get; set; } = "";
        public string extraData { get; set; } = "";
        public string lang { get; set; } = "vi";
        public string signature { get; set; } = "";

    }

    public class CallbackViaMomo
    {
        public string PartnerCode { get; set; }

        public string OrderId { get; set; }
        public string RequestId { get; set; }
        public long Amount { get; set; }
        public string OrderInfo { get; set; }
        public string OrderType { get; set; }
        public long TransId { get; set; }
        public string ResultCode { get; set; }
        public string Message { get; set; }
        public string PayType { get; set; }
        public long ResponseTime { get; set; }
        public string ExtraData { get; set; }
        public string Signature { get; set; }
    }

}
