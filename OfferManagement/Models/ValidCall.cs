using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfferManagement.Models
{
    public class ValidCall
    {
        public int ValidCallId { get; set; }

        public int? ExternalCallId { get; set; }

        public string LabName { get; set; }

        public string LabPhoneNumber { get; set; }

        public string CustomerMobileNumber { get; set; }

        public DateTime EventTime { get; set; }

        public int CallDuration { get; set; }

        public string CallType { get; set; }

        public string CallPurpose { get; set; }

        public string Action { get; set; }

        public string Comment { get; set; }

        public string CallStatus { get; set; }

        public string UpdatedUser { get; set; }

        public DateTime? FollowUpTime { get; set; }

        public DateTime UpdatedDateTime { get; set; } = DateTime.Now;

        public ICollection<MissedCall> MissedCalls { get; set; }

        public string MissedFollowUpOf { get; set; }
    }
}