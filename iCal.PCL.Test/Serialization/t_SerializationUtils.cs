using iCal.PCL.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

#if false
        #region Content Line Parsing Tests
        [TestMethod]
        public void SimpleLine()
        {
            var info = "first: second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Item1);
            Assert.AreEqual("second", info.Item2.Value);
        }

        [TestMethod]
        public void SimpleLineWithExtraSpace()
        {
            var info = "first : second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Item1);
            Assert.AreEqual("second", info.Item2.Value);
        }

        [TestMethod]
        public void SingleValueParameter()
        {
            var info = "first; p1=10 : second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Item1);
            Assert.AreEqual("second", info.Item2.Value);
            Assert.AreEqual(1, info.Item2.Count);
            Assert.AreEqual("p1", info.Item2.Keys.First());
            Assert.AreEqual(1, info.Item2["p1"].Length);
            Assert.AreEqual("10", info.Item2["p1"][0]);
        }

        [TestMethod]
        public void TwoValueParameter()
        {
            var info = "first; p1=10,15 : second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Item1);
            Assert.AreEqual("second", info.Item2.Value);
            Assert.AreEqual(1, info.Item2.Count);
            Assert.AreEqual("p1", info.Item2.Keys.First());
            Assert.AreEqual(2, info.Item2["p1"].Length);
            Assert.AreEqual("10", info.Item2["p1"][0]);
            Assert.AreEqual("15", info.Item2["p1"][1]);
        }

        [TestMethod]
        public void TwoValueParameterWithSpaces()
        {
            var info = "first; p1 = 10 , 15 : second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Item1);
            Assert.AreEqual("second", info.Item2.Value);
            Assert.AreEqual(1, info.Item2.Count);
            Assert.AreEqual("p1", info.Item2.Keys.First());
            Assert.AreEqual(2, info.Item2["p1"].Length);
            Assert.AreEqual("10", info.Item2["p1"][0]);
            Assert.AreEqual("15", info.Item2["p1"][1]);
        }

        [TestMethod]
        public void SingleValueParameterQuoted()
        {
            var info = "first; p1= \"10,15\" : second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Item1);
            Assert.AreEqual("second", info.Item2.Value);
            Assert.AreEqual(1, info.Item2.Count);
            Assert.AreEqual("p1", info.Item2.Keys.First());
            Assert.AreEqual(1, info.Item2["p1"].Length);
            Assert.AreEqual("10,15", info.Item2["p1"][0]);
        }
        #endregion
#endif
#endif
    }
}
