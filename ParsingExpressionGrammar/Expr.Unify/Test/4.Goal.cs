using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using starPadSDK.UnicodeNs;
using Text = starPadSDK.MathExpr.Text;
using starPadSDK.MathExpr;
using ExprSemantic;

namespace ExprPatternMatch
{
    [TestFixture]
    public class TestGoal
    {
        /*
         * 1. x=1 (ambiguity input) 
         * 2. x=y (Not goal, should be line)
         * 
         * 
         */ 

        [Test]
        public void Test0()
        {
            //x=1
            string txt = "x = 1";
            Expr expr = Text.Convert(txt);
            object obj = ExprVisitor.Instance.Match(expr);

            var lst = obj as Dictionary<PatternEnum, object>;
            Assert.NotNull(lst);
        }

        [Test]
        public void TEst0_1()
        {
            //x=y           
            string txt = "m1=m2";
            Expr expr = Text.Convert(txt);

            object obj;
            bool result = expr.IsEquationLabel(out obj);
            Assert.True(result);

            var eq = obj as Equation;
            Assert.NotNull(eq);

            result = eq.IsEqGoal(out obj);
            Assert.False(result);

            //object obj = ExprVisitor.Instance.Match(expr);
            /* Assert.IsInstanceOf(typeof(EqGoal), obj);
            var eqGoal = obj as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.IsInstanceOf(typeof(Var), eqGoal.Lhs);
            var variable = eqGoal.Lhs as Var;
            Assert.NotNull(variable);
            Assert.True(variable.Token.ToString().Equals("x"));
            Assert.IsInstanceOf(typeof(Var), eqGoal.Rhs);
            variable = eqGoal.Rhs as Var;
            Assert.NotNull(variable);
            Assert.True(variable.Token.ToString().Equals("y"));
            Assert.True(eqGoal.Traces.Count == 0);*/
        }

        public void Test1()
        {
            //x = 1 + 2
            string txt = "x = 1 + 2";
            Expr expr = Text.Convert(txt);
            object obj = ExprVisitor.Instance.Match(expr);
            Assert.IsInstanceOf(typeof(EqGoal), obj);
            var eqGoal = obj as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.IsInstanceOf(typeof(Var), eqGoal.Lhs);
            var variable = eqGoal.Lhs as Var;
            Assert.NotNull(variable);
            Assert.True(variable.Token.ToString().Equals("x"));
            Assert.True(eqGoal.Rhs.Equals(3));
            Assert.True(eqGoal.Traces.Count== 1);
 
            //x = 2-3+4*1
            txt = "x = 2-3+4*1";
            expr = Text.Convert(txt);
            obj = ExprVisitor.Instance.Match(expr);
            Assert.IsInstanceOf(typeof(EqGoal), obj);
            eqGoal = obj as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.IsInstanceOf(typeof(Var), eqGoal.Lhs);
            variable = eqGoal.Lhs as Var;
            Assert.NotNull(variable);
            Assert.True(variable.Token.ToString().Equals("x"));
            Assert.True(eqGoal.Rhs.Equals(3));
        }

        public void Test2()
        {
            //x + 1 = 1 + 2
            string txt = "x + 1 = 1 + 2";
            Expr expr = Text.Convert(txt);
            object obj = ExprVisitor.Instance.Match(expr);
            Assert.IsInstanceOf(typeof(EqGoal), obj);
            var eqGoal = obj as EqGoal;
            Assert.NotNull(eqGoal); 
            Assert.IsInstanceOf(typeof(Term), eqGoal.Lhs);
            var lTerm = eqGoal.Lhs as Term;
            Assert.NotNull(lTerm);
            var tuple = lTerm.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            var variable = tuple.Item1 as Var;
            Assert.NotNull(variable);
            Assert.True(variable.ToString().Equals("x"));
            Assert.True(tuple.Item2.Equals(1));
            Assert.True(eqGoal.Rhs.Equals(3));

            var dict = new Dictionary<object, object>();
            bool uResult = eqGoal.Unify(dict);
            Assert.True(uResult);
            Assert.True(dict.Count==1);
            Assert.True(dict.ContainsKey(variable));
            Assert.True(dict[variable].Equals(2));

            Assert.True(eqGoal.Traces.Count == 3);
        }

        [Test]
        public void Test3()
        {
            //y-3=10
            const string txt = "y-3=10";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquationLabel(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);

            result = eq.IsEqGoal(out obj);
            Assert.True(result);

            var eqGoal = obj as EqGoal;
            Assert.NotNull(eqGoal);

            Assert.True(eqGoal.Rhs.Equals(13));
        }  
      
    }
}
