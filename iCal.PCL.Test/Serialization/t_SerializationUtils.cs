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

        #region Content Line Parsing Tests
        [TestMethod]
        public void SimpleLine()
        {
            var info = "first: second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Name);
            Assert.AreEqual("second", info.Value);
        }
        [TestMethod]
        public void SimpleLineWithExtraSpace()
        {
            var info = "first : second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Name);
            Assert.AreEqual("second", info.Value);
        }

        [TestMethod]
        public void SimpleLineWithNoValue()
        {
            var info = "first:".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Name);
            Assert.AreEqual("", info.Value);
        }

        [TestMethod]
        public void SingleValueParameter()
        {
            var info = "first; p1=10 : second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("p1", info.Keys.First());
            Assert.AreEqual(1, info["p1"].Length);
            Assert.AreEqual("10", info["p1"][0]);
        }

        [TestMethod]
        public void SingleValueParameterQuoted()
        {
            var info = "first; p1= \"10,15\" : second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("p1", info.Keys.First());
            Assert.AreEqual(1, info["p1"].Length);
            Assert.AreEqual("10,15", info["p1"][0]);
        }

        [TestMethod]
        public void TwoValueParameter()
        {
            var info = "first; p1=10,15 : second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("p1", info.Keys.First());
            Assert.AreEqual(2, info["p1"].Length);
            Assert.AreEqual("10", info["p1"][0]);
            Assert.AreEqual("15", info["p1"][1]);
        }

        [TestMethod]
        public void TwoValueParameterWithSpaces()
        {
            var info = "first; p1 = 10 , 15 : second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("p1", info.Keys.First());
            Assert.AreEqual(2, info["p1"].Length);
            Assert.AreEqual("10", info["p1"][0]);
            Assert.AreEqual("15", info["p1"][1]);
        }

        [TestMethod]
        public void TwoValueParameterMixedQuotes()
        {
            var info = "first; p1 = 10 , \"15\" : second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(1, info.Count);
            Assert.AreEqual("p1", info.Keys.First());
            Assert.AreEqual(2, info["p1"].Length);
            Assert.AreEqual("10", info["p1"][0]);
            Assert.AreEqual("15", info["p1"][1]);
        }

        [TestMethod]
        public void TwoParametersParameter()
        {
            var info = "first; p1=10,15; p2 = \"hi there\" : second".ParseAsICalContentLine();
            Assert.AreEqual("first", info.Name);
            Assert.AreEqual("second", info.Value);
            Assert.AreEqual(2, info.Count);
            Assert.IsTrue(info.ContainsKey("p2"));
            Assert.AreEqual(1, info["p2"].Length);
            Assert.AreEqual("hi there", info["p2"][0]);
        }
        #endregion
#endif
    }
}
