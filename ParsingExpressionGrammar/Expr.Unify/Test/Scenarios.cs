/*******************************************************************************
 * Copyright (c) 2015 Bo Kang
 *   
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *  
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *******************************************************************************/

using starPadSDK.UnicodeNs;

namespace ExprPatternMatch
{
    using System;
    using AlgebraGeometry;
    using CSharpLogic;    
    using NUnit.Framework;
    using System.Collections.Generic;
    using Text = starPadSDK.MathExpr.Text;
    using starPadSDK.MathExpr;
    using System.Text.RegularExpressions;
    
    [TestFixture]
    public class TestExpression
    {
        [Test]
        public void Test_Numeric()
        {
            string txt = "3";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsNumeric(out obj);
            Assert.True(result);
            Assert.True(obj is int);
            Assert.True(3.Equals(obj));

            txt = "3.3";
            expr = Text.Convert(txt);
            result = expr.IsNumeric(out obj);
            Assert.True(result);
            Assert.True(obj is double);
            Assert.True(obj.Equals(3.3));

            txt = "-4";
            expr = Text.Convert(txt);
            result = expr.IsNumeric(out obj);
            Assert.True(result);
            Assert.True(obj is int);
            Assert.True(obj.Equals(-4));

            txt = "-4.4";
            expr = Text.Convert(txt);
            result = expr.IsNumeric(out obj);
            Assert.True(result);
            Assert.True(obj is double);
            Assert.True(obj.Equals(-4.4));

        }

        [Test]
        public void Test_Arith_1()
        {
            string txt = "1-1";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsExpression(out obj);
            Assert.True(result);
            Assert.True(obj is Term);
            var term = obj as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Add"));

            var lst = term.Args as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 2);
            Assert.True(LogicSharp.IsNumeric(lst[0]));
            Assert.True(LogicSharp.IsNumeric(lst[1]));

            //Term Eval
            obj = term.Eval();
            Assert.True(0.Equals(obj));
            Assert.True(term.Traces.Count == 1);
        }

        public void Test_numerics_2()
        {
            //2-1+3
            string txt = "2-1+3";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);
            Assert.IsInstanceOf(typeof(Term), result);

