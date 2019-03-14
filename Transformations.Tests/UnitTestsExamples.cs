using NUnit.Framework;
using System;

namespace Transformations.Tests
{
    [TestFixture]
    public class UnitTestExamples
    {
        [Test]
        public void Example1()
        {
            Assert.IsFalse(false, "test");
        }

        //[Test]
        //public void Example2()
        //{
        //    Assert.AreEqual(false, "");
        //}
    }
}
