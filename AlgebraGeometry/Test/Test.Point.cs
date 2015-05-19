using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using NUnit.Framework;

namespace AlgebraGeometry.Test
{
    [TestFixture]
    public class TestPoint
    {
        #region Point Numerics

        [Test]
        public void Test1()
        {
            var point = new Point(2.0,3.0);
            var x = new Var('x');
            var y = new Var('y');
            object result = LogicSharp.Run(x, point);
            Assert.IsTrue(2.0.Equals(result));

            var m = new Var('X');
            result = LogicSharp.Run(m, point);
            Assert.Null(result);

            result = LogicSharp.Run(y, point);
            Assert.IsTrue(3.0.Equals(result));
        }

        [Test]
        public void Test1_1()
        {
            var point = new Point(2.0, 3.0);
            var x = new Var('x');
            var goal = new EqGoal(x, 4.0);
            var y = new Var('y');

            object result = LogicSharp.Run(x, goal, point);
            Assert.IsNull(result);

            goal = new EqGoal(y, 3.0);
            result = LogicSharp.Run(y, goal, point);
            Assert.True(3.0.Equals(result));
        }

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
