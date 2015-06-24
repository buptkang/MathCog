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

        #region  Point Reification (Substitution)

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
            //Assert.True(ts.Source.Equals(point));
            Shape expectPt = new Point(2.0, 3.0);
            Assert.IsInstanceOf(typeof(PointSymbol), ts.Target);
            var tPt = ts.Target as PointSymbol;
            Assert.NotNull(tPt);
            Assert.True(tPt.Shape.Equals(expectPt));
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

        [Test]
        public void Test_Sub_1()
        {
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var goal1 = new EqGoal(x, -3);
            var goal2 = new EqGoal(y, -4);
            point.Reify(goal1);
            point.Reify(goal2);
            Assert.True(point.CachedGoals.Count == 2);
            Assert.True(point.CachedSymbols.Count == 1);
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

        #region Reification (substitution)

        [Test]
        public void Test_reify_0()
        {
            //true positive
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var eqGoal = new EqGoal(x, 1); // x=1               
            bool result = point.Reify(eqGoal);

            Assert.True(result);

            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 1);
            Assert.True(point.CachedSymbols.Count == 1);
            var gPoint = point.CachedSymbols.ToList()[0] as Point;
            Assert.NotNull(gPoint);
            Assert.False(gPoint.Concrete);
            Assert.True(1.0.Equals(gPoint.XCoordinate));
            Assert.True(y.Equals(gPoint.YCoordinate));
            Assert.True(gPoint.CachedGoals.Count == 1);

            /******
             * current status:
             * (1,y)
             * 
             * next input:
             * x = 2
            *****/
            eqGoal = new EqGoal(x, 2); // x=2
            result = point.Reify(eqGoal);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 2);
            Assert.True(point.CachedSymbols.Count == 2);
            foreach (var shape in point.CachedSymbols)
            {
                Assert.False(shape.Concrete);
            }

            /******
            * current status:
            * (1,y)
            * (2,y)
            * 
            * next input:
            * y = 1
            *****/
            eqGoal = new EqGoal(y, 1); // y = 1
            result = point.Reify(eqGoal);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 3);
            Assert.True(point.CachedSymbols.Count == 2);
            foreach (var shape in point.CachedSymbols)
            {
                Assert.True(shape.Concrete);
            }

            /******
           * current status:
           * (1,1)
           * (2,1)
           * 
           * next input:
           * y = 2
           *****/
            eqGoal = new EqGoal(y, 2); // y = 2
            result = point.Reify(eqGoal);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 4);
            Assert.True(point.CachedSymbols.Count == 4);
            foreach (var shape in point.CachedSymbols)
            {
                Assert.True(shape.Concrete);
            }


            /******
            * current status:
            * (1,1)
            * (2,1)
            * (1,2)
            * (2,2)
            * next input:
            * x = 3
            *****/

            eqGoal = new EqGoal(x, 3); // x = 3
            result = point.Reify(eqGoal);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 5);
            Assert.True(point.CachedSymbols.Count == 6);
            foreach (var shape in point.CachedSymbols)
            {
                Assert.True(shape.Concrete);
            }
        }

        [Test]
        public void Test_reify_1()
        {
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var t = new Var('t');
            var eqGoal = new EqGoal(t, 1); // t=1               
            bool result = point.Reify(eqGoal);
            Assert.False(result);
        }

        [Test]
        public void Test_Unreify_0()
        {
            //true positive
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var eqGoal = new EqGoal(x, 1); // x=1               
            bool result = point.Reify(eqGoal);

            result = point.UnReify(eqGoal);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 0);
            Assert.True(point.CachedSymbols.Count == 0);
        }

        [Test]
        public void Test_Unreify_00()
        {
            //true positive
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var eqGoal = new EqGoal(x, 1); // x=1               
            bool result = point.Reify(eqGoal);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 1);
            Assert.True(point.CachedSymbols.Count == 1);

            var eqGoal2 = new EqGoal(y, 2); // y=2               
            result = point.Reify(eqGoal2);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 2);
            Assert.True(point.CachedSymbols.Count == 1);

            result = point.UnReify(eqGoal); // undo x=1
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 1);
            Assert.True(point.CachedSymbols.Count == 1);
            var gp = point.CachedSymbols.ToList()[0] as Point;
            Assert.True(gp.XCoordinate.Equals(x));
            Assert.True(gp.YCoordinate.Equals(2));

            result = point.UnReify(eqGoal2); // undo y = 2
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 0);
            Assert.True(point.CachedSymbols.Count == 0);
        }

        [Test]
        public void Test_Unreify_1()
        {
            //true positive
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var eqGoal = new EqGoal(x, 1); // x=1               
            bool result = point.Reify(eqGoal);

            Assert.True(result);

            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 1);
            Assert.True(point.CachedSymbols.Count == 1);
            var gPoint = point.CachedSymbols.ToList()[0] as Point;
            Assert.NotNull(gPoint);
            Assert.False(gPoint.Concrete);
            Assert.True(1.0.Equals(gPoint.XCoordinate));
            Assert.True(y.Equals(gPoint.YCoordinate));
            Assert.True(gPoint.CachedGoals.Count == 1);

            /******
             * current status:
             * (1,y)
             * 
             * next input:
             * x = 2
            *****/
            var eqGoal1 = new EqGoal(x, 2); // x=2
            result = point.Reify(eqGoal1);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 2);
            Assert.True(point.CachedSymbols.Count == 2);
            foreach (var shape in point.CachedSymbols)
            {
                Assert.False(shape.Concrete);
            }

            /******
            * current status:
            * (1,y)
            * (2,y)
            * 
            * next input:
            * y = 1
            *****/
            var eqGoal2 = new EqGoal(y, 1); // y = 1
            result = point.Reify(eqGoal2);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 3);
            Assert.True(point.CachedSymbols.Count == 2);
            foreach (var shape in point.CachedSymbols)
            {
                Assert.True(shape.Concrete);
            }

            /******
           * current status:
           * (1,1)
           * (2,1)
           * 
           * next input:
           * y = 2
           *****/
            var eqGoal3 = new EqGoal(y, 2); // y = 2
            result = point.Reify(eqGoal3);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 4);
            Assert.True(point.CachedSymbols.Count == 4);
            foreach (var shape in point.CachedSymbols)
            {
                Assert.True(shape.Concrete);
            }


            /******
            * current status:
            * (1,1)
            * (2,1)
            * (1,2)
            * (2,2)
            * next input:
            * x = 3
            *****/

            var eqGoal4 = new EqGoal(x, 3); // x = 3
            result = point.Reify(eqGoal4);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 5);
            Assert.True(point.CachedSymbols.Count == 6);
            foreach (var shape in point.CachedSymbols)
            {
                Assert.True(shape.Concrete);
                Assert.True(shape.CachedGoals.Count == 2);
            }

            /******
             * current status:
             * (1,1)
             * (2,1)
             * (3,1)
             * (1,2)
             * (2,2)
             * (3,2)
             * next input:
             * unreify y = 1
             *****/

            result = point.UnReify(eqGoal2);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 4);
            Assert.True(point.CachedSymbols.Count == 3);

            /******
           * current status:
           * (1,2)
           * (2,2)
           * (3,2)
           * next input:
           * unreify x = 2
           *****/
            result = point.UnReify(eqGoal1);
            Assert.True(result);
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 3);
            Assert.True(point.CachedSymbols.Count == 2);
        }

        #endregion
    }
}
