using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobilePayService.Models
{
    public class AgreementModel
    {
        public string BcTenantId { get; set; }
        public string Agreement_Id { get; set; }
        public string Status { get; set; }
        public string Status_Text { get; set; }
        public string External_Id { get; set; }
        public int Status_Code { get; set; }
        public DateTime Timestamp { get; set; }
    }
}