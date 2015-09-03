using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Text = starPadSDK.MathExpr.Text;
using starPadSDK.MathExpr;
using ExprSemantic;

namespace ExprPatternMatch
{
    [TestFixture]
    public class TestGoal
    {
        [Test]
        public void Test0()
        {
            //x=1
            string txt = "x = 1";
            Expr expr = Text.Convert(txt);
            object obj = ExprVisitor.Instance.Match(expr);
            Assert.IsInstanceOf(typeof(EqGoal), obj);
            var eqGoal = obj as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.IsInstanceOf(typeof(Var), eqGoal.Lhs);
            var variable = eqGoal.Lhs as Var;
            Assert.NotNull(variable);
            Assert.True(variable.Token.ToString().Equals("x"));
            Assert.True(eqGoal.Rhs.Equals(1));

            //x=y           
            txt = "x = y";
            expr = Text.Convert(txt);
            obj = ExprVisitor.Instance.Match(expr);
            Assert.IsInstanceOf(typeof(EqGoal), obj);
            eqGoal = obj as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.IsInstanceOf(typeof(Var), eqGoal.Lhs);
            variable = eqGoal.Lhs as Var;
            Assert.NotNull(variable);
            Assert.True(variable.Token.ToString().Equals("x"));
            Assert.IsInstanceOf(typeof(Var), eqGoal.Rhs);
            variable = eqGoal.Rhs as Var;
            Assert.NotNull(variable);
            Assert.True(variable.Token.ToString().Equals("y"));
            Assert.True(eqGoal.Traces.Count == 0);
        }

        [Test]
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

        [Test]
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

        public void Test_Symmetric()
        {
            /*
            //1 = x
            txt = "1 = x";
            expr = Text.Convert(txt);
            result = expr.IsGoal(out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(EqGoal), obj);
            eqGoal = obj as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.IsInstanceOf(typeof(Var), eqGoal.Rhs);
            variable = eqGoal.Rhs as Var;
            Assert.NotNull(variable);
            Assert.True(variable.Token.ToString().Equals("x"));
            Assert.True(eqGoal.Lhs.Equals(1));
            Assert.True(eqGoal.Traces.Count == 0);
             */ 
        }

        public void Test7()
        {
            //TODO
            //associative rule
            /*
            txt = "m + 1 -2 = y";
            expr = Text.Convert(txt);
            result = expr.IsGoal(out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(EqGoal), obj);
            eqGoal = obj as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.IsInstanceOf(typeof(Term), eqGoal.Lhs);
            var term = eqGoal.Lhs as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Add"));
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            variable = tuple.Item1 as Var;
            Assert.NotNull(variable);
            Assert.True(variable.Token.ToString().Equals("m"));
            Assert.True(tuple.Item2.Equals(-1));

            Assert.IsInstanceOf(typeof(Var), eqGoal.Rhs);
            variable = eqGoal.Rhs as Var;
            Assert.NotNull(variable);
            Assert.True(variable.Token.ToString().Equals("y"));

            Assert.True(eqGoal.TraceCount == 1);
            */

            //1+y = m+1

        }
    }
}
