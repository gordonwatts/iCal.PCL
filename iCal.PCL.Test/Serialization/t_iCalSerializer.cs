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
        public void TestEvent1()
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
            Assert.AreEqual(DateTime.Parse("07-18-2006 10:00", System.Globalization.CultureInfo.InvariantCulture), a1.DTStart);
            Assert.AreEqual(DateTime.Parse("07-18-2006 11:00", System.Globalization.CultureInfo.InvariantCulture), a1.DTEnd);
            Assert.AreEqual("20060717T210718Z", a1.Properties["LAST-MODIFIED"].Value);
            Assert.IsNull(a1.URL);
        }

        [TestMethod]
        [DeploymentItem("Serialization/indico1.ics")]
        public void TestIndico1()
        {
            var r = iCalSerializer.Deserialize(new FileInfo("indico1.ics").AsLines());
            Assert.IsNotNull(r);
            var all = r.ToArray();
            Assert.AreEqual(1, all.Length);
            var a1u = all[0];
            Assert.IsInstanceOfType(a1u, typeof(iCalVEvent));
            var a1 = a1u as iCalVEvent;

            Assert.AreEqual("indico-event-371544@cern.ch", a1.UID);
            Assert.AreEqual("LHCP2015 Steering Group Meeting", a1.Summary);
            Assert.AreEqual("Other Institutes", a1.Location);
            Assert.AreEqual("9th LHCP2015 Steering Group Meeting\nPin 0125\n\nhttps://indico.cern.ch/event/371544/", a1.Description);
            Assert.AreEqual(DateTime.Parse("02-03-2015 16:00:00", System.Globalization.CultureInfo.InvariantCulture), a1.DTStart + TimeZoneInfo.Local.GetUtcOffset(a1.DTStart));
            Assert.AreEqual(DateTime.Parse("02-03-2015 17:00:00", System.Globalization.CultureInfo.InvariantCulture), a1.DTEnd + TimeZoneInfo.Local.GetUtcOffset(a1.DTEnd));
            Assert.AreEqual("https://indico.cern.ch/event/371544/", a1.URL.OriginalString);
        }

        [TestMethod]
        [DeploymentItem("Serialization/1l12.ics")]
        public void TestIndicoList()
        {
            var r = iCalSerializer.Deserialize(new FileInfo("1l12.ics").AsLines());
            Assert.IsNotNull(r);
            var all = r.ToArray();
            Assert.AreEqual(57, all.Length);
            var a1u = all[0];
            Assert.IsInstanceOfType(a1u, typeof(iCalVEvent));
            var a1 = a1u as iCalVEvent;

            Assert.AreEqual("indico-event-371544@cern.ch", a1.UID);
            Assert.AreEqual("LHCP2015 Steering Group Meeting", a1.Summary);
            Assert.AreEqual("Other Institutes", a1.Location);
            Assert.AreEqual("9th LHCP2015 Steering Group Meeting\nPin 0125\n\nhttps://indico.cern.ch/event/371544/", a1.Description);
            Assert.AreEqual(DateTime.Parse("02-03-2015 16:00:00", System.Globalization.CultureInfo.InvariantCulture), a1.DTStart + TimeZoneInfo.Local.GetUtcOffset(a1.DTStart));
            Assert.AreEqual(DateTime.Parse("02-03-2015 17:00:00", System.Globalization.CultureInfo.InvariantCulture), a1.DTEnd + TimeZoneInfo.Local.GetUtcOffset(a1.DTEnd));
            Assert.AreEqual("https://indico.cern.ch/event/371544/", a1.URL.OriginalString);
        }

        [TestMethod]
        [DeploymentItem("Serialization/EventIndicoRadiation.ics")]
        public void TestIndicoRadiationEvent()
        {
            var r = iCalSerializer.Deserialize(new FileInfo("EventIndicoRadiation.ics").AsLines());
            Assert.IsNotNull(r);
            var all = r.ToArray();
            Assert.AreEqual(1, all.Length);
            var a1u = all[0];
            Assert.IsInstanceOfType(a1u, typeof(iCalVEvent));
            var a1 = a1u as iCalVEvent;

            Assert.AreEqual("Radiation from Relativistic Electrons in Periodic Structures \"RREPS-15\"", a1.Summary);
        }

        [TestMethod]
        [DeploymentItem("Serialization/EventIndicoLanguage.ics")]
        public void TestIndicoExcitedQCDEventLanguage()
        {
            // Make sure we can deal with other languages
            var r = iCalSerializer.Deserialize(new FileInfo("EventIndicoLanguage.ics").AsLines());
            Assert.IsNotNull(r);
            var all = r.ToArray();
            Assert.AreEqual(1, all.Length);
            var a1u = all[0];
            Assert.IsInstanceOfType(a1u, typeof(iCalVEvent));
            var a1 = a1u as iCalVEvent;

            Assert.AreEqual("Excited QCD 2015", a1.Summary);
        }
    }
}
