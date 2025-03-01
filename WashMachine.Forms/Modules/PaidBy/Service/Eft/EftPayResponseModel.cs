using EFTSolutions;
using System.Collections.Generic;
using System.Linq;

namespace WashMachine.Forms.Modules.PaidBy.Service.Eft
{
    public class EftPayResponseModel
    {
        List<ResponseCodeModel> responseCodes = new List<ResponseCodeModel>()
        {
            new ResponseCodeModel() { Code = "00",  Cn_Message = "",  En_Message = "Payment Successfully!" },
            new ResponseCodeModel() { Code = "01",  Cn_Message = "查詢支付方",  En_Message = "Enquire Payment" },
            new ResponseCodeModel() { Code = "39", Cn_Message = "無此帳戶", En_Message = "Invalid Account" },
            new ResponseCodeModel() { Code = "78", Cn_Message = "追蹤碼錯誤", En_Message = "Tracking Code Error Processor" },
            new ResponseCodeModel() { Code = "02",  Cn_Message = "查詢服務商", En_Message = "Enquire Service" },
            new ResponseCodeModel() { Code = "41", Cn_Message = "帳戶已凍結", En_Message = "Account Frozen" },
            new ResponseCodeModel() { Code = "79", Cn_Message = "無效帳戶", En_Message = "Invalid Account Providers" },
            new ResponseCodeModel() { Code = "03",  Cn_Message = "商戶代號錯誤", En_Message = "Invalid Merchant ID" },
            new ResponseCodeModel() { Code = "42", Cn_Message = "無此帳戶", En_Message = "Invalid Account" },
            new ResponseCodeModel() { Code = "80", Cn_Message = "數據錯誤", En_Message = "Data Error" },
            new ResponseCodeModel() { Code = "04",  Cn_Message = "未做實名認証", En_Message = "A/C Not Authenticated" },
            new ResponseCodeModel() { Code = "43", Cn_Message = "非法支付碼", En_Message = "Invalid Payment Code" },
            new ResponseCodeModel() { Code = "81", Cn_Message = "加密錯誤", En_Message = "Encryption Error" },
            new ResponseCodeModel() { Code = "05" , Cn_Message = "拒絕", En_Message = "   Declined" },
            new ResponseCodeModel() { Code = "51", Cn_Message = "餘額不足", En_Message = "Insufficient Fund" },
            new ResponseCodeModel() { Code = "83", Cn_Message = "密碼不能檢驗", En_Message = "Password Unauthorized" },
            new ResponseCodeModel() { Code = "06",  Cn_Message = "錯誤", En_Message = "Error" },
            new ResponseCodeModel() { Code = "54", Cn_Message = "支付碼已過期", En_Message = "Payment Code Expired" },
            new ResponseCodeModel() { Code = "84", Cn_Message = "未能連接支付方", En_Message = "Cannot Connect to Payment Processor" },
            new ResponseCodeModel() { Code = "09" , Cn_Message = "未開通無網絡支付", En_Message = "No Network Payment Not Authorized" },
            new ResponseCodeModel() { Code = "55", Cn_Message = "支付密碼錯", En_Message = "Incorrect Payment Password" },
            new ResponseCodeModel() { Code = "85", Cn_Message = "正常帳戶", En_Message = "Normal Account" },
            new ResponseCodeModel() { Code = "12", Cn_Message = "交易無效", En_Message = "Invalid Transaction" },
            new ResponseCodeModel() { Code = "57", Cn_Message = "服務不支持", En_Message = "Unsupported Service" },
            new ResponseCodeModel() { Code = "87", Cn_Message = "支付密碼處理錯", En_Message = "Incorrect Payment PW" },
            new ResponseCodeModel() { Code = "13", Cn_Message = "金額錯誤", En_Message = "Invalid Amount" },
            new ResponseCodeModel() { Code = "58", Cn_Message = "交易不允許", En_Message = "Transaction Not Processing" },
            new ResponseCodeModel() { Code = "88", Cn_Message = "網絡故障", En_Message = "Network Failure" },
            new ResponseCodeModel() { Code = "14", Cn_Message = "無效支付碼", En_Message = "Invalid Payment Code Allowed" },
            new ResponseCodeModel() { Code = "59", Cn_Message = "有作弊嫌疑", En_Message = "Fraud  Suspected" },
            new ResponseCodeModel() { Code = "89", Cn_Message = "終端代碼錯誤", En_Message = "Invalid Terminal Code" },
            new ResponseCodeModel() { Code = "19", Cn_Message = "超時-重做交易", En_Message = "Timeout – Retry Again" },
            new ResponseCodeModel() { Code = "60", Cn_Message = "產品限額校驗未通過", En_Message = "Exceed Of The" },
            new ResponseCodeModel() { Code = "90", Cn_Message = "系統備份", En_Message = "System Backup" },
            new ResponseCodeModel() { Code = "20", Cn_Message = "無效交易", En_Message = "Invalid Transaction Payment Threshold Of" },
            new ResponseCodeModel() { Code = "61", Cn_Message = "超限額", En_Message = "Exceed LimitOf" },
            new ResponseCodeModel() { Code = "91", Cn_Message = "支付方網絡錯誤", En_Message = "Payment Processor" },
            new ResponseCodeModel() { Code = "21", Cn_Message = "不作任何處理", En_Message = "No Processing" },
            new ResponseCodeModel() { Code = "62", Cn_Message = "服務代碼錯誤", En_Message = "Server Code Error Network Issue" },
            new ResponseCodeModel() { Code = "92", Cn_Message = "通訊錯誤", En_Message = "Communication Error" },
            new ResponseCodeModel() { Code = "22", Cn_Message = "懷疑操作有誤", En_Message = "Operation Issues" },
            new ResponseCodeModel() { Code = "63", Cn_Message = "支付碼校驗錯誤", En_Message = "Payment Code" },
            new ResponseCodeModel() { Code = "93", Cn_Message = "交易不能完成", En_Message = "Trans Not Completed" },
            new ResponseCodeModel() { Code = "23", Cn_Message = "不可接受的交易費", En_Message = "Unacceptable Trans Fee Verification Failed64 Incorrect Original Amount" },
            new ResponseCodeModel() { Code = "94", Cn_Message = "原始金額不正確", En_Message = "Duplicated Number" },
            new ResponseCodeModel() { Code = "25", Cn_Message = "序號重複", En_Message = "Record Not Found" },
            new ResponseCodeModel() { Code = "65", Cn_Message = "找不到原交易", En_Message = "Restricted" },
            new ResponseCodeModel() { Code = "95", Cn_Message = "限制使用", En_Message = "Daily Cut－Off – Pls Wait" },
            new ResponseCodeModel() { Code = "26", Cn_Message = "買家操作未完成", En_Message = "Buyer Operation Not Completed" },
            new ResponseCodeModel() { Code = "66", Cn_Message = "超當日限額", En_Message = "Exceed Daily Limit" },
            new ResponseCodeModel() { Code = "96", Cn_Message = "系統故障", En_Message = "Systems Error" },
            new ResponseCodeModel() { Code = "27", Cn_Message = "交易已成功", En_Message = "Txn Was Success" },
            new ResponseCodeModel() { Code = "67", Cn_Message = "超當月限額", En_Message = "Exceed Monthly Limit" },
            new ResponseCodeModel() { Code = "97", Cn_Message = "無此終端號", En_Message = "Invalid Terminal ID" },
            new ResponseCodeModel() { Code = "28", Cn_Message = "不要重試", En_Message = "Error – Do No Retry" },
            new ResponseCodeModel() { Code = "68", Cn_Message = "主機應答超時", En_Message = "Host Response  Timeout" },
            new ResponseCodeModel() { Code = "98", Cn_Message = "收不到支付方應答", En_Message = "No Response From Payment Processor" },
            new ResponseCodeModel() { Code = "29", Cn_Message = "交易修改失敗", En_Message = "Modification Failure" },
            new ResponseCodeModel() { Code = "69", Cn_Message = "超外匯兌換限額", En_Message = "Over Currency Exchange Limitation" },
            new ResponseCodeModel() { Code = "99", Cn_Message = "密碼加密格式錯", En_Message = "Invalid PW Encryption" },
            new ResponseCodeModel() { Code = "30", Cn_Message = "傳輸格式錯誤", En_Message = "Transmission Format Error" },
            new ResponseCodeModel() { Code = "70", Cn_Message = "超商户日交易限额", En_Message = "Merchant Daily quota exceed" },
            new ResponseCodeModel() { Code = "A0", Cn_Message = "MAC 校驗錯", En_Message = "MAC  Error" },
            new ResponseCodeModel() { Code = "31", Cn_Message = "網絡中斷", En_Message = "Network Interruption" },
            new ResponseCodeModel() { Code = "75", Cn_Message = "支付密碼錯超限", En_Message = "Payment Password" },
            new ResponseCodeModel() { Code = "N0", Cn_Message = "找不到原交易", En_Message = "Original Trans Not" },
            new ResponseCodeModel() { Code = "34", Cn_Message = "作弊嫌疑", En_Message = "Fraud Suspected retrial over" },
            new ResponseCodeModel() { Code = "76", Cn_Message = "產品代碼錯誤", En_Message = "Invalid  Product Code Found" },
            new ResponseCodeModel() { Code = "36", Cn_Message = "受限制的帳戶", En_Message = "Restricted Account" },
            new ResponseCodeModel() { Code = "77", Cn_Message = "結帳錯誤", En_Message = "Settlement Error" },
        };
        List<ErrorCodeModel> errorCodes = new List<ErrorCodeModel>()
        {
            new ErrorCodeModel(){ Code = -1, Cn_Message = "其他操作進行中", En_Message = "Other operation in progress."},
            new ErrorCodeModel(){ Code = -2, Cn_Message = "無效金額", En_Message = "Invalid amount"},
            new ErrorCodeModel(){ Code = -3, Cn_Message = "無效支付碼", En_Message = "Invalid barcode"},
            new ErrorCodeModel(){ Code = -4, Cn_Message = "無效收銀機參考編號", En_Message = "Invalid ECR reference number"},
            new ErrorCodeModel(){ Code = -5, Cn_Message = "沒有記錄", En_Message = "No such record"},
            new ErrorCodeModel(){ Code = -6, Cn_Message = "沒有交易請求", En_Message = "No such transaction request"},
            new ErrorCodeModel(){ Code = -7, Cn_Message = "等待回覆中", En_Message = "Waiting for response"},

            new ErrorCodeModel(){ Code = -11, Cn_Message = "沖正等處理中", En_Message = "Reversal Pending"},
            new ErrorCodeModel(){ Code = -12, Cn_Message = "連線錯誤", En_Message = "Connection problem"},
            new ErrorCodeModel(){ Code = -13, Cn_Message = "交易已撤消", En_Message = "Transaction already voided"},
            new ErrorCodeModel(){ Code = -14, Cn_Message = "交易超時或通訊錯誤", En_Message = "Transaction Timeout or Communication Error"},

            new ErrorCodeModel(){ Code = -98, Cn_Message = "不知名錯誤", En_Message = "Unknown error"},
            new ErrorCodeModel(){ Code = -99, Cn_Message = "其他錯誤", En_Message = "Other error"},
        };

        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int ReturnId { get; set; }
        public TransactionRecord TransactionRecord { get; set; }
        public ResponseCodeModel GetErrorResposeMessage()
        {
            if (TransactionRecord != null)
            {
                ResponseCodeModel codeModel = responseCodes.FirstOrDefault(f => f.Code.Trim().Equals(TransactionRecord.respondCode?.Trim(), System.StringComparison.OrdinalIgnoreCase));
                return codeModel;
            }
            return null;
        }

        public bool IsPaymentCompleted()
        {
            return $"{TransactionRecord.respondCode}".Equals("00");
        }

        public ErrorCodeModel GetErrorMessage()
        {
            ErrorCodeModel codeModel = errorCodes.FirstOrDefault(f => f.Code == ReturnId);
            return codeModel;
        }
    }
}
