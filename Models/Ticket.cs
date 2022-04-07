using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.Models
{
    public class Ticket
    {
        public string Id { get; set; }
        public string TicketReports { get; set; }
        public string RegisteredDevice { get; set; }
        public string Created { get; set; }
        public int ServedMembers { get; set; }
        public bool Closed { get; set; }
        public int WaitingMembers { get; set; }
        public int NoTurnOutMembers { get; set; }
        public int Ratings { get; set; }
        public int TotalEscalated { get; set; }
        public string AverageWaitingTime { get; set; }
        public string AverageServingTime { get; set; }
        public int MembersBeingServed { get; set; }
        public int TotalMembers { get; set; }
        public Section Section { get; set; }
        public string SectionId { get; set; }
        public string Report { get; set; }
    }
}
