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
#endif
    }
}