            var term = result as Term;
            Assert.NotNull(term);
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item2.Equals(3));
            var term1 = tuple.Item1 as Term;
            Assert.NotNull(term1);
            var tuple1 = term1.Args as Tuple<object, object>;
            Assert.NotNull(tuple1);
            Assert.True(tuple1.Item2.Equals(-1));
            Assert.True(tuple1.Item1.Equals(2));

            object evalResult = term.Eval();
            Assert.True(evalResult.Equals(4));
            Assert.True(term.Traces.Count == 2);
        }

        public void Test_numerics_3()
        {
            //2-3+4*1
            string txt = "2-3+4*1";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);
            Assert.IsInstanceOf(typeof(Term), result);

            var term = result as Term;
            Assert.NotNull(term);
            object evalResult = term.Eval();
            Assert.True(evalResult.Equals(3));
            Assert.True(term.Traces.Count == 3);
        }

        public void Test_numerics_4()
        {
            string txt = "1+2.1+3";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsExpression(out obj);
            Assert.True(result);
            Assert.True(obj is Term);
            var term = obj as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Add"));
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            var arg1 = tuple.Item1 as Term;
            var arg2 = tuple.Item2;
            Assert.NotNull(arg1);
            int number;
            Assert.True(LogicSharp.IsInt(arg2, out number));
            Assert.True(3.Equals(number));

            Assert.True(arg1.Op.Method.Name.Equals("Add"));
            var arg1Args = arg1.Args as Tuple<object, object>;
            Assert.NotNull(arg1Args);
            Assert.True(LogicSharp.IsInt(arg1Args.Item1, out number));
            Assert.True(1.Equals(number));
            double number2;
            Assert.True(LogicSharp.IsDouble(arg1Args.Item2, out number2));
            Assert.True(2.1.Equals(number2));
        }

        public void Test_numerics_5()
        {
            string txt = "1-1 + 2";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsExpression(out obj);
            Assert.True(result);
            Assert.True(obj is Term);
            var term = obj as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Add"));
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            var arg1 = tuple.Item1 as Term;
            var arg2 = tuple.Item2;
            Assert.NotNull(arg1);
            int number;
            Assert.True(LogicSharp.IsInt(arg2, out number));
            Assert.True(2.Equals(number));

            Assert.True(arg1.Op.Method.Name.Equals("Add"));
            var arg1Args = arg1.Args as Tuple<object, object>;
            Assert.NotNull(arg1Args);
            Assert.True(LogicSharp.IsInt(arg1Args.Item1, out number));
            Assert.True(1.Equals(number));
            Assert.True(LogicSharp.IsInt(arg1Args.Item2, out number));
            Assert.True(number.Equals(-1));
        }

        public void Test_numerics_6()
        {
            //2+4*1
            string txt = "2+4*1";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsExpression(out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(Term), obj);
            var term = obj as Term;
            Assert.NotNull(term);
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.Equals(2));
            var term1 = tuple.Item2 as Term;
            Assert.NotNull(term1);
            var tuple1 = term1.Args as Tuple<object, object>;
            Assert.NotNull(tuple1);
            Assert.True(tuple1.Item1.Equals(4));
            Assert.True(tuple1.Item2.Equals(1));
        }

        [Test]
        public void Test_numerics_7()
        {
            var expr1 = new CompositeExpr(WellKnownSym.plus,
                new Expr[] { new IntegerNumber("16"), new IntegerNumber("9") });
            var expr2 = new CompositeExpr(WellKnownSym.root,
                new Expr[] { new IntegerNumber("2"), expr1 });
            var expr3 = new CompositeExpr(WellKnownSym.equals,
                new Expr[] { new LetterSym('d'), expr2 });

            object obj;
            bool result = expr2.IsExpression(out obj);
            Assert.True(result);
            var gTerm = obj as Term;
            Assert.NotNull(gTerm);

            result = expr3.IsEquation(out obj);
            Assert.True(result);
            var gEquation = obj as Equation;
            Assert.NotNull(gEquation);
        }

        [Test]
        public void Test_numerics_8()
        {
            var expr1 = new CompositeExpr(WellKnownSym.divide,
                new Expr[] { new DoubleNumber(0.5) });

            var expr2 = new CompositeExpr(WellKnownSym.times,
                new Expr[] { -1, expr1 });

            object obj;
            bool result = expr2.IsExpression(out obj);
            Assert.True(result);
        }
    }

    [TestFixture]
    public class TestExpressionEval
    {
        [Test]
        public void Test_Label_1()
        {
            ///"A", "c", "XT","c12","c_1" 

            string txt = "A";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "c";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "XT";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "c12";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2m1";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);
        }

        [Test]
        public void Test_Label_2()
        {
            //-12x, -2x, 2x, x, 2y,2xy, -3y,2A, 12mm, 2XY, -2Y
            string txt = "-12x";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "-2x";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2x";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "x";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2y";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2xy";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "-3y";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2A";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "12mm";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2XY";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "-2Y";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);
        }

        [Test]
        public void Test_Label_3()
        {
            //ax, 2ax, -ax, -2ax, axy
            string txt = "ax";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "2ax";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "-ax";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "-2ax";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.False(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);

            txt = "axy";
            expr = Text.Convert(txt);
            result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);
        }

        [Test]
        public void Test_Label_4()
        {
            //TODO decimal Pattern Match
            //2.1x
            string txt = "2.1x";

            Match sss = Regex.Match(txt, @"([a-z]+)|([0-9]+)");
            foreach (Group gp in sss.Groups)
            {
                Console.Write(gp.Value);
            }

            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsLabel(out obj);
            Assert.True(result);
            result = expr.IsExpression(out obj);
            Assert.True(result);
        }

    }

    [TestFixture]
    public class TestLabel
    {
        [Test]
        public void Test1()
        {
            var expr0 = new CompositeExpr(WellKnownSym.times,
              new Expr[] { new LetterSym('m'), new IntegerNumber(1) });

            object output;
            bool result = expr0.IsLabel(out output);
            Assert.True(result);
        }
    }

    [TestFixture]
    public class TestExpressionAlgebra
    {
        #region Algebra pattern match and eval

        public void Test1()
        {
            //a+1+2
            string txt = "a+1+2";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);
            Assert.IsInstanceOf(typeof(Term), result);

            var term = result as Term;
            Assert.NotNull(term);
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item2.Equals(2));
            var term1 = tuple.Item1 as Term;
            Assert.NotNull(term1);
            var tuple1 = term1.Args as Tuple<object, object>;
            Assert.NotNull(tuple1);
            Assert.True(tuple1.Item2.Equals(1));
            var variable = tuple1.Item1 as Var;
            Assert.NotNull(variable);
            Assert.True(variable.ToString().Equals("x"));

            //term evaluation
            Assert.True(term.Traces.Count == 0);

        }

        public void Test_1()
        {
            string txt = "a+1";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsExpression(out obj);
            Assert.True(result);
            Assert.True(obj is Term);
            var term = obj as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Add"));
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            var arg1 = tuple.Item1 as Var;
            Assert.NotNull(arg1);
            Assert.True(arg1.ToString().Equals("x"));
            var arg2 = tuple.Item2;
            Assert.True(LogicSharp.IsNumeric(arg2));
        }

        #endregion
    }

    [TestFixture]
    public class TestEquation
    {
        [Test]
        public void Test_1()
        {
            string txt = "2=1-1";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
        }

        [Test]
        public void Test_2()
        {
            string txt = "1+1+1";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquation(out obj);
            Assert.False(result);
        }

        [Test]
        public void Test_3()
        {
            string txt = "d＾2 = 16+9";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
        }

        [Test]
        public void Test_4()
        {
            string txt = "m1*m2=-1";
            Expr expr = Text.Convert(txt);

            object obj;
            bool result = expr.IsEquationLabel(out obj);
            Assert.True(result);
        }

        [Test]
        public void Test_5()
        {
            var expr0 = new CompositeExpr(WellKnownSym.times,
                new Expr[] { new LetterSym('m'), new IntegerNumber(1) });

            var expr11 = new CompositeExpr(WellKnownSym.divide,
                new Expr[] { 0.5 });

            var expr1 = new CompositeExpr(WellKnownSym.times,
                new Expr[] { -1, expr11 });
            var expr2 = new CompositeExpr(WellKnownSym.equals,
                new Expr[] { expr0, expr1 });

            object obj;
            bool result = expr0.IsExpression(out obj);
            Assert.True(result);

            result = expr2.IsEquationLabel(out obj);
            Assert.True(result);
        }
    }

    [TestFixture]
    public class TestPoint
    {
        [Test]
        public void Test1()
        {
            string txt = "(1,1)";
            Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);

            Assert.IsInstanceOf<PointSymbol>(result);
            var ps = result as PointSymbol;
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("1"));
            Assert.True(ps.SymYCoordinate.Equals("1"));
            Assert.Null(ps.Shape.Label);

            string txt1 = "(-1,-1)";
            expr = Text.Convert(txt1);
            result = ExprVisitor.Instance.Match(expr);
            Assert.IsInstanceOf<PointSymbol>(result);
            ps = result as PointSymbol;
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("-1"));
            Assert.True(ps.SymYCoordinate.Equals("-1"));
            Assert.Null(ps.Shape.Label);
        }

        [Test]
        public void Test2()
        {
            string txt = "A(x,2)";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);
            var ps = result as PointSymbol;
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("x"));
            Assert.True(ps.SymYCoordinate.Equals("2"));
            Assert.True(ps.Shape.Label.Equals("A"));
            
            const string txt1 = "x = 2.0";
            expr = Text.Convert(txt1);
            result = ExprVisitor.Instance.Match(expr);
            Assert.NotNull(result);
            var dict = result as Dictionary<PatternEnum, object>;
            Assert.NotNull(dict);

            // Assert.IsInstanceOf(typeof(KeyValuePair<object,object>), result);
        }

        [Test]
        public void Test3()
        {
            string txt = "(-3.0,y)";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object result = ExprVisitor.Instance.Match(expr);
            var ps = result as PointSymbol;
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("-3"));
            Assert.True(ps.SymYCoordinate.Equals("y"));
            Assert.Null(ps.Shape.Label);
        }

        [Test]
        public void TestCoordinate()
        {
            string txt = "-3.6";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsCoordinateTerm(out obj);
            Assert.True(result);
            Assert.True(obj is double);
            Assert.True(obj.Equals(-3.6));

            txt = "3 = x";
            expr = Text.Convert(txt);
            result = expr.IsCoordinateTerm(out obj);
            Assert.True(result);
            Assert.IsInstanceOf(typeof(EqGoal), obj);
            //var dict = (KeyValuePair<object, object>)obj;
            var dict = (EqGoal)obj;
            Assert.NotNull(dict);
            
            Assert.True(dict.Lhs.ToString().Equals("x"));
            Assert.True(dict.Rhs.Equals(3));

            txt = "Y = 4.0";
            expr = Text.Convert(txt);
            result = expr.IsCoordinateTerm(out obj);
            Assert.True(result);
            Assert.NotNull(obj);
            Assert.IsInstanceOf(typeof(EqGoal), obj);
            dict = (EqGoal)obj;
            Assert.NotNull(dict);
            Assert.True(dict.Lhs.ToString().Equals("Y"));
            Assert.True(dict.Rhs.Equals(4.0));

        }

        [Test]
        public void Test_Point()
        {
            object x = 3;
            object y = -3.9;

            PointSymbol ps = PointPatternExtensions.CreatePointSymbol(x, y);
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("3"));
            Assert.True(ps.SymYCoordinate.Equals("-3.9"));
            Assert.True(ps.ToString().Equals("(3,-3.9)"));
            var pt = ps.Shape as Point;
            Assert.NotNull(pt);
            Assert.True(pt.Concrete);

            string label = "A";
            ps = PointPatternExtensions.CreatePointSymbol(label, x, y);
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("3"));
            Assert.True(ps.SymYCoordinate.Equals("-3.9"));
            Assert.True(ps.ToString().Equals("A(3,-3.9)"));
            pt = ps.Shape as Point;
            Assert.NotNull(pt);
            Assert.True(pt.Concrete);

            x = "X";
            y = "2";
            ps = PointPatternExtensions.CreatePointSymbol(x, y);
            Assert.NotNull(ps);
            Assert.True(ps.SymXCoordinate.Equals("X"));
            Assert.True(ps.SymYCoordinate.Equals("2"));
            Assert.True(ps.ToString().Equals("(X,2)"));
            pt = ps.Shape as Point;
            Assert.NotNull(pt);
            Assert.False(pt.Concrete);

            /*            
                       var dict  = new KeyValuePair<object, object>("m", 4);
                       var dict2 = new KeyValuePair<object, object>("n", 5);

                       x = dict;
                       y = dict2;
                       ps = ExprKnowledgeFactory.CreatePointSymbol(x, y);
                       Assert.NotNull(ps);
                       Assert.True(ps.SymXCoordinate.Equals("4"));
                       Assert.True(ps.SymYCoordinate.Equals("5"));
                       pt = ps.Shape as Point;
                       Assert.NotNull(pt);
                       Assert.True(pt.XCoordinate.Equals("m"));
                       Assert.True(pt.YCoordinate.Equals("n"));
                       Assert.True(ps.ToString().Equals("(4,5)"));
                       Assert.True(pt.Concrete); 
            */
        }
    }

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
            Assert.True(eqGoal.Traces.Count == 1);

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
            Assert.True(dict.Count == 1);
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

    [TestFixture]
    public class TestLine
    {
        /*
         * True Positive Test:
         * 
         *  1: x=2 
         *  2: y=1
         *  3: x+by+1=0
         *  4: 2x+y+1=0
         *  5: ax=2
         *  6: by=0
         *  7: cy=0
         *  8: -2.1x-3y-1=0
         *  9: -2x+3.2y+1=0
         *  10: ax-by+1=0
         *  11: -ax-by-9=0 
         *  12: 2x=1
         *  13: 2x+3y=1
         *  14: 3y-2x+1=0
         *  15: 2y+y-x+1=0
         *  16: y = 2x+1
         *  17: y = -x+3
         *  18: y = -ax+3
         *  19: a(x+2y-1=0)
         *  20: y = 3x+2
         *  21: y= 3x+k
         *  22: y-3=10
         *  23: 4y=x
         *  24: y=3x
         * 
         * False Negative Test:
         *  
         *  1: 2+1=0
         *  2: 2z+1=0
         */

        [Test]
        public void Test_Line_TruePositive_1()
        {
            //x=2 
            const string txt = "x=2";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            LineSymbol ls;
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals("1"));
            Assert.True(ls.ToString().Equals("x-2=0"));
        }

        [Test]
        public void Test_Line_TruePositive_2()
        {
            const string txt = "y=1";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.Null(ls.SymA);
            Assert.True(ls.SymB.Equals("1"));
            Assert.True(ls.ToString().Equals("y-1=0"));
        }

        [Test]
        public void Test_Line_TruePositive_3()
        {
            const string txt = "ax+by+1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals("a"));
            Assert.True(ls.SymB.Equals("b"));
            Assert.True(ls.ToString().Equals("ax+by+1=0"));
        }

        [Test]
        public void Test_Line_TruePositive_4()
        {
            const string txt = "2x+y+1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals("2"));
            Assert.True(ls.SymB.Equals("1"));
            Assert.True(ls.ToString().Equals("2x+y+1=0"));
        }

        [Test]
        public void Test_Line_TruePositive_5()
        {
            //ax=2
            var a = new Var('a');
            const string txt = "ax=2";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.NotNull(ls);
            Assert.True(ls.SymA.Equals(a.ToString()));
            Assert.Null(ls.SymB);
            Assert.True(ls.SymC.Equals("-2"));
        }

        [Test]
        public void Test_Line_TruePositive_6()
        {
            //by=0
            var b = new Var('b');
            const string txt = "by=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.Null(ls.SymA);
            Assert.True(ls.SymB.Equals(b.ToString()));
            Assert.True(ls.SymC.Equals("0"));
        }

        [Test]
        public void Test_Line_TruePositive_7()
        {
            //cy=0
            var c = new Var('c');
            const string txt = "cy=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.Null(ls.SymA);
            Assert.True(ls.SymB.Equals(c.ToString()));
            Assert.True(ls.SymC.Equals("0"));
        }

        [Test]
        public void Test_Line_TruePositive_8()
        {
            const string txt = "-2.1x-3y-1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("-2.1"));
            Assert.True(ls.SymB.Equals("-3"));
            Assert.True(ls.SymC.Equals("-1"));
        }

        [Test]
        public void Test_Line_TruePositive_9()
        {
            const string txt = "-2x+3.2y+1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("-2"));
            Assert.True(ls.SymB.Equals("3.2"));
            Assert.True(ls.SymC.Equals("1"));
        }

        public void Test_Line_TruePositive_10()
        {
            const string txt = "ax-by+1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("a"));
            Assert.True(ls.SymB.Equals("(-1*b)"));
            Assert.True(ls.SymC.Equals("1"));
        }

        public void Test_Line_TruePositive_11()
        {
            //11: -ax-by-9=0 
            const string txt = "-ax-by-9=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("(-1*a)"));
            Assert.True(ls.SymB.Equals("(-1*b)"));
            Assert.True(ls.SymC.Equals("-9"));
        }

        [Test]
        public void Test_Line_TruePositive_12()
        {
            //12: 2x=1
            const string txt = "2x=1";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("2"));
            Assert.Null(ls.SymB);
            Assert.True(ls.SymC.Equals("-1"));
        }

        [Test]
        public void Test_Line_TruePositive_13()
        {
            //13: 2x+3y=1
            const string txt = "2x+3y=1";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("2"));
            Assert.True(ls.SymB.Equals("3"));
            Assert.True(ls.SymC.Equals("-1"));
        }

        [Test]
        public void Test_Line_TruePositive_14()
        {
            //14: 3y-2x+1=0
            const string txt = "3y-2x+1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("-2"));
            Assert.True(ls.SymB.Equals("3"));
            Assert.True(ls.SymC.Equals("1"));
            Assert.True(ls.ToString().Equals("-2x+3y+1=0"));
        }

        [Test]
        public void Test_Line_TruePositive_15()
        {
            // 15: 2y+y-x+1=0
            const string txt = "2y+y-x+1=0";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymA.Equals("-1"));
            Assert.True(ls.SymB.Equals("3"));
            Assert.True(ls.SymC.Equals("1"));
        }

        [Test]
        public void Test_Line_TruePositive_16()
        {
            //16: y = 2x+1
            const string txt = "y=2x+1";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymSlope.Equals("2"));
            Assert.True(ls.SymIntercept.Equals("1"));
            Assert.True(ls.ToString().Equals("y=2x+1"));
            /*
                        Assert.True(ls.SymA.Equals("-1"));
                        Assert.True(ls.SymB.Equals("3"));
                        Assert.True(ls.SymC.Equals("1"));
            */
        }

        [Test]
        public void Test_Line_TruePositive_17()
        {
            //17: y = -x+3
            const string txt = "y=-x+3";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymSlope.Equals("-1"));
            Assert.True(ls.SymIntercept.Equals("3"));
            Assert.True(ls.ToString().Equals("y=-x+3"));
        }

        public void Test_Line_TruePositive_18()
        {
            //18: y = -ax-3
            const string txt = "y=-ax-3";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquationLabel(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.SymSlope.Equals("(-1*a)"));
            Assert.True(ls.SymIntercept.Equals("-3"));
            Assert.True(ls.ToString().Equals("y=(-1*a)x-3"));
        }

        [Test]
        public void Test_Line_TruePositive_19()
        {
            //19: ab(x+2y-1=0)
            const string txt = "ab(x+2y-1=0)";
            Expr expr = Text.Convert(txt);
            object obj;
            //LineSymbol ls;
            bool result = expr.IsEquationLabel(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            Assert.True(eq.EqLabel != null);
            Assert.True(eq.EqLabel.ToString().Equals("ab"));
        }

        [Test]
        public void Test_Line_TruePositive_20()
        {
            //20:  y = 3x+2
            const string txt = " y = 3x+2";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquationLabel(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);

            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.ToString().Equals("y=3x+2"));
        }

        [Test]
        public void Test_Line_TruePositive_21()
        {
            //21: y= 3x+k
            const string txt = "y=3x+k";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquationLabel(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);

            result = eq.IsLineEquation(out ls, false);
            Assert.True(result);
            Assert.True(ls.ToString().Equals("y=3x+k"));
        }

        [Test]
        public void Test_Line_TruePositive_22()
        {
            string a = "-10.6x";

            string b = a.Substring(1, 2);


            //22: y-3= 10
            const string txt = "y-3=10";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquationLabel(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);

            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.ToString().Equals("y-13=0"));
        }

        [Test]
        public void Test_Line_TruePositive_23()
        {
            //23. 4y=x
            const string txt = "4y=x";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquationLabel(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);

            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            Assert.True(ls.ToString().Equals("-x+4y=0"));
        }

        [Test]
        public void Test_Line_TruePositive_24()
        {
            //24: y=3x
            const string txt = "y=3x";
            Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquationLabel(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);

            result = eq.IsLineEquation(out ls);
            Assert.True(result);
            //            Assert.True(ls.ToString().Equals("-x+4y=0"));
        }

        [Test]
        public void Test_Line_FalseNegative_1()
        {
            //2+1=0
            const string txt = "2+1=0";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.False(result);
        }

        [Test]
        public void Test_Line_FalseNegative_2()
        {
            //2z+1=0
            const string txt = "2z+1=0";
            starPadSDK.MathExpr.Expr expr = Text.Convert(txt);
            object obj;
            LineSymbol ls;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            result = eq.IsLineEquation(out ls);
            Assert.False(result);
        }
    }

    [TestFixture]
    public class TestCircle
    {
        /*
         *  1: (x-2)^2+(y-2)^2=1
         *  2: (x+2)^2+(y+1)^2=4
         *  3: (y-2)^2+(x+1)^2=16
         *  4: x^2+y^2=1
         *  TODO 5: (x-a)^2+(y+2)^2=1
         *  TODO 6: x^2+y^2+2x+2y+4=0
         */
        [Test]
        public void Test_Circle_TruePositive_1()
        {
            //(x-2)^2+(y-2)^2=1
            const string txt = "(x-2)^2+(y-2)^2=1";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            CircleSymbol cs;
            result = eq.IsCircleEquation(out cs);
            Assert.True(result);
            Assert.NotNull(cs);
            Assert.True(cs.SymRadius.Equals("1"));
        }

        [Test]
        public void Test_Circle_TruePositive_2()
        {
            //(x+2)^2+(y+1)^2=4
            const string txt = "(x+2)^2+(y+1)^2=4";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            CircleSymbol cs;
            result = eq.IsCircleEquation(out cs);
            Assert.True(result);
            Assert.NotNull(cs);
            
            Assert.True(cs.SymRadius.Equals("2"));
            Assert.True(cs.SymCentral.SymXCoordinate.Equals("-2"));
            Assert.True(cs.SymCentral.SymYCoordinate.Equals("-1"));
        }

        [Test]
        public void Test_Circle_TruePositive_3()
        {
            //(y-2)^2+(x+1)^2=16
            const string txt = "(y-2)^2+(x+1)^2=16";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            CircleSymbol cs;
            result = eq.IsCircleEquation(out cs);
            Assert.True(result);
            Assert.NotNull(cs);

            Assert.True(cs.SymRadius.Equals("4"));
            Assert.True(cs.SymCentral.SymXCoordinate.Equals("-1"));
            Assert.True(cs.SymCentral.SymYCoordinate.Equals("2"));
        }

        [Test]
        public void Test_Circle_TruePositive_4()
        {
            //4: x^2+y^2=1
            const string txt = "x^2+y^2=1";
            Expr expr = Text.Convert(txt);
            object obj;
            bool result = expr.IsEquation(out obj);
            Assert.True(result);
            var eq = obj as Equation;
            Assert.NotNull(eq);
            CircleSymbol cs;
            result = eq.IsCircleEquation(out cs);
            Assert.True(result);
            Assert.NotNull(cs);

            Assert.True(cs.SymRadius.Equals("1"));
            Assert.True(cs.SymCentral.SymXCoordinate.Equals("0"));
            Assert.True(cs.SymCentral.SymYCoordinate.Equals("0"));
        }
    }

    [TestFixture]
    public class TestLineRelation
    {
        [Test]
        public void Test0()
        {
            const string txt3 = "AB";
            Expr expr3 = Text.Convert(txt3);
            object result = ExprVisitor.Instance.Match(expr3);

            var str = result as String;
            Assert.NotNull(str);
            /*            var dict = result as Dictionary<PatternEnum, object>;
                        Assert.NotNull(dict);
                        Assert.True(dict.Count==2);
                        Assert.True(dict.ContainsKey(PatternEnum.Label));
                        Assert.True(dict.ContainsKey(PatternEnum.Expression));
             */
        }
    }
}
