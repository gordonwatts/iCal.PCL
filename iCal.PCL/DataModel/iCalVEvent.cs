
using System.Collections.Generic;
namespace iCal.PCL.DataModel
{
    /// <summary>
    /// Data from an iCal VEVENT record.
    /// </summary>
    public class iCalVEvent
    {
        public object UID { get; set; }

        public object SUMMARY { get; set; }

        public object Location { get; set; }

        public object Summary { get; set; }

        public object DTStart { get; set; }

        public object DTEnd { get; set; }

        public Dictionary<string, RawContentLineInfo> Properties { get; set; }
    }
}
