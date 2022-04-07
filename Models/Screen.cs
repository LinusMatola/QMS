using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.Models
{
    public class Screen
    {
        public string Id { get; set; }
        [AllowNull]
        public string TicketNumber { get; set; }
        public string Created { get; set; }
        public DeskCounter DeskCounter { get; set; }
        public string DeskCounterId { get; set; }
        public string DeskName { get; set; }
        public string Status { get; set; }
    }
}
