using System.Linq;
using AlgebraGeometry;
using AlgebraGeometry.Expr;
using CSharpLogic;
using ExprSemantic;
using NUnit.Framework;

namespace MathReason
{
    [TestFixture]
    public class TestEquation
    {
        [Test]
        public void Test_Equation_1()
        {
            const string input1 = "1+2=3";
            var obj = Reasoner.Instance.Load(input1);
            var eqExpr = obj as AGEquationExpr;
            Assert.NotNull(eqExpr);
            var equation = eqExpr.Equation;
            Assert.NotNull(equation);

            //Answer
            Assert.True(equation.CachedEntities.Count == 1);
            var cachedEq = equation.CachedEntities.ToList()[0] as bool?;
            Assert.NotNull(cachedEq);
            Assert.True(cachedEq.Value);

            //How to procedure
            eqExpr.GenerateSolvingTrace();




            //Assert.NotNull(traces);
            //Assert.True(traces.Count ==1);
        }

        [Test]
        public void Test_Equation_2()
        {
            const string input1 = "1+2=4";
            var obj = Reasoner.Instance.Load(input1);
            var eqExpr = obj as AGEquationExpr;
            Assert.NotNull(eqExpr);
            var equation = eqExpr.Equation;
            Assert.NotNull(equation);

            Assert.True(equation.CachedEntities.Count == 1);
            var cachedEq = equation.CachedEntities.ToList()[0] as bool?;
            Assert.NotNull(cachedEq);
            Assert.False(cachedEq.Value);
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
            Assert.True(goal.Traces.Count == 2);

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