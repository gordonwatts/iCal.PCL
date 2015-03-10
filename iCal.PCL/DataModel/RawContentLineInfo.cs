using System;
using System.Collections.Generic;

namespace iCal.PCL.DataModel
{
    /// <summary>
    /// Contains the information from a single line. This includes:
    ///  - The name of the line (e.g. BEGIN or SUMMARY). This is always upper case.
    ///  - The list of parameters and values that appear before the ":". These are upper case except for
    ///    when they are quoted.
    ///  - The line after the ":", untouched.
    /// </summary>
    public class RawContentLineInfo : Dictionary<string, string[]>
    {
        /// <summary>
        /// The name for the content line
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// THe remainder of the line - the value of the content line.
        /// </summary>
        public string Value { get; set; }

        public RawContentLineInfo()
        {

        }

        /// <summary>
        /// Generate from the raw information
        /// </summary>
        /// <param name="name"></param>
        /// <param name="props"></param>
        /// <param name="value"></param>
        public RawContentLineInfo(string name, IEnumerable<Tuple<string, string[]>> props, string value)
        {
            Name = name;
            Value = value;
            if (props != null)
            {
                foreach (var item in props)
                {
                    Add(item.Item1, item.Item2);
                }
            }
        }
    }
}
