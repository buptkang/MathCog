using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AlgebraGeometry
{
    [TestFixture]
    public class TestLineUnify
    {
        #region Point and Point

        [Test]
        public void Test_CreateLine_1()
        {
            //x-y+1=0
            var pt1 = new Point(1.0, 2.0);
            var pt2 = new Point(2.0, 3.0);
            var line = LineRelation.Unify(pt1, pt2);
            Assert.NotNull(line);
            Assert.True(line.A.Equals(1.0));
            Assert.True(line.B.Equals(-1.0));
            Assert.True(line.C.Equals(1.0));
        }

        [Test]
        public void Test_CreateLine_2()
        {
            var pt1 = new Point(1.0, 2.0);
            var pt2 = new Point(1.0, 2.0);
            var line = LineRelation.Unify(pt1, pt2);
            Assert.Null(line);
        }

        [Test]
        public void Test_CreateLine_3()
        {
            var pt1 = new Point(2.0, 1.0);
            var pt2 = new Point(3.0, 1.0);
            var line = LineRelation.Unify(pt1, pt2);
            Assert.NotNull(line);
            Assert.Null(line.A);
            Assert.True(line.B.Equals(1.0));
            Assert.True(line.C.Equals(-1.0));
        }

        [Test]
        public void Test_CreateLine_4()
        {
            var pt1 = new Point(2.0, 1.0);
            var pt2 = new Point(2.0, 2.0);
            var line = LineRelation.Unify(pt1, pt2);
            Assert.NotNull(line);
            Assert.True(line.A.Equals(1.0));
            Assert.Null(line.B);
            Assert.True(line.C.Equals(-2.0));
        }

        #endregion
    }
}
