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
        [Display(Name = "Customer Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Not a valid 10 digit mobile number")]
        public string MobileNumber { get; set; }

        [Required]
        [Display(Name = "Shop Name")]
        [StringLength(75, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string ShopName { get; set; }

        [Required]
        [Display(Name = "Billed Value")]
        public double BilledValue { get; set; }

        [Required]
        [Display(Name = "Discount Amount")]
        public double Discount { get; set; }

        [Required]
        [Display(Name = "Reason for Discount")]
        public DiscountReason DiscountReason { get; set; }

        [Required]
        [Display(Name = "OTP")]
        [StringLength(6, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        public string OTP { get; set; }

        [Required]
        [Display(Name = "OTP Message Template")]
        public MessageTemplate MessageTemplate { get; set; }

        [Required]
        public DateTime BilledDateTime { get; set; } = DateTime.Now.ToLocalTime();
    }

    public enum MessageTemplate
    {
        TemplateA,
        TemplateB,
        TemplateC,
        TemplateD
    }

    public enum DiscountReason
    {
        RegularCustomer,
        PrivilegedCustomer,
        PromotionalOffer,
        CorporateBenefits
    }
}