using iCal.PCL.DataModel;
using System.Collections.Generic;

namespace iCal.PCL.Serialization
{
    /// <summary>
    /// Basic class to deserialize (and perhaps one day serialize) iCal's
    /// </summary>
    public class iCalRawSerializer
    {
        /// <summary>
        /// Does the most basic deserialization. We will return when we reach the closing of
        /// the first opening block we find.
        /// </summary>
        /// <param name="input"></param>
        /// <remarks>
        /// iCal input is a series of lines (some are continued).
        /// </remarks>
        public static RawModel Deserialize(IEnumerable<string> lines)
        {
            var stack = new Stack<RawModel>();
            RawModel current = null;
            foreach (var line in lines.UnfoldLines())
            {
                var t = line.SplitiCalLine();
                if (t.Item1 == "BEGIN")
                {
                    var r = new RawModel() { Name = t.Item2 };
                    stack.Push(current);
                    if (current != null)
                        current.AddBlock(r);
                    current = r;
                }
                else if (t.Item1 == "END")
                {
                    (t.Item2 != current.Name).ThrowiCalError("Opening ('{0}') and closing ('{1}') blocks do not match", current.Name, t.Item2);
                    // Note that the first push we do will be a null - so the check here may look funny!
                    if (stack.Count == 1)
                        return current;
                    current = stack.Pop();
                }
                else
                {
                    (current == null).ThrowiCalError("iCal Key/Value pair outside block");
                    current.AddProperty(t.Item1, t.Item2);
                }
            }

            return null;
        }

    }
}
