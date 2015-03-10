using iCal.PCL.DataModel;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
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
        ///     name               = x-name / iana-token
        ///     iana-token         = 1*(ALPHA / DIGIT / "-")
        ///     x-name             = "X-" [vendorid "-"] 1*(ALPHA / DIGIT / "-")
        /// </summary>
        static readonly Parser<string> ParseCLName =
            from s in (Parse.LetterOrDigit.Or(Parse.Char('-'))).AtLeastOnce().Text().Token()
            select s;

        ///     paramtext          = *SAFE-CHAR
        ///     SAFE-CHAR          = Any character except CTLs, DQUOTE, ";", ":", ","
        static readonly Parser<string> ParseCLSafeParameterValue =
            from s in Parse.AnyChar.Except(Parse.Chars("\";:,")).AtLeastOnce().Text().Token()
            select s.Trim();

        ///     quoted-string      = DQUOTE *QSAFE-CHAR DQUOTE
        ///     QSAFE-CHAR         = Any character except CTLs and DQUOTE
        static readonly Parser<string> ParseCLQuotedParameterValue =
            from chars in Parse.AnyChar.Except(Parse.Char('"')).Many().Text().Contained(Parse.Char('"'), Parse.Char('"')).Token()
            select chars;

        ///     param-value        = paramtext / quoted-string
        static readonly Parser<string> ParseCLParameterValue =
            from s in ParseCLSafeParameterValue.Or(ParseCLQuotedParameterValue)
            select s;

        ///     contentline        = name *(";" param ) ":" value CRLF
        ///     param              = param-name "=" param-value
        ///                          *("," param-value)
        static readonly Parser<Tuple<string, string[]>> ParseCLParameter =
            from semi in Parse.Char(';')
            from name in ParseCLName
            from eql in Parse.Char('=')
            from values in ParseCLParameterValue.DelimitedBy(Parse.Chars(","))
            select Tuple.Create(name, values.ToArray());


        ///     value              = *VALUE-CHAR
        ///     VALUE-CHAR         = Any textual character
        static readonly Parser<string> ParseCLValue =
            from c in Parse.AnyChar.Many().Text().Token()
            select c;

        ///     contentline        = name *(";" param ) ":" value CRLF
        static readonly Parser<RawContentLineInfo> ParseRawContentLineInfo =
            from n in ParseCLName
            from ps in ParseCLParameter.Many()
            from colon in Parse.Char(':')
            from v in ParseCLValue
            select new RawContentLineInfo(n, ps, v);

        /// <summary>
        /// Parse a content line.
        /// </summary>
        /// <param name="line">String of the content line</param>
        /// <returns>The name and the rest of the content line</returns>
        /// <remarks>
        /// From RFC 2445:
        ///     name               = x-name / iana-token
        ///     iana-token         = 1*(ALPHA / DIGIT / "-")
        ///     x-name             = "X-" [vendorid "-"] 1*(ALPHA / DIGIT / "-")
        ///     param              = param-name "=" param-value
        ///                          *("," param-value)
        ///     param-name         = iana-token / x-token
        /// </remarks>
        public static RawContentLineInfo ParseAsICalContentLine(this string line)
        {
            return ParseRawContentLineInfo.Parse(line);
        }

        /// <summary>
        /// Do a very simple split around the standard single line.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Tuple<string, string> SplitiCalLine(this string line)
        {
            var idx = line.IndexOf(":");
            if (idx < 0)
                throw new ArgumentException("Line does not contain a colon");

            return Tuple.Create(line.Substring(0, idx).Trim(), line.Substring(idx + 1).Trim());
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
