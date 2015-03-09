using System;
using System.Collections.Generic;

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
            foreach (var line in sourceLines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    yield return line;
            }
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
