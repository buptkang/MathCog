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

        #region Point Query
    
        [Test]
        public void Test2()
        {
            var x = new Var('x');
            var point = new Point(x, 3);
            Goal goal = new EqGoal(x, 2.0);
            point.Reify(goal);
            Assert.True(point.Concrete);
            Assert.True(2.0.Equals(point.Properties[point.XCoordinate]));
            Assert.True(3.Equals(point.Properties[point.YCoordinate]));
        }

        [Test]
        public void Test2_1()
        {
            var x = new Var('x');
            var point = new Point(x, 3);
            Goal goal = new EqGoal(x, 2.0);
            point.Reify(goal);
            Assert.True(point.Concrete);
            Assert.True(2.0.Equals(point.Properties[point.XCoordinate]));
            Assert.True(3.Equals(point.Properties[point.YCoordinate]));

            //point.UnReify(goal);
            Assert.False(point.Concrete);
            Assert.False(point.Properties.ContainsKey(point.XCoordinate));
            Assert.True(3.Equals(point.Properties[point.YCoordinate]));
        }

        [Test]
        public void Test3()
        {
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            Goal goal1 = new EqGoal(x, -3);
            Goal goal2 = new EqGoal(y, -4);

            var lst = new List<Goal>();
            lst.Add(goal1);
            lst.Add(goal2);

            point.Reify(lst);
            Assert.True(point.Concrete);
            Assert.True(point.Properties[point.XCoordinate].Equals(-3));
            Assert.True(point.Properties[point.YCoordinate].Equals(-4));
        }

        [Test]
        public void Test4()
        {
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            Goal goal1 = new EqGoal(x, -3);
            Goal goal2 = new EqGoal(y, -4);
            Goal goal3 = new EqGoal(x, 2);
            var lst = new List<Goal>()
            {
                goal1, goal2,goal3
            };

            point.Reify(lst);
            Assert.False(point.Concrete);
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
