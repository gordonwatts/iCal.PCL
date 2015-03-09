using iCal.PCL.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace iCal.PCL.Test.Serialization
{
    /// <summary>
    /// Test the raw serializer
    /// </summary>
    [TestClass]
    public class t_iCalRawSerializer
    {
        [TestMethod]
        public void ParseEmpty()
        {
            // A file with nothing should give us nothing!
            var r = iCalRawSerializer.Deserialize(Enumerable.Empty<string>());
            Assert.IsNull(r);
        }

        [TestMethod]
        public void ParseBlankLines()
        {
            // A file with nothing should give us nothing!
            var r = iCalRawSerializer.Deserialize(TestUtils.FromParameterList("", "", ""));
            Assert.IsNull(r);
        }

        [TestMethod]
        [DeploymentItem("Serialization/Event1.ics")]
        public void ParseSimpleCalendar()
        {
            var r = iCalRawSerializer.Deserialize(new FileInfo("Event1.ics").AsLines());
            Assert.IsNotNull(r);
            Assert.AreEqual("2.0", r["VERSION"]);

            var bs = r.SubBlocks["VEVENT"];
            Assert.AreEqual(1, bs.Length);
            var b = bs[0];
            Assert.AreEqual("Test event", b["SUMMARY"]);
        }
    }
}
