using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OfferManagement.Models
{
    public class MissedCall
    {
        public int Id { get; set; }

        public int? ExternalCallId { get; set; }
        public string LabName { get; set; }

        public string LabPhoneNumber { get; set; }

        public string CustomerMobileNumber { get; set; }

        public DateTime EventTime { get; set; }

        public int? ValidCallId { get; set; }

        [ForeignKey("ValidCallId")]
        public ValidCall ValidCall { get; set; }
    }
}