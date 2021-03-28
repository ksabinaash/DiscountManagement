using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OfferManagement.Models
{
    public class ValidCallEdit
    {
        public int ValidCallId { get; set; }

        public DateTime EventTime { get; set; }

        public string LabName { get; set; }

        public string CustomerMobileNumber { get; set; }

        [Required]
        [Display(Name = "Purpose *")]
        public string CallPurpose { get; set; }

        [Required]
        [Display(Name = "Action *")]
        public string Action { get; set; }

        [Required]
        [Display(Name = "Comment *")]
        public string Comment { get; set; }

        [Display(Name = "FollowUpTime **")]
        public DateTime? FollowUpTime { get; set; }
    }
}