using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.Models
{
    public class ServicesReport
    {
        public string Id { get; set; }
        public string Report { get; set; }
        public string Created { get; set; }
        public DateTime Date { get; set; }
        public int TotalMembers { get; set; }
        public int TotalServed { get; set; }
        public int AverageWaitingTime { get; set; }
        public int TotalWaitingTime { get; set; }
        public int AverageServingTime { get; set; }
        public int TotalServingTime { get; set; }
        public int Rating { get; set; }
        public int RatingAverage { get; set; }
        public int EscalatedCases { get; set; }
        public int EscalatedCasesSolved { get; set; }
        public int EscalatedCasesAverage { get; set; }
        public Service Service { get; set; }
        public string ServiceId { get; set; }
    }
}
