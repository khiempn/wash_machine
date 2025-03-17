using System.Collections.Generic;
using System.Linq;

namespace WashMachine.Forms.Modules.PaidBy.Machine.Octopus
{
    public enum OctopusPaymentStatus
    {

        SUCCESS = 3,
        FAILURE = 2,
        MANUAL_TIMEOUT = 1
    }

    public class OctopusPaymentResponseModel
    {
        List<ResponseCodeModel> responseCodes = new List<ResponseCodeModel>()
        {
            new ResponseCodeModel() { Code = 100001,  Cn_Message = "機器故障。請與本店職員聯絡", En_Message = "Machine out of order. Please contact staff for assistance" },
            new ResponseCodeModel() { Code = 100003,  Cn_Message = "", En_Message = "Invalid parameters" },
            new ResponseCodeModel() { Code = 100005,  Cn_Message = "未能接駁八達通收費器", En_Message = "MOP connection failure" },
            new ResponseCodeModel() { Code = 100016,  Cn_Message = "讀卡錯誤,請重試", En_Message = "Read card error, retry please" },
            new ResponseCodeModel() { Code = 100017,  Cn_Message = "讀卡錯誤,請重試", En_Message = "Read card error, retry please" },
            new ResponseCodeModel() { Code = 100019,  Cn_Message = "此卡失效。請使用另一張八達通卡",  En_Message = "Invalid Octopus. Please use another Octopus" },
            new ResponseCodeModel() { Code = 100020,  Cn_Message = "請再次拍卡", En_Message = "Please tap card again" },
            new ResponseCodeModel() { Code = 100021,  Cn_Message = "此八達通卡或產品已失效,\r\n請聯絡港鐵客務中心",  En_Message = "Invalid Octopus, please contact MTR Customer\r\nService Centre" },
            new ResponseCodeModel() { Code = 100022,  Cn_Message = "交易未能完成。請再次拍卡",  En_Message = "Incomplete transaction. Please retry with the same Octopus" },
            new ResponseCodeModel() { Code = 100023,  Cn_Message = "此卡失效。請使用另一張八達通卡", En_Message = "Transaction Log full" },
            new ResponseCodeModel() { Code = 100024,  Cn_Message = "此卡失效。請使用另一張八達通卡", En_Message = "Invalid Octopus. Please use another Octopus" },
            new ResponseCodeModel() { Code = 100025,  Cn_Message = "交易未能完成。請再次拍卡",  En_Message = "Incomplete transaction. Please retry with the same Octopus" },

            new ResponseCodeModel() { Code = 100032,  Cn_Message = "請再次拍卡", En_Message = "Please tap card again" },
            new ResponseCodeModel() { Code = 100033,  Cn_Message = "請再次拍卡", En_Message = "The transaction amount is different from previous incomplete transaction." },
            new ResponseCodeModel() { Code = 100034,  Cn_Message = "讀卡錯誤,請重試",  En_Message = "Read card error, retry please" },
            new ResponseCodeModel() { Code = 100035,  Cn_Message = "此卡失效。請使用另一張八達通卡", En_Message = "Invalid Octopus. Please use another Octopus" },
            new ResponseCodeModel() { Code = 100048,  Cn_Message = "卡內餘額不足,交易取消。請先行 增值", En_Message = "Insufficient value on card. Transaction cancelled. Please add value first" },
            new ResponseCodeModel() { Code = 100049,  Cn_Message = "卡上儲值額超出上限,請使用另一張八達通卡", En_Message = "Stored value on card exceeds limit. Please use another Octopus" },
            new ResponseCodeModel() { Code = 100051,  Cn_Message = "控制台識別號碼不正確", En_Message = "Invalid POS controller ID" },
            new ResponseCodeModel() { Code = 100066,  Cn_Message = "", En_Message = "System time error" },

            new ResponseCodeModel() { Code = 100099,  Cn_Message = "", En_Message = "Firmware upgrade has performed due to HouseKeeping()" },
            new ResponseCodeModel() { Code = 100101,  Cn_Message = "", En_Message = "Failed to create AR (SaveLog)" },
            new ResponseCodeModel() { Code = 100102,  Cn_Message = "", En_Message = "Failed to create UD (SaveLog)" },

            new ResponseCodeModel() { Code = -100022, Cn_Message = "請重試(八達通號碼 88888888)", En_Message = "Retry please (Octopus no. 88888888)" },
            new ResponseCodeModel() { Code = int.MinValue, Cn_Message = "", En_Message = "Please setup Octopus equipment for this device" },
            new ResponseCodeModel() { Code = int.MaxValue, Cn_Message = "發生錯誤(編號 999999)", En_Message = "Error 999999" },
            new ResponseCodeModel() { Code = (int)OctopusPaymentStatus.SUCCESS, Cn_Message = "付款成功!", En_Message = "Payment successfully!" },
            new ResponseCodeModel() { Code = (int)OctopusPaymentStatus.FAILURE, Cn_Message = "", En_Message = "Can not completed payment!" },
            new ResponseCodeModel() { Code = (int)OctopusPaymentStatus.MANUAL_TIMEOUT, Cn_Message = "", En_Message = "Timeout when waiting scan card." },
        };


        public OctopusPaymentResponseModel()
        {

        }

        public bool Status { get; set; }
        public CardInfo CardInfo { get; set; }
        public string Message { get; set; }
        public int Rs { get; set; }
        public List<int> MessageCodes { get; set; }
        public bool IsStop { get; internal set; }

        /// <summary>
        /// Specific Scenario If the card detected is not the same card during retry on incomplete transaction (100022)
        /// </summary>
        /// <param name="isSpecificScenario"></param>
        /// <returns></returns>
        public string GetMessage(int rs)
        {
            var res = responseCodes.FirstOrDefault(w => w.Code == rs);
            if (res == null)
            {
                var resCode = responseCodes.First(w => w.Code == int.MaxValue);
                return $"{resCode.Cn_Message}\n{resCode.En_Message}";
            }
            else
            {
                return $"{res.Cn_Message}\n{res.En_Message}";
            }
        }

        public string GetMessageDefault()
        {
            var resCode = responseCodes.First(w => w.Code == 100001);
            return $"ERROR CODE: 100001\n{resCode.Cn_Message}\n{resCode.En_Message}";
        }
    }
}
