namespace LearningKit.Models.Checkout
{
    /// <summary>
    /// Model for a fictitious payment response.
    /// </summary>
    public class ResponseModel
    {
        public int InvoiceNo { get; set; }
        public string Message { get; set; }
        public bool Completed { get; set; }
        public string TransactionID { get; set; }
        public string ResponseCode { get; set; }
        public decimal Amount { get; set; }
        public bool Approved { get; set; }
    }
}