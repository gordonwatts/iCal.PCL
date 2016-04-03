
using iCal.PCL.DataModel;
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
        public static IEnumerable<object> Deserialize(IEnumerable<string> inputLines)
        {
            // Do the most basic thing first
            var rawData = iCalRawSerializer.Deserialize(inputLines);
            if (rawData == null || rawData.SubBlocks.Count == 0)
                return Enumerable.Empty<object>();

            return rawData.SubBlocks.Select(binfo => TranslateBlock(binfo.Key, binfo.Value)).Where(x => x != null).SelectMany(x=> x);
        }

        /// <summary>
        /// Translators for each event block type
        /// </summary>
        /// <remarks>
        /// The implied contract is that the function cannot return null. If it gets called, it must return some sort of object.
        /// </remarks>
        private static Dictionary<string, Func<RawModel, object>> _translators = new Dictionary<string, Func<RawModel, object>>()
        {
            {"VEVENT", MakeVEvent}
        };

        /// <summary>
        /// Driver to find the proper translator for a particular object type
        /// </summary>
        /// <param name="iCalObjectName"></param>
        /// <param name="rawModel"></param>
        /// <returns></returns>
        private static IEnumerable<object> TranslateBlock(string iCalObjectName, DataModel.RawModel[] rawModel)
        {
            if (!_translators.ContainsKey(iCalObjectName))
                return null;
            var translator = _translators[iCalObjectName];
            return rawModel.Select(rm => translator(rm));
        }

        /// <summary>
        /// Given a block that is a VEvent, do the translation.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static object MakeVEvent(RawModel arg)
        {
            return new iCalVEvent()
            {
                Summary = arg.GetPropValueWithDefault("SUMMARY", "").AsiCalText(),
                UID = arg.GetPropValueWithDefault("UID", "").AsiCalText(),
                Location = arg.GetPropValueWithDefault("LOCATION", "").AsiCalText(),
                Description = arg.GetPropValueWithDefault("DESCRIPTION", "").AsiCalText(),
                DTEnd = arg.GetPropValueWithDefault("DTEND", "").AsiCalDateTime(),
                DTStart = arg.GetPropValueWithDefault("DTSTART", "").AsiCalDateTime(),
                URL = arg.GetPropValueWithDefault("URL", "").AsiCalUri(),
                Properties = arg.ContentLine,
            };
        }
    }
}
