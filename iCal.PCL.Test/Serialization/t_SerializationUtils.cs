using iCal.PCL.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace iCal.PCL.Test.Serialization
{
    [TestClass]
    public class t_SerializationUtils
    {
        // Internals are only visible in the debug version
#if DEBUG
        [TestMethod]
        public void BlankLinesRemoved()
        {
            var lines = TestUtils.FromParameterList("", "", "");
            var r = lines.UnfoldLines().ToArray();
            Assert.AreEqual(0, r.Length);
        }

        [TestMethod]
        public void StandardFoldedLine()
        {
            var lines = TestUtils.FromParameterList(
                "SUMMARY: This is a test of th",
                " is thing."
                );
            var r = lines.UnfoldLines().ToArray();
            Assert.AreEqual(1, r.Length);
            Assert.AreEqual("SUMMARY: This is a test of this thing.", r[0]);
        }

        [TestMethod]
        public void Outlook2007FoldedLines()
        {
            var lines = TestUtils.FromParameterList(
                "first: this is",
                "\t the second line"
                );
            var r = lines.UnfoldLines().ToArray();
            Assert.AreEqual(1, r.Length);
            Assert.AreEqual("first: this is the second line", r[0]);
        }

        [TestMethod]
        public void MultipleNormalLines()
        {
            var lines = TestUtils.FromParameterList(
                "first: this is",
                "second: no way",
                "third: freak",
                "fourth: dude"
                );
            var r = lines.UnfoldLines().ToArray();
            Assert.AreEqual(4, r.Length);
            Assert.AreEqual("third: freak", r[2]);
        }

        #region Line Splitter
        [TestMethod]
        public void SimpleLineSplit()
        {
            var r = "hi: there".SplitiCalLine();
            Assert.AreEqual("hi", r.Item1);
            Assert.AreEqual("there", r.Item2);
        }

        [TestMethod]
        public void SimpleLineSplitWithSpaces()
        {
            var r = "hi :    there".SplitiCalLine();
            Assert.AreEqual("hi", r.Item1);
            Assert.AreEqual("there", r.Item2);
        }
        #endregion

        #region Content Line Parsing Tests
        [TestMethod]
        public void SimpleLineLowCase()
        {
            var info = "first: second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
        }

        [TestMethod]
        public void SimpleLineUpCase()
        {
            var info = "FIRST: second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
        }

        [TestMethod]
        public void SimpleLineMixedCase()
        {
            var info = "FIrST: second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
        }

        [TestMethod]
        public void SimpleLineWithExtraSpace()
        {
            var info = "first : second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
        }

        [TestMethod]
        public void SimpleLineWithNoValue()
        {
            var info = "first:".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("", info.Value);
        }

        [TestMethod]
        public void SingleValueParameterLowCase()
        {
            var info = "first; p1=10 : second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("P1", info.Keys.First());
            Assert.AreEqual(1, info["P1"].Length);
            Assert.AreEqual("10", info["P1"][0]);
        }

        [TestMethod]
        public void SingleValueParameterUpCase()
        {
            var info = "first; P1=10 : second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("P1", info.Keys.First());
            Assert.AreEqual(1, info["P1"].Length);
            Assert.AreEqual("10", info["P1"][0]);
        }

        [TestMethod]
        public void SingleValueParameterQuoted()
        {
            var info = "first; p1= \"10,15\" : second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("P1", info.Keys.First());
            Assert.AreEqual(1, info["P1"].Length);
            Assert.AreEqual("10,15", info["P1"][0]);
        }

        [TestMethod]
        public void SingleValueParameterQuotedCaseMixed()
        {
            var info = "first; p1= \"This\" : second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("P1", info.Keys.First());
            Assert.AreEqual(1, info["P1"].Length);
            Assert.AreEqual("This", info["P1"][0]);
        }

        [TestMethod]
        public void SingleValueParameterUnQuotedCaseMixed()
        {
            var info = "first; p1= This : second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("P1", info.Keys.First());
            Assert.AreEqual(1, info["P1"].Length);
            Assert.AreEqual("THIS", info["P1"][0]);
        }

        [TestMethod]
        public void TwoValueParameter()
        {
            var info = "first; p1=10,15 : second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("P1", info.Keys.First());
            Assert.AreEqual(2, info["P1"].Length);
            Assert.AreEqual("10", info["P1"][0]);
            Assert.AreEqual("15", info["P1"][1]);
        }

        [TestMethod]
        public void TwoValueParameterWithSpaces()
        {
            var info = "first; p1 = 10 , 15 : second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("P1", info.Keys.First());
            Assert.AreEqual(2, info["P1"].Length);
            Assert.AreEqual("10", info["P1"][0]);
            Assert.AreEqual("15", info["P1"][1]);
        }

        [TestMethod]
        public void TwoValueParameterMixedQuotes()
        {
            var info = "first; p1 = 10 , \"15\" : second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("P1", info.Keys.First());
            Assert.AreEqual(2, info["P1"].Length);
            Assert.AreEqual("10", info["P1"][0]);
            Assert.AreEqual("15", info["P1"][1]);
        }

        [TestMethod]
        public void TwoParametersParameter()
        {
            var info = "first; p1=10,15; p2 = \"hi there\" : second".ParseAsICalContentLine();
            Assert.AreEqual("FIRST", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(2, info.Count);
            Assert.IsTrue(info.ContainsKey("P2"));
            Assert.AreEqual(1, info["P2"].Length);
            Assert.AreEqual("hi there", info["P2"][0]);
        }
        #endregion

        #region Date parsing
        [TestMethod]
        public void SimpleDate()
        {
            var d = "19970714".AsiCalDate();
            Assert.AreEqual(1997, d.Year);
            Assert.AreEqual(7, d.Month);
            Assert.AreEqual(14, d.Day);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void SimpleDateWrongLength()
        {
            var d = "199707144".AsiCalDate();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SimpleDateBadDay()
        {
            var d = "19970744".AsiCalDate();
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void SimpleDateAlpha()
        {
            var d = "199707a4".AsiCalDate();
        }

        [TestMethod]
        public void SimpleLocalTime()
        {
            var t = "230001".AsiCalTime(null);
            Assert.AreEqual(23, t.Item1.Hours);
            Assert.AreEqual(0, t.Item1.Minutes);
            Assert.AreEqual(1, t.Item1.Seconds);
            Assert.IsFalse(t.Item2);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void SimpleLocalTimeWrongLength()
        {
            var t = "2230001".AsiCalTime(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SimpleLocalTimeBadMinute()
        {
            var t = "239901".AsiCalTime(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void SimpleLocalTimeAlpha()
        {
            var t = "230a01".AsiCalTime(null);
        }

        [TestMethod]
        public void SimpleLocalTimeUTC()
        {
            var t = "100000Z".AsiCalTime(null);

            Assert.AreEqual(0, t.Item1.Minutes);
            Assert.AreEqual(0, t.Item1.Seconds);
            Assert.AreEqual(10, t.Item1.Hours);
            Assert.IsTrue(t.Item2);

        }

        [TestMethod]
        public void SimpleLocalDateTime()
        {
            var t = "19980118T230000".AsiCalDateTime();
            Assert.AreEqual(1998, t.Year);
            Assert.AreEqual(1, t.Month);
            Assert.AreEqual(18, t.Day);
            Assert.AreEqual(23, t.Hour);
            Assert.AreEqual(0, t.Minute);
            Assert.AreEqual(0, t.Second);
        }

        [TestMethod]
        public void SimpleLocalDateTimeUtc()
        {
            var t = "20150313T000400Z".AsiCalDateTime();

            // We need to get the UTC offset to figure out how this is going to affect things, as it is right on th edge, and add it back.
            // Then it should match the numbers above.

            t += TimeZoneInfo.Local.GetUtcOffset(t);

            Assert.AreEqual(2015, t.Year);
            Assert.AreEqual(3, t.Month);
            Assert.AreEqual(4, t.Minute);
            Assert.AreEqual(0, t.Second);

            Assert.AreEqual(13, t.Day);
            Assert.AreEqual(0, t.Hour);
        }

        [TestMethod]
        public void SimpleText()
        {
            Assert.AreEqual("hi", "hi".AsiCalText());
        }

        [TestMethod]
        public void TextWithNewLineInMiddle()
        {
            Assert.AreEqual("hi\nhi", @"hi\nhi".AsiCalText());
        }

        [TestMethod]
        public void TextWithNewLineAtStart()
        {
            Assert.AreEqual("\nhihi", @"\nhihi".AsiCalText());
        }

        [TestMethod]
        public void TextWithNewLineAtEnd()
        {
            Assert.AreEqual("hihi\n", @"hihi\n".AsiCalText());
        }

        [TestMethod]
        public void TextWithNewLineAllOver()
        {
            Assert.AreEqual("\nhi\nhi\n", @"\nhi\nhi\n".AsiCalText());
        }
        #endregion

        #region Uri's
        [TestMethod]
        public void EmptyURI()
        {
            Assert.IsNull("".AsiCalUri());
        }

        [TestMethod]
        public void GoodURI()
        {
            Assert.AreEqual("http://www.nytimes.com", "http://www.nytimes.com".AsiCalUri().OriginalString);
        }
        #endregion
#endif
    }
}
