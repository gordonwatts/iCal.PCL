
using System;
using System.Collections.Generic;
namespace iCal.PCL.DataModel
{
    /// <summary>
    /// Data from an iCal VEVENT record.
    /// </summary>
    public class iCalVEvent
    {
        public string UID { get; set; }

        public string Location { get; set; }

        public string Summary { get; set; }

        public DateTime DTStart { get; set; }

        public DateTime DTEnd { get; set; }

        public Dictionary<string, RawContentLineInfo> Properties { get; set; }

        public string Description { get; set; }

        public Uri URL { get; set; }
    }
}
