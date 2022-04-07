using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.Models
{
    public class TodaysTicket
    {
        public string Id { get; set; }
        public string TicketNumber { get; set; }
        public string Created { get; set; }
        public bool Served { get; set; }
        public bool Serving { get; set; }
        public string CheckInTime { get; set; }
        public int OrderNumber { get; set; }
        public Service Service { get; set; }
        public string ServiceId { get; set; }
        public Section Section { get; set; }
        public string SectionId { get; set; }
    }
}
