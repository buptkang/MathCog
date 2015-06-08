using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using NUnit.Framework;

namespace AlgebraGeometry.Test
{
    [TestFixture]
    public partial class TestPoint
    {
        #region Point Checking

        [Test]
        public void Test_check()
        {
            var x = new Var('x');
            var point = new Point(x, 3);
            Assert.False(point.Concrete);
            point = new Point(2.0,3.0);
            Assert.True(point.Concrete);
        }

        [Test]
        public void Test_check1()
        {
            var pt = new Point(1, 1);
            Assert.True(pt.XCoordinate.Equals(1.0));
            Assert.True(pt.YCoordinate.Equals(1.0));
        }

        #endregion

        #region Point Rounding Test

        [Test]
        public void Test_Point_Rounding_1()
        {
            var point = new Point(2.01111212, 3.12121211);
            Assert.True(point.XCoordinate.Equals(2.0));
            Assert.True(point.YCoordinate.Equals(3.1));
        }
       
        #endregion

        #region Point Reify and DynamicObject Reify
    
        [Test]
        public void Test_PassByValue()
        {
            var x = new Var('x');
            var point = new Point(x, 3);
            var goal = new EqGoal(x, 2.0);
            point.Reify(goal);
            Assert.False(point.Concrete);
            Assert.True(point.CachedSymbols.Count == 1);
            Assert.True(point.CachedGoals.Count == 1);

            var gShape = point.CachedSymbols.ToList()[0];
            Assert.NotNull(gShape);
            Assert.True(gShape.Traces.Count == 1);
            var ts = gShape.Traces[0] as TraceStep;
            Assert.NotNull(ts);
            Assert.True(ts.Source.Equals(point));
            Shape expectPt = new Point(2.0, 3.0);
            Assert.IsInstanceOf(typeof(Point), ts.Target);
            var tPt = ts.Target as Point;
            Assert.NotNull(tPt);
            Assert.True(tPt.Equals(expectPt));
        }

        [Test]
        public void Test_PassByReference()
        {
            var x = new Var('x');
            var point = new Point(x, 3);
            Goal goal = new EqGoal(x, 2.0);
            point.Reify(goal);
            Assert.True(point.Properties.ContainsKey(x));
            Assert.True(point.Properties[x].Equals(2.0));
        }

        #endregion

        #region Point Symbolic

        [Test]
        public void Test1_Symbolic()
        {
           var pt = new Point(1, 1);
           var ptSym = new PointSymbol(pt);

           Assert.True(ptSym.ToString().Equals("(1,1)"));

           pt = new Point("A", 1,1);
           ptSym = new PointSymbol(pt);
           Assert.True(ptSym.ToString().Equals("A(1,1)"));
        }

        [Test]
        public void Test2_Symbolic()
        {
            var pt = new Point("x", -2);
            var ptSym = new PointSymbol(pt);
            Assert.True(ptSym.ToString().Equals("(x,-2)"));
        }
        #endregion

        #region Numeric and Symbolic Integration Test

        [Test]
        public void Test1_Integration()
        {
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var ptSym = new PointSymbol(point);
            Assert.True(ptSym.ToString().Equals("(x,y)"));
            point.Label = "A";
            Assert.True(ptSym.ToString().Equals("A(x,y)"));

            Goal goal1 = new EqGoal(y, -4);
            Goal goal2 = new EqGoal(x, -3);

            var lst = new List<Goal>()
            {
                goal1, goal2
            };

            point.Reify(lst);
            Assert.True(ptSym.ToString().Equals("A(-3,-4)"));
        }

        #endregion

    }
}
