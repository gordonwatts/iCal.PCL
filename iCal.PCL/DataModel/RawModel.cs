using System.Collections.Generic;
using System.Linq;

namespace iCal.PCL.DataModel
{
    public class RawModel
    {
        /// <summary>
        /// The properties in the current item
        /// </summary>
        public Dictionary<string, RawContentLineInfo> ContentLine { get; private set; }

        /// <summary>
        /// Get the value a particular content line
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public RawContentLineInfo this[string pName]
        {
            get { return ContentLine[pName]; }
        }

        /// <summary>
        /// The models that are also contained in this calendar
        /// </summary>
        public Dictionary<string, RawModel[]> SubBlocks { get; private set; }

        /// <summary>
        /// Get the block list for a sub model
        /// </summary>
        /// <param name="bname"></param>
        /// <returns></returns>
        public RawModel[] GetBlocks(string bname)
        {
            return SubBlocks[bname];
        }

        /// <summary>
        /// The name of the block (VEVENT, etc.).
        /// </summary>
        public string Name { get; set; }

        public RawModel()
        {
            ContentLine = new Dictionary<string, RawContentLineInfo>();
            SubBlocks = new Dictionary<string, RawModel[]>();
        }

        /// <summary>
        /// Add a property to the block
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        internal void AddProperty(string key, RawContentLineInfo info)
        {
            ContentLine[key] = info;
        }

        /// <summary>
        /// Add a block to the list of blocks.
        /// </summary>
        /// <param name="r"></param>
        internal void AddBlock(RawModel r)
        {
            var inserter = new RawModel[] { r };
            if (!SubBlocks.ContainsKey(r.Name))
            {
                SubBlocks[r.Name] = inserter;
            }
            else
            {
                // TODO: Make efficient
                SubBlocks[r.Name] = SubBlocks[r.Name]
                    .Concat(inserter)
                    .ToArray();
            }
        }

        /// <summary>
        /// Return a property value or a default value.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        internal string GetPropValueWithDefault(string name, string defaultValue)
        {
            if (!ContentLine.ContainsKey(name))
                return defaultValue;
            var cl = ContentLine[name];
            return cl.Value;
        }
    }
}
