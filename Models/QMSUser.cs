using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HubnyxQMS.Models
{
    public class QMSUser : IdentityUser
    {
        public string FullName { get; set; }
        public virtual ICollection<QMSRole> QMSRoles { get; set; }
        public string ProfilePicture { get; set; }
        public string Created { get; set; }
        public int ServedToday { get; set; }
        public int ServedAlltime { get; set; }
        [AllowNull]
        public string NowServing { get; set; }
        [AllowNull]
        public string NowServingSecret { get; set; }
        public string NowServingSectionId { get; set; }
        public int RatingToday { get; set; }
        public int AverageRatingToday { get; set; }
        public int RatingAllTime { get; set; }
        public int AverageRatingAlltime { get; set; }
        public bool ServingMember { get; set; }
        public string AverageServingTime { get; set; }
        public double SuccessRateToday { get; set; }
        public double SuccessRateAlltime { get; set; }
        public bool CanViewDashboard { get; set; }
        public string FullReport { get; set; }
        public string DailyReport { get; set; }
        public string Notifications { get; set; }
        public string ServiceName { get; set; }

    }
}
