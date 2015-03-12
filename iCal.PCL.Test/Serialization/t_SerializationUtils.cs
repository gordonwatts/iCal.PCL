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
        [ExpectedException(typeof(InvalidOperationException))]
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
        #endregion

#endif
    }
}
