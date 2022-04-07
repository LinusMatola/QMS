using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.Models
{
    public class Escalate
    {
        public string Id { get; set; }
        public string TicketNumber { get; set; }
        public string Reason { get; set; }
        public string ReplytoMember { get; set; }
        public string ReplytoEscalator { get; set; }
        public string EscalatedBy { get; set; }
        public string Created { get; set; }
        public string CreatedTime { get; set; }
        public bool Status { get; set; }
        public QMSUser QMSUser { get; set; }
        public string QMSUserId { get; set; }
        public string Chat { get; set; }
        public int TotalChats { get; set; }
    }
}
