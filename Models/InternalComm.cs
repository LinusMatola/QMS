using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.Models
{
    public class InternalComm
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string SenderEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public int Order { get; set; }
        public string Created { get; set; }
        public bool Seen { get; set; }
    }
}
