namespace WashMachine.Forms.Modules.PaidBy.Service.Eft
{
    public class EftPayRequestModel
    {
        public string TilNumber { get; set; }
        public short PaymentType { get; set; }
        public string Barcode { get; set; }
        public long Amount { get; set; }
        public string EcrRefNo { get; set; }
    }
}
