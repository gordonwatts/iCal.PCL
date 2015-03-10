using System;
using System.Collections.Generic;
using System.Text;

namespace iCal.PCL.Serialization
{
    /// <summary>
    /// Some helper extension methods
    /// </summary>
    static class SerializationUtils
    {
        /// <summary>
        /// Given input lines, return them with continuations removed (unfolding) and ignore
        /// blank lines (they will kill off any continuations).
        /// </summary>
        /// <param name="sourceLines"></param>
        /// <returns></returns>
        public static IEnumerable<string> UnfoldLines(this IEnumerable<string> sourceLines)
        {
            StringBuilder bld = new StringBuilder();
            foreach (var line in sourceLines)
            {
                // If we have a blank line, we always finish off what we are looking at.
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (bld.Length > 0)
                    {
                        var r = bld.ToString();
                        bld.Clear();
                        yield return r;
                    }
                }
                else if (bld.Length == 0)
                {
                    // Always stash the first line
                    bld.Append(line);
                }
                else
                {
                    // If it starts with a space, then it is a continuation character
                    if (line[0] == ' ' || line[0] == '\t')
                    {
                        bld.Append(line.Substring(1));
                    }
                    else
                    {
                        var r = bld.ToString();
                        bld.Clear();
                        bld.Append(line);
                        yield return r;
                    }
                }
            }

            if (bld.Length > 0)
                yield return bld.ToString();
        }

        /// <summary>
        /// Split a line in two, by the colon.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Tuple<string, string> ParseAsICalContentLine(this string line)
        {
            var idx = line.IndexOf(":");
            if (idx < 0)
                throw new ArgumentException("Line does not contain a colon");

            return Tuple.Create(line.Substring(0, idx), line.Substring(idx + 1));
        }

        /// <summary>
        /// Short hand to help with throwing errors when we find something wrong.
        /// </summary>
        /// <param name="ifTrue"></param>
        /// <param name="message"></param>
        public static void ThrowiCalError(this bool ifTrue, string message, params string[] args)
        {
            if (ifTrue)
                throw new InvalidOperationException(string.Format(message, args));
        }
    }
}
