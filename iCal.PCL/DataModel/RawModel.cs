using System.Collections.Generic;
using System.Linq;

namespace iCal.PCL.DataModel
{
    public class RawModel
    {
        /// <summary>
        /// The properties in the current item
        /// </summary>
        public Dictionary<string, string> Properties { get; private set; }

        /// <summary>
        /// Get the property of this particular name
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public string this[string pName]
        {
            get { return Properties[pName]; }
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
            Properties = new Dictionary<string, string>();
            SubBlocks = new Dictionary<string, RawModel[]>();
        }

        /// <summary>
        /// Add a property to the block
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        internal void AddProperty(string key, string value)
        {
            Properties[key] = value;
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
    }
}
