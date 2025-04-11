using Newtonsoft.Json;
using System;

namespace WashMachine.Business.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string Location { get; set; }
        public float Amount { get; set; }
        public int Quantity { get; set; }
        public int PaymentId { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }
        public int PaymentStatus { get; set; }
        public string DeviceId { get; set; }
        public string CardJson { get; set; }
        public string OctopusNo { get; set; }
        public string BuyerAccountID { get; set; }
        public string Message { get; set; }
        public DateTime? InsertTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public TransactionRecord TransactionRecord
        {

            get
            {
                try
                {
                    if (PaymentTypeName == "Alipay" || PaymentTypeName == "Payme")
                    {
                        return JsonConvert.DeserializeObject<TransactionRecord>(CardJson);
                    }
                }
                catch (Exception)
                {

                }
                return new TransactionRecord();
            }
        }
    }

    public class TransactionRecord
    {
        public short paymentType;

        public short transType;

        public string merchantID;

        public string terminalID;

        public string respondCode;

        public string approvalCode;

        public string respondText;

        public string ECRReferenceNumber;

        public string barcode;

        public long amount;

        public string transactionDateTime;

        public string cutOffDay;

        public string traceNum;

        public string debitInCurrency;

        public long debitInAmount;

        public string OrderNumber;

        public string hostMessage;

        public bool voided;

        public string originalTraceNum;

        public string originalOrderNumber;

        public string RRN;

        public string hostDateTime;

        public long discountAmount;

        public long couponAmount;

        public long paymentAmount;

        public string oriTID;

        public string buyerAccountID;

        public string open_id;
    }
}
