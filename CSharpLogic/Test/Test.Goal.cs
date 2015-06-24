using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpLogic.Test
{
    [TestFixture]
    public class TestGoal
    {
        #region Goal and Expression Conversion

        [Test]
        public void TestEqGoal_Expression0()
        {
            var term = new Term(Expression.Add, new Tuple<object, object>(1, 1));
            var eqGoal = new EqGoal(term);
            Assert.True(eqGoal.IsExpression);
        }

        [Test]
        public void TestEqGoal_Expression1()
        {
            //1+1
            var term = new Term(Expression.Add, new Tuple<object, object>(1, 1));
            var eqGoal = new EqGoal(term);
            Assert.True(eqGoal.Traces.Count == 1);
            Assert.True(eqGoal.Lhs.Equals(2));

            //1+1+1
            var internalTerm = new Term(Expression.Add, new Tuple<object, object>(1, 1));
            term = new Term(Expression.Add, new Tuple<object, object>(internalTerm, 1));
            eqGoal = new EqGoal(term);
            Assert.True(eqGoal.Traces.Count == 2);
            Assert.True(eqGoal.Lhs.Equals(3));
        }

        #endregion

        #region Goal Basic

        [Test]
        public void TestEqGoal0()
        {
            //2=2
            var eqGoal = new EqGoal(2, 2);
            var substitutions = new Dictionary<object, object>();
            bool result = eqGoal.Unify(substitutions);
            Assert.True(result);
            Assert.True(substitutions.Count == 0);

            //3=4
            eqGoal = new EqGoal(3, 4);
            substitutions = new Dictionary<object, object>();
            result = eqGoal.Unify(substitutions);
            Assert.False(result);

            //3=5-2
            var rhs = new Term(Expression.Subtract, new Tuple<object, object>(5, 2));
            eqGoal = new EqGoal(3, rhs);
            substitutions = new Dictionary<object, object>();
            result = eqGoal.Unify(substitutions);
            Assert.True(result);
            Assert.True(eqGoal.Traces.Count == 1);

            //x = 2
            var x = new Var('x');
            eqGoal = new EqGoal(x, 3);
            substitutions = new Dictionary<object, object>();
            result = eqGoal.Unify(substitutions);
            Assert.True(result);
            Assert.True(eqGoal.Traces.Count == 0);
            Assert.True(substitutions.Count == 1);
            Assert.True(substitutions.ContainsKey(x));
            Assert.True(substitutions[x].Equals(3));
            Assert.True(eqGoal.EarlySafe());

            //x = x
            eqGoal = new EqGoal(x, x);
            substitutions = new Dictionary<object, object>();
            result = eqGoal.Unify(substitutions);
            Assert.True(result);
            Assert.True(eqGoal.Traces.Count == 0);
            Assert.True(substitutions.Count == 0);

            //x = y
            var y = new Var('y'); 
            eqGoal = new EqGoal(x, y);
            substitutions = new Dictionary<object, object>();
            result = eqGoal.Unify(substitutions);
            Assert.True(result);
            Assert.True(eqGoal.Traces.Count == 0);
            Assert.True(substitutions.Count == 1);
            Assert.False(eqGoal.EarlySafe());
        }

        [Test]
        public void TestEqGoal2()
        {
            var variable1 = new Var('x');
            var variable2 = new Var('x');
            var eqGoal = new EqGoal(variable1, 2);
            Assert.True(eqGoal.ContainsVar(variable2));
        }

        [Test]
        public void TestEqGoal3()
        {
            var variable1 = new Var('y');
            var variable2 = new Var('x');
            var eqGoal = new EqGoal(variable1, 2);
            Assert.False(eqGoal.ContainsVar(variable2));
        }


        #endregion

        #region Goal Evaluation

        [Test]
        public void TestEqGoal_Eval0()
        {
            //x + 1 = 2
            var x = new Var('x');
            var lhs = new Term(Expression.Add, new Tuple<object, object>(x, 1));
            var goal = new EqGoal(lhs, 2);           
            Assert.True(goal.TraceCount == 2);
            var ts = goal.Traces[0] as TraceStep;
            var latest = ts.Target as EqGoal;
            Assert.NotNull(latest);
            Assert.True(latest.ToString().Equals("x=1"));

            var substitutions = new Dictionary<object, object>();
            bool result = goal.Unify(substitutions);
            Assert.True(result);
            Assert.True(substitutions.Count == 1);
            Assert.True(substitutions.ContainsKey(x));
            Assert.True(substitutions[x].Equals(1));

            //////////////////////////////////////////////////

            /*
             * x + 1 - 3 = 2
             */
            
            var internLsh = new Term(Expression.Subtract, new Tuple<object,object>(1, 3));
            lhs = new Term(Expression.Add, new Tuple<object, object>(x,internLsh));
            goal = new EqGoal(lhs, 2);
            Assert.True(goal.Traces.Count == 3);
            ts = goal.Traces[0] as TraceStep;
            latest = ts.Target as EqGoal;
            Assert.NotNull(latest);
            Assert.True(latest.ToString().Equals("x=4"));

            substitutions = new Dictionary<object, object>();
            result = goal.Unify(substitutions);
            Assert.True(result);
            Assert.True(substitutions.Count == 1);
            Assert.True(substitutions.ContainsKey(x));
            Assert.True(substitutions[x].Equals(4));
           
            ///////////////////////////////////////////////////
            
            // x + 1 - 5　＝　2+1
            internLsh = new Term(Expression.Subtract, new Tuple<object, object>(1, 5));
            lhs = new Term(Expression.Add, new Tuple<object, object>(x, internLsh));
            var rhs = new Term(Expression.Add, new Tuple<object, object>(2, 1));
            goal = new EqGoal(lhs, rhs);
            Assert.True(goal.Traces.Count == 4);
            ts = goal.Traces[0] as TraceStep;
            latest = ts.Target as EqGoal;
            Assert.NotNull(latest);
            Assert.True(latest.ToString().Equals("x=7"));

            substitutions = new Dictionary<object, object>();
            result = goal.Unify(substitutions);
            Assert.True(result);
            Assert.True(substitutions.Count == 1);
            Assert.True(substitutions.ContainsKey(x));
            Assert.True(substitutions[x].Equals(7));           
        }

        #endregion

        #region Reification

        [Test]
        public void test_reify_object2()
        {
            var foo = new DyLogicObject();
            foo.Properties.Add("y", 1);
            var goal = new EqGoal(new Var("x"), 2);
            foo.Reify(goal);

            Assert.True(foo.Properties.Count == 2);
        }

        #endregion 
    }
}
