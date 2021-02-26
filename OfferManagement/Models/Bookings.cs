using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OfferManagement.Models
{
    public class Room
    {
        [Display(Name = "RoomNo")]
        public string RoomNo { get; set; }

        [Display(Name = "RoomType")]
        public string RoomType { get; set; }

        [Display(Name = "BookingDate")]
        public DateTime BookingDate { get; set; }

    }

    public class Bookings
    {

        public DateTime FromDate { get; set; } 

        public DateTime ToDate { get; set; } 

        public string RoomType { get; set; } = "Single";

    }
}