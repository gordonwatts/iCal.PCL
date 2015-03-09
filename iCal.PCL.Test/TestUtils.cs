using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace iCal.PCL.Test
{
    static class TestUtils
    {
        /// <summary>
        /// Take a list of items of the same type and return them one after the other as an enumerable
        /// </summary>
        /// <typeparam name="T">Type of all arguments</typeparam>
        /// <param name="source">List of parameters to return</param>
        /// <returns></returns>
        public static IEnumerable<T> FromParameterList<T>(params T[] source)
        {
            foreach (var item in source)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Read the file in, all lines, and return them one at a time.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<string> AsLines(this FileInfo input)
        {
            Assert.IsTrue(input.Exists);
            using (var reader = input.OpenText())
            {
                string line = "hi";
                while (line != null)
                {
                    line = reader.ReadLine();
                    if (line != null)
                        yield return line;
                }
            }
        }
    }
}
