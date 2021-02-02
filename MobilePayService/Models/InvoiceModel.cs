using System;

namespace MobilePayService.Models
{
    public class InvoiceModel
    {
        public string InvoiceId { get; set; }
        public string InvoiceCallBackSoapURL { get; set; }
        public string BCTenantURL { get; set; }
        public string Status { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Date { get; set; }
    }
}