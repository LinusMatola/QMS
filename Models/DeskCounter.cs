using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.Models
{
    public class DeskCounter
    {
        public string Id { get; set; }
        public string CounterNumber { get; set; }
        public bool Status { get; set; }
        [AllowNull]
        public QMSUser QMSUser { get; set; }
        [AllowNull]
        public string QMSUserId { get; set; }
    }
}
