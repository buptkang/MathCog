using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CSharpLogic;
using NUnit.Framework;

namespace AlgebraGeometry.Test
{
    [TestFixture]
    public class TestLine
    {
        #region Line Checking

        [Test]
        public void Test_Check1()
        {
            var line = new Line(1, 2, 3);
            Assert.True(line.Concrete);

            Assert.True(line.A.Equals(1.0));
            Assert.True(line.B.Equals(2.0));
            Assert.True(line.C.Equals(3.0));

            var variable = new Var('x');
            var line2 = new Line(2, variable, 3);
            Assert.False(line2.Concrete);
        }

        #endregion

        #region Line Rounding Test

        [Test]
        public void Test_Point_Rounding_1()
        {
            var line = new Line(2.01111212, 3.12121211, 3.0000);
            Assert.True(line.A.Equals(2.0));
            Assert.True(line.B.Equals(3.1));
            Assert.True(line.C.Equals(3.0));
        }

        #endregion

        #region Line Symbolic

        [Test]
        public void test_symbolic_1()
        {
            //x+2y+3=0
            var line = new Line(1, 2, 3);
            var lineSymbol = new LineSymbol(line);
            string str = lineSymbol.ToString();
            Assert.True(str.Equals("x+2y+3=0"));

            //-x+2y+3=0
            line = new Line(-1, 2, 3);
            lineSymbol = new LineSymbol(line);
            str = lineSymbol.ToString();
            Assert.True(str.Equals("-x+2y+3=0"));

            //x+y+1=0
            line = new Line(1, 1, 1);
            lineSymbol = new LineSymbol(line);
            str = lineSymbol.ToString();
            Assert.True(str.Equals("x+y+1=0"));

            //x-y+1=0
            line = new Line(1, -1, 1);
            lineSymbol = new LineSymbol(line);
            str = lineSymbol.ToString();
            Assert.True(str.Equals("x-y+1=0"));

            //x-3y-2=0
            line = new Line(1, -3, -2);
            lineSymbol = new LineSymbol(line);
            str = lineSymbol.ToString();
            Assert.True(str.Equals("x-3y-2=0"));

            //x=0
            line = new Line(1, 0, 0);
            lineSymbol = new LineSymbol(line);
            str = lineSymbol.ToString();
            Assert.True(str.Equals("x=0"));

            //y=0
            line = new Line(0, 1, 0);
            lineSymbol = new LineSymbol(line);
            str = lineSymbol.ToString();
            Assert.True(str.Equals("y=0"));
        }

        [Test]
        public void test_symbolic_2()
        {
            //ax+2y-1=0
            var variable = new Var('a');
            var line = new Line(variable, 2, -1);
            var lineSymbol = new LineSymbol(line);
            string str = lineSymbol.ToString();
            Assert.True(str.Equals("ax+2y-1=0"));

            //x+by+c=0
            var variable2 = new Var('b');
            var variable3 = new Var('c');
            line = new Line(1, variable2, variable3);
            lineSymbol = new LineSymbol(line);
            str = lineSymbol.ToString();
            Assert.True(str.Equals("x+by+c=0"));

            //ax=0
            line = new Line(variable, 0, 0);
            lineSymbol = new LineSymbol(line);
            str = lineSymbol.ToString();
            Assert.True(str.Equals("ax=0"));

            //by+3=0
            line = new Line(0, variable2, 3);
            lineSymbol = new LineSymbol(line);
            str = lineSymbol.ToString();
            Assert.True(str.Equals("by+3=0"));

            //x-by+1=0 => not allowed -> Exception
            var term =  new Term(Expression.Multiply, 
                new Tuple<object, object>(-1, variable2));
            line = new Line(1, term, 3);
            lineSymbol = new LineSymbol(line);
            str = lineSymbol.ToString();
            Assert.True(str.Equals("x+(-1*b)y+3=0"));
        }

