using System.Collections.Generic;
using System.Linq;

namespace WashMachine.Forms.Modules.PaidBy.Machine.Octopus
{
    public class OctopusPaymentResponseModel
    {
        List<ResponseCodeModel> responseCodes = new List<ResponseCodeModel>()
        {
            new ResponseCodeModel() { Code = 100016,  Cn_Message = "請再次拍卡",  En_Message = "Please tap card again" },
            new ResponseCodeModel() { Code = 100017,  Cn_Message = "請再次拍卡",  En_Message = "Please tap card again" },
            new ResponseCodeModel() { Code = 100020,  Cn_Message = "請再次拍卡",  En_Message = "Please tap card again" },
            new ResponseCodeModel() { Code = 100032,  Cn_Message = "請再次拍卡",  En_Message = "Please tap card again" },
            new ResponseCodeModel() { Code = 100034,  Cn_Message = "請再次拍卡",  En_Message = "Please tap card again" },

            new ResponseCodeModel() { Code = 100019,  Cn_Message = "此卡失效。請使用另一張八達通卡",  En_Message = "Invalid Octopus. Please use another Octopus" },
            new ResponseCodeModel() { Code = 100021,  Cn_Message = "此卡失效。請使用另一張八達通卡",  En_Message = "Invalid Octopus. Please use another Octopus" },
            new ResponseCodeModel() { Code = 100024,  Cn_Message = "此卡失效。請使用另一張八達通卡",  En_Message = "Invalid Octopus. Please use another Octopus" },
            new ResponseCodeModel() { Code = 100035,  Cn_Message = "此卡失效。請使用另一張八達通卡",  En_Message = "Invalid Octopus. Please use another Octopus" },

            new ResponseCodeModel() { Code = 100022,  Cn_Message = "交易未能完成。請再次拍卡",  En_Message = "Incomplete transaction. Please retry with the same Octopus" },

            new ResponseCodeModel() { Code = 100048, Cn_Message = "卡內餘額不足,交易取消。請先行 增值", En_Message = "Insufficient value on card. Transaction cancelled. Please add value first" },

            new ResponseCodeModel() { Code = 100049, Cn_Message = "卡上儲值額超出上限,請使用另一張八達通卡", En_Message = "Stored value on card exceeds limit. Please use another Octopus" },

            new ResponseCodeModel() { Code = 100001,  Cn_Message = "機器故障。請與本店職員聯絡", En_Message = "Machine out of order. Please contact staff for assistance" },
            new ResponseCodeModel() { Code = -2, Cn_Message = "請重試(八達通號碼 88888888)", En_Message = "Retry please (Octopus no. 88888888)" },
            new ResponseCodeModel() { Code = int.MinValue, Cn_Message = "", En_Message = "Please setup Octopus equipment for this device" },
        };

        public OctopusPaymentResponseModel()
        {

        }

        public bool Status { get; set; }
        public CardInfo CardInfo { get; set; }
        public string Message { get; set; }
        public int Rs { get; set; }
        public bool SpecificScenario { get; set; }

        /// <summary>
        /// Specific Scenario If the card detected is not the same card during retry on incomplete transaction (100022)
        /// </summary>
        /// <param name="isSpecificScenario"></param>
        /// <returns></returns>
        public string GetMessage(int rs, bool isSpecificScenario)
        {
            var res = responseCodes.FirstOrDefault(w => w.Code == rs);
            if (res == null)
            {
                var resCode = responseCodes.First(w => w.Code == 100001);
                return $"ERROR CODE:{rs}\n{resCode.Cn_Message}\n{resCode.En_Message}";
            }
            else
            {
                if (isSpecificScenario)
                {
                    var resCode = responseCodes.First(w => w.Code == -2);
                    return $"ERROR CODE:{rs}\n{res.Cn_Message}\n{res.En_Message}\n" + $"{resCode.Cn_Message}\n{resCode.En_Message}";
                }
                else
                {
                    return $"ERROR CODE:{rs}\n{res.Cn_Message}\n{res.En_Message}";
                }
            }
        }
    }
}
