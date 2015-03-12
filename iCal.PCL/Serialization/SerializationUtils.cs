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

        #region Content Line Parser
        // This uses Sprache - the RFC 2445 is resonably complex. It could be if there is a lot of parsing to be
        // done this should be replaced with something more efficient.

        /// <summary>
        ///     name               = x-name / iana-token
        ///     iana-token         = 1*(ALPHA / DIGIT / "-")
        ///     x-name             = "X-" [vendorid "-"] 1*(ALPHA / DIGIT / "-")
        /// </summary>
        static readonly Parser<string> ParseCLName =
            from s in (Parse.LetterOrDigit.Or(Parse.Char('-'))).AtLeastOnce().Text().Token()
            select s.ToUpper();

        ///     paramtext          = *SAFE-CHAR
        ///     SAFE-CHAR          = Any character except CTLs, DQUOTE, ";", ":", ","
        static readonly Parser<string> ParseCLSafeParameterValue =
            from s in Parse.AnyChar.Except(Parse.Chars("\";:,")).AtLeastOnce().Text().Token()
            select s.Trim().ToUpper();

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

        #endregion

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

        /// <summary>
        /// Short hand to help with throwing errors when we find something wrong.
        /// </summary>
        /// <param name="ifTrue"></param>
        /// <param name="message"></param>
        public static void ThrowiCalError<T>(this bool ifTrue, Func<string, T> makeError, string message, params string[] args)
            where T : Exception, new()
        {
            if (ifTrue)
            {
                throw makeError(string.Format(message, args));
            }
        }

        /// <summary>
        /// Parse a RFC 2445 date string. Time will be set to midnight in the local time zone.
        /// </summary>
        /// <param name="dateSpec">RFC 2445 date string</param>
        /// <returns>DateTime structure of the date. Throws if not valid</returns>
        public static DateTime AsiCalDate(this string dateSpec)
        {
            (dateSpec.Length != 8).ThrowiCalError(m => new FormatException(m), "The date '{0}' is not a valid RFC 2445 iCal date.", dateSpec);

            var year = dateSpec.Substring(0, 4);
            var month = dateSpec.Substring(4, 2);
            var day = dateSpec.Substring(6, 2);

            return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
        }

        /// <summary>
        /// Calculate the offset from midnight that this time represents, in the local time zone.
        /// This will loose the timezone info that is contained in the iCal. This will not convert
        /// the time - you need a date to do the conversion. It will only give you time zone info.
        /// </summary>
        /// <param name="timeSpec">String spec of the time</param>
        /// <param name="pInfo"></param>
        /// <returns>The time the string represents, and if it is in UTC format or not</returns>
        /// <remarks>
        /// RFC 2445 has three types of date string:
        ///   1. Local time - this is correctly parsed
        ///   2. UTC time - this is correctly parsed
        ///   3. VTIMEZONE time - where the timezone rules are given in the input, and the timezone is named in the property. This is not
        ///                       parsed, and will be in local time currently.
        /// When #3 is implemented, the return (bool) should be changed to deal with it.
        /// </remarks>
        public static Tuple<TimeSpan, bool> AsiCalTime(this string timeSpec, RawContentLineInfo pInfo)
        {
            var isUtc = false;
            if (timeSpec.EndsWith("Z"))
            {
                isUtc = true;
                timeSpec = timeSpec.Substring(0, timeSpec.Length - 1);
            }

            (timeSpec.Length != 6).ThrowiCalError(m => new FormatException(m), "The time '{0}' is not a valid RFC 2445 iCal time.", timeSpec);

            var hours = int.Parse(timeSpec.Substring(0, 2));
            var minutes = int.Parse(timeSpec.Substring(2, 2));
            var seconds = int.Parse(timeSpec.Substring(4, 2));

            (hours < 0 || hours > 23
                || minutes < 0 || minutes > 59
                || seconds < 0 || seconds > 59).ThrowiCalError(m => new ArgumentOutOfRangeException(m), "Time '{0}' has some out of range components.", timeSpec);

            return Tuple.Create(new TimeSpan(hours, minutes, seconds), isUtc);
        }

        /// <summary>
        /// Return a DateTime from an iCal date/time string, with the time conversion done. Currently supports only:
        /// 1. Local time
        /// 2. UTC time
        /// THe third format, which has time zone infomration included in the ics file, is not supported.
        /// </summary>
        /// <param name="dateTimeSpec"></param>
        /// <param name="pinfo"></param>
        /// <returns>The local time of a time given in the file</returns>
        public static DateTime AsiCalDateTime(this string dateTimeSpec)
        {
            var tIndex = dateTimeSpec.IndexOf('T');
            (tIndex < 0).ThrowiCalError(m => new FormatException(m), "The date-time '{0}' was not a valid RFC 2445 iCal date-time", dateTimeSpec);

            var dt = dateTimeSpec.Substring(0, tIndex).AsiCalDate();
            var ts = dateTimeSpec.Substring(tIndex + 1).AsiCalTime(null);

            // If this is a UTC guy, then we have some issues
            var sumTime = dt + ts.Item1;
            if (ts.Item2)
            {
                var utcOffset = TimeZoneInfo.Local.GetUtcOffset(sumTime);
                sumTime -= utcOffset;
            }

            return sumTime;
        }
    }
}