        [Test]
        public void test_symbolic_label()
        {
            //ax+2y-1=0
            var variable = new Var('a');
            var line = new Line(variable, 2, -1);
            var lineSymbol = new LineSymbol(line);
            string str = lineSymbol.ToString();
            Assert.True(str.Equals("ax+2y-1=0"));

            line.Label = "M";
            Assert.True(lineSymbol.ToString().Equals("M(ax+2y-1=0)"));
        }

        #endregion

        #region Line Reify and DynamicObject Reify

        [Test]
        public void test_reify_1()
        {
            /*
             * //ax+by+c=0
             *  b = 1
             *  a = 2
             */
            var variable = new Var('a');
            var variable2 = new Var('b');
            var variable3 = new Var('c');
            var line = new Line(variable, variable2, variable3);

            //a = 2
            var goal = new EqGoal(variable, 2.0);
            line.Reify(goal);
            Assert.False(line.Concrete);
            Assert.True(line.CachedSymbols.Count == 1);
            Assert.True(line.CachedGoals.Count == 1);

            var gline = line.CachedSymbols.ToList()[0] as Line;
            Assert.NotNull(gline);
            Assert.True(gline.Traces.Count == 1);
            var ts = gline.Traces[0] as TraceStep;
            Assert.NotNull(ts);
            var source = ts.Source as LineSymbol;
            Assert.NotNull(source);
            Assert.True(source.ToString().Equals("ax+by+c=0"));
            var target = ts.Target as LineSymbol;
            Assert.NotNull(target);
            Assert.True(target.ToString().Equals("2x+by+c=0"));

            ///////////////////////////////////////////////////////

            line = new Line(variable, variable2, variable3);

            //b = 1
            goal = new EqGoal(variable2, 1);
            line.Reify(goal);
            Assert.False(line.Concrete);
            Assert.True(line.CachedSymbols.Count == 1);
            Assert.True(line.CachedGoals.Count == 1);

            gline = line.CachedSymbols.ToList()[0] as Line;
            Assert.NotNull(gline);
            Assert.True(gline.Traces.Count == 1);
            ts = gline.Traces[0] as TraceStep;
            Assert.NotNull(ts);
            source = ts.Source as LineSymbol;
            Assert.NotNull(source);
            Assert.True(source.ToString().Equals("ax+by+c=0"));
            target = ts.Target as LineSymbol;
            Assert.NotNull(target);
            Assert.True(target.ToString().Equals("ax+y+c=0"));

            ///////////////////////////////////////////////////////

            line = new Line(variable, variable2, variable3);

            //c = -3.2
            goal = new EqGoal(variable3, -3.2);
            line.Reify(goal);
            Assert.False(line.Concrete);
            Assert.True(line.CachedSymbols.Count == 1);
            Assert.True(line.CachedGoals.Count == 1);

            gline = line.CachedSymbols.ToList()[0] as Line;
            Assert.NotNull(gline);
            Assert.True(gline.Traces.Count == 1);
            ts = gline.Traces[0] as TraceStep;
            Assert.NotNull(ts);
            source = ts.Source as LineSymbol;
            Assert.NotNull(source);
            Assert.True(source.ToString().Equals("ax+by+c=0"));
            target = ts.Target as LineSymbol;
            Assert.NotNull(target);
            Assert.True(target.ToString().Equals("ax+by-3.2=0"));

        }

