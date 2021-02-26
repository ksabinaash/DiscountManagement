using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfferManagement.Models
{
    public class MissedCallGrid
    {
        public int Id { get; set; }

        public string LabName { get; set; }

        public string LabPhoneNumber { get; set; }

        public string CustomerMobileNumber { get; set; }

        public DateTime EventTime { get; set; }

        public string CallBackStatus { get; set; }

        public string RespondedTime { get; set; }

        public string RespondedLabName { get; set; }

        public string RespondedLabPhoneNumber { get; set; }

        //Why ? missedcall number is the customer mobile number right
        public string RespondedCustomerMobileNumber { get; set; }


        //Why ? RespondedEventTime number is the RespondedTime right
        public DateTime RespondedEventTime { get; set; }

        public int RespondedCallDuration { get; set; }

        public string RespondedCallType { get; set; }

        public string CallPurpose { get; set; }

        public string Action { get; set; }

        public string Comment { get; set; }
    }
}