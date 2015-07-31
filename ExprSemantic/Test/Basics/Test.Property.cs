using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgebraGeometry;
using AlgebraGeometry.Expr;
using ExprSemantic;
using NUnit.Framework;
using starPadSDK.MathExpr;
using Text = starPadSDK.MathExpr.Text;
using CSharpLogic;

namespace ExprSemanticTest
{
    public class TestProperty
    {
        [Test]
        public void Test0()
        {
            //x = 1
            const string fact1 = "x = 1";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetProperties();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
        }

        [Test]
        public void Test_Arithmetic()
        {
            //Add arithmetic to the property

            //a = 2-1
            const string fact1 = "a = 2-1";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetProperties();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var prop = result[0] as AGPropertyExpr;
            Assert.NotNull(prop);
            var goal = prop.Goal as EqGoal;
            Assert.True(goal.Traces.Count == 1);

            //a+1=1
            const string fact2 = "a+1=1";
            Reasoner.Instance.Unload(fact1);
            Reasoner.Instance.Load(fact2);
            result = Reasoner.Instance.TestGetProperties();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            prop = result[0] as AGPropertyExpr;
            Assert.NotNull(prop);
            goal = prop.Goal as EqGoal;
            Assert.NotNull(goal);
            Assert.True(goal.Traces.Count==2);

            //a+1=2*2
            const string fact3 = "a+1=2*2";
            Reasoner.Instance.Unload(fact2);
            Reasoner.Instance.Load(fact3);
            result = Reasoner.Instance.TestGetProperties();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            prop = result[0] as AGPropertyExpr;
            Assert.NotNull(prop);
            goal = prop.Goal as EqGoal;
            Assert.NotNull(goal);
            Assert.True(goal.Traces.Count == 3);
        }
    }
}
