using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OfferManagement.Models
{
    public class DiscountTransaction
    {
        [Required]
        [Display(Name = "Customer Name *")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string CustomerName { get; set; }

        [Display(Name = "User Email")]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Display(Name = "Customer Email")]
        [EmailAddress]
        public string CustomerEmail { get; set; }

        [Required]
        [Display(Name = "Mobile Number *")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Not a valid 10 digit mobile number")]
        public string MobileNumber { get; set; }

        [Required]
        [Display(Name = "PCC Name *")]
        public string PCCName { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        [Display(Name = "Bill Value *")]
        public double BillValue { get; set; }

        [Required]
        [Display(Name = "Discount Amount *")]
        [Range(1, double.MaxValue, ErrorMessage = "Only positive number allowed")]
        public double Discount { get; set; }

        [Required]
        [Display(Name = "Billed Value")]
        public double BilledValue
        {
            get
            {
                return BillValue - Discount;
            }
        }

        [Required]
        [Display(Name = "Reason for Discount *")]
        public string DiscountReason { get; set; }

        
        [Display(Name = "OTP")]
        [StringLength(4, ErrorMessage = "The OTP must be 4 characters long.")]
        public string OTP { get; set; }

        [Required]
        [Display(Name = "OTP Message Template")]
        public string MessageTemplate { get; set; }

        [Required]
        public DateTime BilledDateTime { get; set; } = DateTime.Now.ToLocalTime();
        
        public string ValidationStatus { get; set; }

        public bool enableValidatebtn { get; set; } = false;

        //public bool enableResendbtn { get; set; } = false;

        public bool enableSubmitbtn { get; set; } = true;
    }
}