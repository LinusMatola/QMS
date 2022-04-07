using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.ViewModels
{
    public class ManageUserViewModel
    {
        public string FullName { get; set; }
        public string UserEmail { get; set; }
        public IList<string> UserRoles { get; set; }
    }
}
