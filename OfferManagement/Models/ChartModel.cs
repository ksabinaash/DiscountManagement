using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfferManagement.Models
{
    public class CallVolumeChart
    {
        public List<string> labs { get; set; }

        public List<string> callTypes { get; set; }

        public Dictionary<string, List<ChartMetrics>> volumeData { get; set; }

        public List<List<string>> countData { get; set; }

    }

    public class ChartMetrics
    {
        public string name { get; set; }

        public int count { get; set; }
    }

    public class CallPurposeChart
    {
        public List<string> labs { get; set; }

        public List<string> purposes { get; set; }

        public Dictionary<string, List<ChartMetrics>> purposeData { get; set; }

        public List<List<int>> countData { get; set; }

        public List<string> sumData { get; set; }

    }

    public class CallTrendChart
    {
        public List<string> labs { get; set; }
        public List<string> callTypes { get; set; }
        public List<string> period { get; set; }
        public string labName { get; set; }
        public Dictionary<string, List<ChartMetrics>> trendData { get; set; }

        public List<List<string>> countData { get; set; }
    }
}