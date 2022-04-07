using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.Models
{
    public class QMSUserService
    {
        public string Id { get; set; }
        public Service Service { get; set; }
        public string ServiceId { get; set; }
        public QMSUser QMSUser { get; set; }
        public string QMSUserId { get; set; }
        public string Created { get; set; }
        public int ServedToday { get; set; }
        public int ServedAllTime { get; set; }
    }
}
