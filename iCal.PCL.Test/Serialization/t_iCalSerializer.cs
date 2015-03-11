using iCal.PCL.DataModel;
using iCal.PCL.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace iCal.PCL.Test.Serialization
{
    [TestClass]
    public class t_iCalSerializer
    {
        [TestMethod]
        [DeploymentItem("Serialization/Event1.ics")]
        public void TestMethod1()
        {
            var r = iCalSerializer.Deserialize(new FileInfo("Event1.ics").AsLines());
            Assert.IsNotNull(r);
            var all = r.ToArray();
            Assert.AreEqual(1, all.Length);
            var a1u = all[0];
            Assert.IsInstanceOfType(a1u, typeof(iCalVEvent));
            var a1 = a1u as iCalVEvent;

            Assert.AreEqual("uuid1153170430406", a1.UID);
            Assert.AreEqual("Test event", a1.Summary);
            Assert.AreEqual("Daywest", a1.Location);
            Assert.AreEqual(DateTime.Parse("18-07-2006 10:00"), a1.DTStart);
            Assert.AreEqual(DateTime.Parse("18-07-2006 11:00"), a1.DTEnd);
            Assert.AreEqual("20060717T210718Z", a1.Properties["LAST-MODIFIED"].Value);
        }
    }
}
