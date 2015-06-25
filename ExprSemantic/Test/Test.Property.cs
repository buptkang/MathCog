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
            var reasoner = new Reasoner();
            //x = 1
            const string fact1 = "x = 1";
            reasoner.Load(fact1);
            var result = reasoner.TestGetProperties();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
        }

        [Test]
        public void Test_Arithmetic()
        {
            //Add arithmetic to the property

            var reasoner = new Reasoner();

            //a = 2-1
            const string fact1 = "a = 2-1";
            reasoner.Load(fact1);
            var result = reasoner.TestGetProperties();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var prop = result[0] as AGPropertyExpr;
            Assert.NotNull(prop);
            var goal = prop.Goal as EqGoal;
            Assert.True(goal.Traces.Count == 1);

            //a+1=1
            const string fact2 = "a+1=1";
            reasoner.Unload(fact1);
            reasoner.Load(fact2);
            result = reasoner.TestGetProperties();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            prop = result[0] as AGPropertyExpr;
            Assert.NotNull(prop);
            goal = prop.Goal as EqGoal;
            Assert.NotNull(goal);
            Assert.True(goal.Traces.Count==2);

            //a+1=2*2
            const string fact3 = "a+1=2*2";
            reasoner.Unload(fact2);
            reasoner.Load(fact3);
            result = reasoner.TestGetProperties();
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
