using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfferManagement.Models
{
    public class CallVolumeChart
    {
        public List<ChartMetrics> MissedCalls { get; set; }
        public List<ChartMetrics> IncomingCalls { get; set; }
        public List<ChartMetrics> OutgoingCalls { get; set; }
        public List<string> Labs { get; set; }
        public List<string> MissedCallsCount { get; set; }
        public List<string> IncomingCallsCount { get; set; }
        public List<string> OutgoingCallsCount { get; set; }

    }

    public class ChartMetrics
    {
        public string labName { get; set; }

        public int count { get; set; }
    }
}