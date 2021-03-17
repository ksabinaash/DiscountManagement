using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OfferManagement.Models
{
    public class ChartsFilterModel
    {

        public DateTime FromDate { get; set; } = DateTime.Now.AddDays(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ChartsDaysRange"]));

        public DateTime ToDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "LabName")]
        public string LabName { get; set; }


    }
}