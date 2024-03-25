using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Services.Interfaces.Transaction;
using Services.Utilities;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Transaction
{
    public class MomoServices : IMomoServices
    {
        private readonly IOptions<MomoModel> _options;

        public MomoServices(IOptions<MomoModel> options)
        {
            _options = options;
        }

        public string MomoDeposit(TransactionPoint transactionViaMomo, Guid currentUserId)
        {
            var orderId = Guid.NewGuid().ToString();
            var requestId = orderId;
            string rawHash = "accessKey=" + _options.Value.AccessKey +
              "&amount=" + transactionViaMomo.PointValue +
              "&extraData=" + currentUserId +
              "&ipnUrl=" + _options.Value.NotifyUrl +
              "&orderId=" + orderId +
              "&orderInfo=" + "Nạp điểm" +
              "&partnerCode=" + _options.Value.PartnerCode +
              "&redirectUrl=" + _options.Value.ReturnUrl +
              "&requestId=" + requestId +
              "&requestType=" + _options.Value.RequestType
              ;

            string signature = MomoUtils.ComputeHmacSha256(rawHash, _options.Value.SecretKey);

            JObject message = new()
            {
                { "partnerCode", _options.Value.PartnerCode },
                { "partnerName", "FlashShift" },
                { "storeId", "FlashShift" },
                { "requestId", requestId },
                { "amount", transactionViaMomo.PointValue },
                { "orderId", orderId },
                { "orderInfo", "Nạp điểm" },
                { "redirectUrl", _options.Value.ReturnUrl },
                { "ipnUrl", _options.Value.NotifyUrl },
                { "lang", "en" },
                { "extraData", currentUserId },
                { "requestType", _options.Value.RequestType },
                { "signature", signature }
            };
            string responseFromMomo = SendMoMoRequest(message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);
            return jmessage.GetValue("payUrl").ToString();
        }

        public bool VerifyMomoCallback(CallbackViaMomo callbackViaMomo)
        {
            var secretKey = _options.Value.SecretKey;
            var accessKey = _options.Value.AccessKey;

            string rawHash = "accessKey=" + accessKey +
              "&amount=" + callbackViaMomo.Amount +
              "&extraData=" + callbackViaMomo.ExtraData +
              "&message=" + callbackViaMomo.Message +
              "&orderId=" + callbackViaMomo.OrderId +
              "&orderInfo=" + callbackViaMomo.OrderInfo +
              "&orderType=" + callbackViaMomo.OrderType +
              "&partnerCode=" + callbackViaMomo.PartnerCode +
              "&payType=" + callbackViaMomo.PayType +
              "&requestId=" + callbackViaMomo.RequestId +
              "&responseTime=" + callbackViaMomo.ResponseTime +
              "&resultCode=" + callbackViaMomo.ResultCode +
              "&transId=" + callbackViaMomo.TransId
              ;

            string signature = MomoUtils.ComputeHmacSha256(rawHash, secretKey);
            if (signature == callbackViaMomo.Signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string SendMoMoRequest(string postJsonString)
        {
            try
            {
                var endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";

                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(endpoint);

                var postData = postJsonString;
                var data = Encoding.UTF8.GetBytes(postData);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/json";

                httpWReq.ContentLength = data.Length;
                httpWReq.ReadWriteTimeout = 30000;
                httpWReq.Timeout = 15000;
                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();

                string jsonresponse = "";

                using (var reader = new StreamReader(response.GetResponseStream()))
                {

                    string temp = null;
                    while ((temp = reader.ReadLine()) != null)
                    {
                        jsonresponse += temp;
                    }
                }

                return jsonresponse;
            }
            catch (WebException e)
            {
                return e.Message;
            }
        }

    }
}