        [Test]
        public void test_reify_2()
        {
            /*
             * //ax+by+c=0
             *  b = 1
             *  a = 2
             */
            var variable = new Var('a');
            var variable2 = new Var('b');
            var variable3 = new Var('c');
            var line = new Line(variable, variable2, variable3);

            //a = 2
            var goal = new EqGoal(variable, 2.0);
            line.Reify(goal);
            Assert.False(line.Concrete);
            Assert.True(line.CachedSymbols.Count == 1);
            Assert.True(line.CachedGoals.Count == 1);

            var gline = line.CachedSymbols.ToList()[0] as Line;
            Assert.NotNull(gline);
            Assert.True(gline.Traces.Count == 1);
            var ts = gline.Traces[0] as TraceStep;
            Assert.NotNull(ts);
            var source = ts.Source as LineSymbol;
            Assert.NotNull(source);
            Assert.True(source.ToString().Equals("ax+by+c=0"));
            var target = ts.Target as LineSymbol;
            Assert.NotNull(target);
            Assert.True(target.ToString().Equals("2x+by+c=0"));

            //b = 1
            var goal2 = new EqGoal(variable2, 1);
            line.Reify(goal2);
            Assert.False(line.Concrete);
            Assert.True(line.CachedSymbols.Count == 1);
            Assert.True(line.CachedGoals.Count == 2);

            gline = line.CachedSymbols.ToList()[0] as Line;
            Assert.NotNull(gline);
            Assert.True(gline.Traces.Count == 2);
            ts = gline.Traces[0] as TraceStep;
            Assert.NotNull(ts);
            target = ts.Target as LineSymbol;
            Assert.NotNull(target);
            Assert.True(target.ToString().Equals("2x+y+c=0"));
        }

        [Test]
        public void test_unreify_1()
        {
            var variable = new Var('a');
            var variable2 = new Var('b');
            var variable3 = new Var('c');
            var line = new Line(variable, variable2, variable3);

            //a = 2
            var goal = new EqGoal(variable, 2.0);
            line.Reify(goal);
            Assert.False(line.Concrete);
            Assert.True(line.CachedSymbols.Count == 1);
            Assert.True(line.CachedGoals.Count == 1);

            line.UnReify(goal);
            Assert.True(line.CachedSymbols.Count == 0);
            Assert.True(line.CachedGoals.Count == 0);
        }

        [Test]
        public void test_unreify_2()
        {
            /*
            * //ax+by+c=0
            *  b = 1
            *  a = 2
            */
            var variable = new Var('a');
            var variable2 = new Var('b');
            var variable3 = new Var('c');
            var line = new Line(variable, variable2, variable3);

            //a = 2
            var goal = new EqGoal(variable, 2.0);
            line.Reify(goal);
            Assert.False(line.Concrete);
            Assert.True(line.CachedSymbols.Count == 1);
            Assert.True(line.CachedGoals.Count == 1);

            var gline = line.CachedSymbols.ToList()[0] as Line;
            Assert.NotNull(gline);
            Assert.True(gline.Traces.Count == 1);
            var ts = gline.Traces[0] as TraceStep;
            Assert.NotNull(ts);
            var source = ts.Source as LineSymbol;
            Assert.NotNull(source);
            Assert.True(source.ToString().Equals("ax+by+c=0"));
            var target = ts.Target as LineSymbol;
            Assert.NotNull(target);
            Assert.True(target.ToString().Equals("2x+by+c=0"));

            //b = 1
            var goal2 = new EqGoal(variable2, 1);
            line.Reify(goal2);
            Assert.False(line.Concrete);
            Assert.True(line.CachedSymbols.Count == 1);
            Assert.True(line.CachedGoals.Count == 2);

            line.UnReify(goal2);
            Assert.True(line.CachedSymbols.Count == 1);
            Assert.True(line.CachedGoals.Count == 1);
        }

        #endregion 

        #region Line Property

        [Test]
        public void test_property_1()
        {
            var line = new Line(1, 2, 3);
            Assert.True(line.Concrete);

            Assert.True(line.A.Equals(1.0));
            Assert.True(line.B.Equals(2.0));
            Assert.True(line.C.Equals(3.0));

            //slope = ?
        }

        [Test]
        public void test_property_2()
        {
            var variable = new Var('x');
            var line2 = new Line(2, variable, 3);
            Assert.False(line2.Concrete);

            //slope = 2.0
            //Reify
                        
        }


        #endregion
    }
}
