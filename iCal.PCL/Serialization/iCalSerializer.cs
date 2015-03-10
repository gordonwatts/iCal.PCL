
using System;
using System.Collections.Generic;
using System.Linq;
namespace iCal.PCL.Serialization
{
    /// <summary>
    /// Deserialize an iCal item
    /// </summary>
    public class iCalSerializer
    {
        /// <summary>
        /// Given an valid "ics" file/data, return a list of the objects that are found in it.
        /// </summary>
        /// <param name="inputLines">The raw lines from the file</param>
        /// <returns>Sequence of iCalEVent, etc., objects</returns>
        IEnumerable<object> DeserializeiCal(IEnumerable<string> inputLines)
        {
            // Do the most basic thing first
            var rawData = iCalRawSerializer.Deserialize(inputLines);
            if (rawData == null || rawData.SubBlocks.Count == 0)
                return Enumerable.Empty<object>();

            throw new NotImplementedException();
        }
    }
}
