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
#endif
    }
}
