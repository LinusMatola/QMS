using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.Models
{
    public class Service
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Section Section { get; set; }
        public string SectionId { get; set; }
        public string FullReport { get; set; }
        public string DailyReport { get; set; }
    }
}
