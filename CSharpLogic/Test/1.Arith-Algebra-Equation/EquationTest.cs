﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NUnit.Framework;

namespace CSharpLogic
{
    [TestFixture]
    public class EquationTest
    {
        #region Arithmetic Only

        [Test]
        public void Test_Arith_1()
        {
            //1+2=3
            var lhs = new Term(Expression.Add, new List<object>() { 1, 2 });
            var equation = new Equation(lhs, 3);
            bool result = equation.ContainsVar();
            Assert.False(result);
            Assert.True(equation.ToString().Equals("(1+2)=3"));

            Equation outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.NotNull(evalResult);
            Assert.True(evalResult.Value);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("3=3"));
            Assert.True(equation.Traces.Count == 1);
        }

        [Test]
        public void Test_Arith_2()
        {
            //1+2=4
            var lhs = new Term(Expression.Add, new List<object>() { 1, 2 });
            var equation = new Equation(lhs, 4);
            bool result = equation.ContainsVar();
            Assert.False(result);
            Assert.True(equation.ToString().Equals("(1+2)=4"));

            Equation outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.NotNull(evalResult);
            Assert.False(evalResult.Value);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("3=4"));
        }

        [Test]
        public void Test_Arith_3()
        {
            //1+2+3=6
            var lhs = new Term(Expression.Add, new List<object>() { 1, 2, 3 });
            var equation = new Equation(lhs, 6);
            bool result = equation.ContainsVar();
            Assert.False(result);
            Assert.True(equation.ToString().Equals("(1+2+3)=6"));

            Equation outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.NotNull(evalResult);
            Assert.True(evalResult.Value);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("6=6"));
            Assert.True(equation.Traces.Count == 2);
        }

        [Test]
        public void Test_Arith_4()
        {
            //1*2*3=7
            var lhs = new Term(Expression.Multiply, new List<object>() { 1, 2, 3 });
            var equation = new Equation(lhs, 7);
            bool result = equation.ContainsVar();
            Assert.False(result);
            Assert.True(equation.ToString().Equals("(1*2*3)=7"));

            Equation outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.NotNull(evalResult);
            Assert.False(evalResult.Value);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("6=7"));
            Assert.True(equation.Traces.Count == 2);
        }

        #endregion

        #region Algebra

        [Test]
        public void Test_Algebra_1()
        {
            //1+2=x
            var x = new Var('x');
            var lhs = new Term(Expression.Add, new List<object>() { 1, 2 });
            var equation = new Equation(lhs, x);
            bool result = equation.ContainsVar();
            Assert.True(result);
            Assert.True(equation.ToString().Equals("(1+2)=x"));

            Equation outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.Null(evalResult);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("((1*x)-3)=0"));
            Assert.True(equation.Traces.Count == 7);
        }

        [Test]
        public void Test_Algebra_2()
        {
            //x+2=3
            var x = new Var('x');
            var lhs = new Term(Expression.Add, new List<object>() { x, 2 });
            var equation = new Equation(lhs, 3);
            bool result = equation.ContainsVar();
            Assert.True(result);
            Assert.True(equation.ToString().Equals("(x+2)=3"));

            Equation outputEq;
            bool? evalResult = equation.Eval(out outputEq);
            Assert.Null(evalResult);
            Assert.NotNull(outputEq);
            Assert.True(outputEq.ToString().Equals("(x-1)=0"));
            Assert.True(equation.Traces.Count == 7);
        }

        [Test]
        public void Test_Algebra_3()
        {


        }

        #endregion

        #region Expression Test

        [Test]
        public void Test_Expression_1()
        {
            //1+1
            var term = new Term(Expression.Add, new List<object>() { 1, 1 });
            var equation = new Equation(term);
            //Assert.True(equation.IsExpression);
            Assert.True(equation.ToString().Equals("(1+1)"));

            //1+1+1
            term = new Term(Expression.Add, new List<object>() { 1, 1, 1 });
            equation = new Equation(term);
            //Assert.True(equation.IsExpression);
            Assert.True(equation.ToString().Equals("(1+1+1)"));
        }

        #endregion

        #region Equation Test

        [Test]
        public void Test_Equation_1()
        {
            object pt1XCoord = 1.0;
            object pt1YCoord = 2.0;
            object pt2XCoord = 4.0;
            object pt2YCoord = 6.0;

            var variable = new Var('d');
            var lhs = new Term(Expression.Power, new List<object>() { variable, 2.0});

            var term1 = new Term(Expression.Subtract, new List<object>() { pt1XCoord, pt2XCoord });
            var term11 = new Term(Expression.Power, new List<object>() { term1, 2.0 });

            var term2 = new Term(Expression.Subtract, new List<object>() { pt1YCoord, pt2YCoord });
            var term22 = new Term(Expression.Power, new List<object>() { term2, 2.0});

            var rhs = new Term(Expression.Add, new List<object>() { term11, term22 }); 
            //var obj = rhs.Eval();

            //Assert.True(obj.Equals(25));
            var eq = new Equation(lhs, rhs);
            EqGoal goal;
            bool result = eq.IsEqGoal(out goal);
            Assert.True(result);
            Assert.NotNull(goal);
            Assert.True(goal.Rhs.Equals(5));
        }

        #endregion
    }
}
