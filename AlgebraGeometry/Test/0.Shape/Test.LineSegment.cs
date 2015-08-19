using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AlgebraGeometry
{
    [TestFixture]
    public class TestLineSegment
    {
        #region Line segment type checking and validation

        [Test]
        public void Test1()
        {
            var pt = new Point(1.0, 2.0);
            var pt2 = new Point(2.0, 4.0);
            var lineSeg = new LineSegment(pt, pt2);
           // Assert.True(lineSeg.RelationStatus);
            Assert.NotNull(lineSeg.Label);
        }

        [Test]
        public void Test2()
        {
            var pt1 = new Point(1.0, 2.0);
            var pt2 = new Point(1.0, 2.0);
            try
            {
                var lineSeg = new LineSegment(pt1, pt2);
                Assert.Null(lineSeg);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }

        #endregion
    }
}
