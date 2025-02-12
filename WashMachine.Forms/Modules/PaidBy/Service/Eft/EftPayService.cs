using EFTSolutions;
using WashMachine.Forms.Modules.PaidBy.PaidByItems;
using WashMachine.Forms.Modules.PaidBy.PaidByItems.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WashMachine.Forms.Modules.PaidBy.Service.Eft
{
    public class EftPayService : IEftPayService
    {
        readonly string RESPONSE_SUCCESS_CODE = "00";

        EFTPaymentsServer eftPaymentServer;

        public EftPayService()
        {
            
        }

        public async Task<EftPayResponseModel> Sale(EftPayRequestModel eftPayParameter)
        {
            TransactionRecord tr = null;

            try
            {
                Logger.Log($"{nameof(AliPayPaidByItem)} Step 3");
                if (string.IsNullOrWhiteSpace(eftPayParameter.TilNumber))
                {
                    throw new Exception($"{nameof(eftPayParameter.TilNumber)} is not null");
                }

                if (string.IsNullOrWhiteSpace(eftPayParameter.Barcode))
                {
                    throw new Exception($"{nameof(eftPayParameter.Barcode)} is not null");
                }
                Logger.Log($"{nameof(AliPayPaidByItem)} Step 4");

                if (eftPaymentServer == null)
                {
                    eftPaymentServer = new EFTPaymentsServer(eftPayParameter.TilNumber);
                    Logger.Log($"{nameof(AliPayPaidByItem)} Step 4_1");
                }

                Logger.Log($"{nameof(AliPayPaidByItem)} Step 5");
                int iRet = eftPaymentServer.sale(eftPayParameter.PaymentType, eftPayParameter.Barcode, eftPayParameter.Amount, eftPayParameter.EcrRefNo);
                Logger.Log($"{nameof(AliPayPaidByItem)} Step 6");
                if (iRet == 0)
                {
                    Logger.Log($"{nameof(AliPayPaidByItem)} Step 7");
                    iRet = eftPaymentServer.getSaleResponse(eftPayParameter.PaymentType, ref tr);
                    Logger.Log($"{nameof(AliPayPaidByItem)} Step 8 {JsonConvert.SerializeObject(tr)}");
                }
                else
                {
                    return new EftPayResponseModel()
                    {
                        IsSuccess = false,
                        Message = $"Can not complete Sale by Eft",
                        TransactionRecord = null,
                        ReturnId = iRet
                    };
                }
                Logger.Log($"{nameof(AliPayPaidByItem)} Step 9");

                if (tr != null)
                {
                    List<string> saleUnconfirmed = new List<string> { "06", "19", "26", "96", "98" };

                    if (tr.respondCode.Equals(RESPONSE_SUCCESS_CODE))
                    {
                        Logger.Log($"{nameof(AliPayPaidByItem)} Step 10");
                        return new EftPayResponseModel()
                        {
                            IsSuccess = true,
                            Message = "Successfully",
                            TransactionRecord = tr,
                            ReturnId = iRet
                        };
                    }
                    else if (saleUnconfirmed.Contains(tr.respondCode))
                    {
                        iRet = eftPaymentServer.confirmLastTransaction();
                        if (iRet == 0)
                        {
                            iRet = eftPaymentServer.getConfirmTransactionResponse(ref tr);
                            if (tr.respondCode.Equals(RESPONSE_SUCCESS_CODE))
                            {
                                Logger.Log($"{nameof(AliPayPaidByItem)} Step 10");
                                return new EftPayResponseModel()
                                {
                                    IsSuccess = true,
                                    Message = "Successfully",
                                    TransactionRecord = tr,
                                    ReturnId = iRet
                                };
                            }
                        }
                    }

                    return new EftPayResponseModel()
                    {
                        IsSuccess = false,
                        Message = $"Can not complete Sale by Eft",
                        TransactionRecord = tr,
                        ReturnId = iRet
                    };
                }
                else
                {
                    return new EftPayResponseModel()
                    {
                        IsSuccess = false,
                        Message = $"Can not complete Sale by Eft",
                        TransactionRecord = tr,
                        ReturnId = iRet
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return new EftPayResponseModel()
                {
                    IsSuccess = false,
                    Message = $"Can not complete Sale by Eft {ex.Message}",
                    TransactionRecord = tr,
                    ReturnId = int.MinValue
                };
            }

        }
    }
}
