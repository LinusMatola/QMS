using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.Models
{
    public class WithDrawal
    {
        public string Id { get; set; }
        public string Created { get; set; }
        public string TicketNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string MemberReason { get; set; }
        public string MemberNumber { get; set; }
        public WithDrawalReason WithDrawalReason { get; set; }
        public string WithDrawalReasonId { get; set; }
        public bool Status { get; set; }
        public string Chat { get; set; }
        public QMSUser QMSUser { get; set; }
        public string QMSUserId { get; set; }
    }
}
