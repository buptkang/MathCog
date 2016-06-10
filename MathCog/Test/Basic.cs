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

using starPadSDK.MathExpr;

namespace MathCog
{
    using CSharpLogic;
    using NUnit.Framework;
    using AlgebraGeometry;
    using ExprPatternMatch;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class TestBasic
    {
        #region Equation

        [Test]
        public void Test_Equation_1()
        {
            const string input1 = "1+2=3";
            var obj = Reasoner.Instance.Load(input1);
            var eqExpr = obj as AGEquationExpr;
            Assert.NotNull(eqExpr);
            var equation = eqExpr.Equation;
            Assert.NotNull(equation);

         /*   //Answer
            Assert.True(equation.CachedEntities.Count == 1);
            var cachedEq = equation.CachedEntities.ToList()[0] as bool?;
            Assert.NotNull(cachedEq);
            Assert.True(cachedEq.Value);

            //How to procedure
            eqExpr.GenerateSolvingTrace();
*/



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
       /*     Assert.NotNull(equation);

            Assert.True(equation.CachedEntities.Count == 1);
            var cachedEq = equation.CachedEntities.ToList()[0] as bool?;
            Assert.NotNull(cachedEq);
            Assert.False(cachedEq.Value);*/
        }

        [Test]
        public void Test_Equation_3()
        {
            const string fact1 = "2x+5x-6+y-2y=0";
            var obj = Reasoner.Instance.Load(fact1) as AGShapeExpr;
            Assert.NotNull(obj);
            var ls = obj.ShapeSymbol as LineSymbol;
            Assert.NotNull(ls);
            Assert.True(ls.SymSlope.Equals("7"));
        }

        [Test]
        public void Test_Equation_4()
        {
            Expr expr = ExprMock.Mock4();
            object obj;
            bool result = expr.IsEquationLabel(out obj);

            Assert.True(result);
        }


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
            bool result1 = Reasoner.Instance.Unload(fact1);
            Assert.True(result1);
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

        #endregion

        #region Point Shape

        #region Point Pattern Match (Unification)

        [Test]
        public void Test_PatternMatch0()
        {
            /*
             * (1,1)
             */
            const string fact1 = "(1,1)";
            var result = Reasoner.Instance.Load(fact1);
            var shapeExpr = result as AGShapeExpr;
            Assert.NotNull(shapeExpr);

            List<AGShapeExpr> results = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(results);
            Assert.True(results.Count == 1);
            var ps = results[0];
            Assert.NotNull(ps);
            var pt = ps.ShapeSymbol.Shape as Point;
            Assert.NotNull(pt);
            Assert.True(pt.Concrete);
            Assert.True(ps.ShapeSymbol.CachedSymbols.Count == 0);

            /*
             * Ask this point's X Coordinate
             * Query: X=?
             * =>
             * Answer: X=2
             */
            const string query1 = "x=";
            var queryExpr = Reasoner.Instance.Load(query1) as AGQueryExpr;
            Assert.NotNull(queryExpr);
            var queryTag = queryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.CachedEntities.Count == 1);
            var eqGoal = queryTag.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(eqGoal);
            Assert.True(eqGoal.Rhs.Equals(1.0));
        }

        public void Test_PatternMatch1()
        {
            /*
             *  A(v,2)
             */

            const string fact1 = "A(v,2)";
            var result = Reasoner.Instance.Load(fact1);
            var shapeExpr = result as AGShapeExpr;
            Assert.NotNull(shapeExpr);

            List<AGShapeExpr> results = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(results);
            Assert.True(results.Count == 1);
            var ps = results[0];
            Assert.NotNull(ps);
            var pt = ps.ShapeSymbol.Shape as Point;
            Assert.NotNull(pt);
            Assert.False(pt.Concrete);
            Assert.True(ps.ShapeSymbol.CachedSymbols.Count == 0);

            /*
             * v = 5
             * 
             */
            const string fact2 = "v=5";
            var propExpr = Reasoner.Instance.Load(fact2) as AGPropertyExpr;
            Assert.NotNull(propExpr);
            Assert.True(shapeExpr.ShapeSymbol.CachedSymbols.Count == 1);
        }

        [Test]
        public void Test_PatternMatch2()
        {
            /*
             *  (0,-4)
             */
            const string fact1 = "(0,-4)";
            var result = Reasoner.Instance.Load(fact1);
            var shapeExpr = result as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            Assert.True(shapeExpr.ShapeSymbol.ToString().Equals("(0,-4)"));

            var ps = shapeExpr.ShapeSymbol as PointSymbol;
            Expr expr = ps.ToExpr();
            Assert.NotNull(expr);

        }


        #endregion

        #region Point Reification(Substitution)

        public void Test_Substitution_0()
        {
            ///////////////////////////////////////////////////////
            const string fact1 = "(x,y)";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ps = result[0];
            Assert.NotNull(ps);
            Assert.False(ps.ShapeSymbol.Shape.Concrete);
            Assert.True(ps.ShapeSymbol.CachedSymbols.Count == 0);
            ////////////////////////////////////////////////////////
            const string fact2 = "x=2";
            Reasoner.Instance.Load(fact2);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ps = result[0];
            var lst = ps.RenderKnowledge;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 1);
            var gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
            var shape = gShapeExpr.ShapeSymbol.Shape as Point;
            Assert.NotNull(shape);
            Assert.False(shape.Concrete);
            Assert.True(shape.XCoordinate.Equals(2.0));

            //Trace checking         
            //Expect substitute goal from given shape (x,y) as (2,y)
            /*
                        var traceLst = gShapeExpr.KnowledgeTrace;
                        Assert.True(traceLst.Count == 1);
                        var trace = traceLst[0];
            */
            //Assert.True(trace.Source.ToString().Equals("(x,y)"));
            //Assert.True(trace.Target.ToString().Equals("(2,y)"));

            ////////////////////////////////////////////////////////////////

            Reasoner.Instance.Unload(fact2);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ps = result[0] as AGShapeExpr;
            Assert.NotNull(ps);
            ps.RetrieveRenderKnowledge();
            Assert.Null(ps.RenderKnowledge);
        }

        public void Test_Substitution_1()
        {
            //With Arithmetic in the goal
            const string fact1 = "(x,y)";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ps = result[0] as AGShapeExpr;
            Assert.NotNull(ps);
            Assert.False(ps.ShapeSymbol.Shape.Concrete);
            Assert.True(ps.ShapeSymbol.CachedSymbols.Count == 0);

            const string fact2 = "x+1=2";
            Reasoner.Instance.Load(fact2);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ps = result[0] as AGShapeExpr;

            ps.RetrieveRenderKnowledge();
            var lst = ps.RenderKnowledge;
            Assert.True(lst.Count == 1);
            var gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);

            var shape = gShapeExpr.ShapeSymbol.Shape as Point;
            Assert.NotNull(shape);
            Assert.False(shape.Concrete);
            Assert.True(shape.XCoordinate.Equals(1.0));

            //Trace checking         
            //var traceLst = gShapeExpr.KnowledgeTrace;
            //Assert.True(traceLst.Count == 3);
        }

        public void Test_Substitution_2()
        {
            const string fact1 = "(2,a)";
            var obj = Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ps = result[0];
            Assert.NotNull(ps);
            Assert.False(ps.ShapeSymbol.Shape.Concrete);
            Assert.True(ps.ShapeSymbol.CachedSymbols.Count == 0);

            const string fact3 = "a=1";
            Reasoner.Instance.Load(fact3);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var lst = result;
            Assert.NotNull(lst);
            var se = lst[0];
            Assert.NotNull(se);

            se.RetrieveRenderKnowledge();
            var gShapes = se.RenderKnowledge;
            Assert.NotNull(gShapes);
            var gShapeLst = gShapes as IList<IKnowledge> ?? gShapes.ToList();
            Assert.True(gShapeLst.Count() == 1);
            var gShapeExpr = gShapeLst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
            var gShape = gShapeExpr.ShapeSymbol as PointSymbol;
            Assert.NotNull(gShape);
            Assert.True(gShape.SymYCoordinate.Equals("1"));
            Assert.True(gShape.CachedGoals.Count == 1);
            Assert.True(gShape.Shape.Traces.Count == 1);

            var pt = gShape.Shape as Point;
            Assert.True(pt != null);
            int index = gShapes.IndexOf(gShapeExpr);
            //gShapes[index] = 

            //pt.YCoordinate = 5.0;


        }

        #endregion


        #endregion

        #region Line Shape

        #region Line Pattern Match (Unification)

        [Test]
        public void Test_Line_Concrete1()
        {
            var reasoner = Reasoner.Instance;
            //x=2
            const string fact1 = "x=2";
            reasoner.Load(fact1);
            var result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ls = result[0];
            Assert.NotNull(ls);
            var line = ls.ShapeSymbol.Shape as Line;
            Assert.NotNull(line);
            Assert.True(line.Concrete);
            Assert.True(line.A.Equals(1.0));
            Assert.Null(line.B);
            Assert.True(line.C.Equals(-2.0));
        }

        public void Test_Line_Concrete2()
        {
            var reasoner = Reasoner.Instance;

            //y=1
            const string fact1 = "y=1";
            reasoner.Load(fact1);
            var result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ls = result[0];
            Assert.NotNull(ls);
            var lineSym = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSym);
            Assert.Null(lineSym.SymA);
            Assert.True(lineSym.SymB.Equals("1"));
            Assert.True(lineSym.SymC.Equals("-1"));
        }

        public void Test_Line_Concrete3()
        {
            var reasoner = Reasoner.Instance;
            //2x+y+3=0
            const string fact1 = "2x+y+3=0";
            reasoner.Load(fact1);
            var result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ls = result[0];
            Assert.NotNull(ls);
            var lineSym = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSym);
            Assert.True(lineSym.SymA.Equals("2"));
            Assert.True(lineSym.SymB.Equals("1"));
            Assert.True(lineSym.SymC.Equals("3"));
        }

        public void Test_Line_Concrete4()
        {
            var reasoner = Reasoner.Instance;
            //2x+3=0
            const string fact1 = "2x+3=0";
            reasoner.Load(fact1);
            var result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ls = result[0];
            Assert.NotNull(ls);
            var lineSym = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSym);
            Assert.True(lineSym.SymA.Equals("2"));
            Assert.Null(lineSym.SymB);
            Assert.True(lineSym.SymC.Equals("3"));
        }

        public void Test_Line_Concrete5()
        {
            var reasoner = Reasoner.Instance;
            const string fact1 = "-2x+3.2y+1=0";
            reasoner.Load(fact1);
            var result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ls = result[0];
            Assert.NotNull(ls);
            var lineSym = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSym);
            Assert.True(lineSym.SymA.Equals("-2"));
            Assert.True(lineSym.SymB.Equals("3.2"));
            Assert.True(lineSym.SymC.Equals("1"));
        }

        public void Test_Line_Concrete6()
        {
            /*   var reasoner = Reasoner.Instance;
               const string fact1 = "-4x-y+5=0";
               reasoner.Load(fact1, null, true);
               var result = reasoner.TestGetShapeFacts();
               Assert.NotNull(result);
               Assert.True(result.Count == 1);
               var ls = result[0];
               Assert.NotNull(ls);
               var lineSym = ls.ShapeSymbol as LineSymbol;
               Assert.NotNull(lineSym);*/

        }



        #endregion

        #region Line Substitution (Reification)

        public void Test_Line_Sub1()
        {
            /*
             * ax = 2
             * a =2 
             * 
             */
            //ax=2

            var reasoner = Reasoner.Instance;
            const string fact1 = "ax=2";
            reasoner.Load(fact1);
            var result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ls = result[0];
            Assert.NotNull(ls);
            var lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            var line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            Assert.False(line.Concrete);
            Assert.True(lineSymbol.SymA.Equals("a"));
            Assert.Null(lineSymbol.SymB);
            Assert.True(lineSymbol.SymC.Equals("-2"));

            const string prop1 = "a=2";
            reasoner.Load(prop1);
            result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            ls.RetrieveRenderKnowledge();
            var lst = ls.RenderKnowledge;
            Assert.True(lst.Count == 1);
            var gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
            var shape = gShapeExpr.ShapeSymbol.Shape as Line;
            Assert.NotNull(shape);
            Assert.True(shape.Concrete);
            Assert.True(shape.A.Equals(2.0));
            Assert.Null(shape.B);
            Assert.True(shape.C.Equals(-2.0));

            reasoner.Unload(prop1);
            result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            ls.RetrieveRenderKnowledge();
            Assert.Null(ls.RenderKnowledge);
        }

        public void Test_Line_Sub2()
        {
            /*
            * ax + by = 2
            * a = 2 
            * b = 1
            */

            var reasoner = Reasoner.Instance;

            const string fact1 = "ax+by=2";
            reasoner.Load(fact1);
            var result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ls = result[0];
            Assert.NotNull(ls);
            var lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            var line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            Assert.False(line.Concrete);
            Assert.True(lineSymbol.SymA.Equals("a"));
            Assert.True(lineSymbol.SymB.Equals("b"));
            Assert.True(lineSymbol.SymC.Equals("-2"));

            const string prop1 = "a=2";
            reasoner.Load(prop1);
            result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            ls.RetrieveRenderKnowledge();
            var lst = ls.RenderKnowledge;
            Assert.True(lst.Count == 1);
            var gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
            var shape = gShapeExpr.ShapeSymbol.Shape as Line;
            Assert.NotNull(shape);
            Assert.False(shape.Concrete);
            Assert.True(shape.A.Equals(2.0));
            Assert.True(shape.B.ToString().Equals("b"));
            Assert.True(shape.C.Equals(-2.0));

            const string prop2 = "b=1";
            reasoner.Load(prop2);
            result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            ls.RetrieveRenderKnowledge();
            lst = ls.RenderKnowledge;
            Assert.True(lst.Count == 1);
            gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
            shape = gShapeExpr.ShapeSymbol.Shape as Line;
            Assert.NotNull(shape);
            Assert.True(shape.Concrete);
            Assert.True(shape.A.Equals(2.0));
            Assert.True(shape.B.Equals(1));
            Assert.True(shape.C.Equals(-2.0));

            reasoner.Unload(prop1);
            result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            ls.RetrieveRenderKnowledge();
            lst = ls.RenderKnowledge;
            Assert.True(lst.Count == 1);
            gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
            shape = gShapeExpr.ShapeSymbol.Shape as Line;
            Assert.NotNull(shape);
            Assert.False(shape.Concrete);
            Assert.True(shape.A.ToString().Equals("a"));
            Assert.True(shape.B.Equals(1));
            Assert.True(shape.C.Equals(-2.0));

            reasoner.Unload(prop2);
            result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            ls.RetrieveRenderKnowledge();
            Assert.Null(ls.RenderKnowledge);
        }

        public void Test_Line_Sub3()
        {
            var reasoner = Reasoner.Instance;

            //ax+by+m=0
            const string fact1 = "ax+by+m=0";
            reasoner.Load(fact1);
            var result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ls = result[0];
            Assert.NotNull(ls);
            var lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            var line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            Assert.False(line.Concrete);
            Assert.True(lineSymbol.SymA.Equals("a"));
            Assert.True(lineSymbol.SymB.Equals("b"));
            Assert.True(lineSymbol.SymC.Equals("m"));

            //m=2
            const string prop1 = "m=2";
            reasoner.Load(prop1);
            result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            ls.RetrieveRenderKnowledge();
            Assert.True(ls.RenderKnowledge.Count == 1);
            var gShapeExpr = ls.RenderKnowledge[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
            var shape = gShapeExpr.ShapeSymbol.Shape as Line;
            Assert.NotNull(shape);
            Assert.False(shape.Concrete);
            Assert.True(shape.A.ToString().Equals("a"));
            Assert.True(shape.B.ToString().Equals("b"));
            Assert.True(shape.C.Equals(2.0));

            //Unload m=2
            reasoner.Unload(prop1);
            result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            ls.RetrieveRenderKnowledge();
            Assert.Null(ls.RenderKnowledge);
        }

        public void Test_Line_Sub4()
        {
            /*
             * ax = 0
             * a = 2 
             * 
             */
            var reasoner = Reasoner.Instance;
            const string fact1 = "ax=0";
            reasoner.Load(fact1);
            var result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ls = result[0];
            Assert.NotNull(ls);
            var lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            var line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            Assert.False(line.Concrete);
            Assert.True(lineSymbol.SymA.Equals("a"));
            Assert.Null(lineSymbol.SymB);
            Assert.True(lineSymbol.SymC.Equals("0"));

            const string prop1 = "a=2";
            reasoner.Load(prop1);
            result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            ls.RetrieveRenderKnowledge();
            var lst = ls.RenderKnowledge;
            Assert.True(lst.Count == 1);
            var gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
            var shape = gShapeExpr.ShapeSymbol.Shape as Line;
            Assert.NotNull(shape);
            Assert.True(shape.Concrete);
            Assert.True(shape.A.Equals(2.0));
            Assert.Null(shape.B);
            Assert.True(shape.C.Equals(0.0));

            reasoner.Unload(prop1);
            result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            ls.RetrieveRenderKnowledge();
            Assert.Null(ls.RenderKnowledge);
        }

        #endregion

        #endregion

        #region Circle Shape

        /*
         *  1: (x-2)^2+(y-2)^2=1
         *  2: (x+2)^2+(y+1)^2=4
         *  3: (y-2)^2+(x+1)^2=16
         *  4: x^2+y^2=1
         *  TODO 5: (x-a)^2+(y+2)^2=1
         *  TODO 6: x^2+y^2+2x+2y+4=0
         */

        [Test]
        public void Test_Circle_Concrete1()
        {
            var reasoner = Reasoner.Instance;
            const string fact1 = "(x-2)^2+(y-2)^2=1";
            object knowledge = reasoner.Load(fact1);

            Assert.True(knowledge is AGShapeExpr);
            var result = reasoner.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var cs = result[0];
            Assert.NotNull(cs);
            var circle = cs.ShapeSymbol.Shape as Circle;
            Assert.NotNull(circle);
            Assert.True(circle.Radius.Equals(1));
        }

        #endregion

        #region Query Test

        #region Explicit Query

        [Test]
        public void QueryProp0()
        {
            /*
            * Input Fact: a = 1
            * Query: a=?
            * =>
            Answer: a = 1
            Why: given fact
            * */
            const string input = "a=1";
            Reasoner.Instance.Load(input);

            List<AGPropertyExpr> lst = Reasoner.Instance.TestGetProperties();
            Assert.True(lst.Count == 1);

            const string query = "a=";
            object result = Reasoner.Instance.Load(query);
            var agQueryExpr = result as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.Success);
            Assert.True(queryTag.CachedEntities.Count == 1);
            var cachedGoal = queryTag.CachedEntities.ToList()[0] as EqGoal;
            Assert.NotNull(cachedGoal);
            Assert.True(cachedGoal.Rhs.Equals(1));
        }

        [Test]
        public void QueryProp1()
        {
            /*
            * Input Fact: x = 1 (Line)
            * Query: x=?
            * =>
            No answer, cannot find property x!. 
            * */
            /*            
            var reasoner = new Reasoner();
            const string input = "x = 1";
            reasoner.Load(input);
            var variable = new Var('x');
            object obj;
            bool result = reasoner.Answer(variable, out obj);
            Assert.False(result);
            */
        }

        [Test]
        public void QueryProp2()
        {
            /*
            *   Input Fact: a = 1
            *   Query: y =?
            *   =>
            *   Answer : y = null
            *   Why: No knowledge input about y
            */
            const string input = "a = 1";
            Reasoner.Instance.Load(input);

            const string query = "y=";
            object result = Reasoner.Instance.Load(query);
            var agQueryExpr = result as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            Assert.False(agQueryExpr.QueryTag.Success);
        }

        [Test]
        public void QueryProp3()
        {
            /*
            * Input Fact: a+1=2
            * Query: a = ?
            * =>
            * Answer: a=1
            * Why: Simplify the given quation, draw down arrow to check details.
            */
            /*     const string input = "a+1=2";
                 Reasoner.Instance.Load(input);

                 const string query = "a = ?";
                 object result = Reasoner.Instance.Load(query);
                 var agQueryExpr = result as AGQueryExpr;
                 Assert.NotNull(agQueryExpr);
                 var eqGoal = agQueryExpr.QueryTag as EqGoal;*/
            /*      Assert.NotNull(eqGoal);
                  Assert.True(eqGoal.CachedEntities.Count == 1);
                  var cachedGoal = eqGoal.CachedEntities.ToList()[0] as EqGoal;
                  Assert.NotNull(cachedGoal);
                  Assert.True(cachedGoal.Rhs.Equals(1));*/
        }

        [Test]
        public void QueryProp4()
        {
            /*
             * Input Fact: a=1, a=2
             * Query: a = ?
             * =>
             * Answser: ambiguity
             * Why: two facts are shown
             */

            const string input1 = "a=2";
            const string input2 = "a=1";
            Reasoner.Instance.Load(input1);
            Reasoner.Instance.Load(input2);

            const string query = "a = ";
            object result = Reasoner.Instance.Load(query);
            var agQueryExpr = result as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var queryTag = agQueryExpr.QueryTag;
            Assert.NotNull(queryTag);
            Assert.True(queryTag.CachedEntities.Count == 2);
        }

        [Test]
        public void TestProp5()
        {
            /*
             * Input Fact: a=1
             * Query: a+1 = ?
             * =>
             * Answser: a=2
             * Why: two traces
             */
            const string input = "a=1";
            Reasoner.Instance.Load(input);

            const string query = "a+1=?";
            object result = Reasoner.Instance.Load(query);
            var agQueryExpr = result as AGQueryExpr;
            Assert.NotNull(agQueryExpr);
            var equation = agQueryExpr.QueryTag as Equation;
            Assert.NotNull(equation);
            //TODO cache equation
        }

        #endregion

        #region Single Point Test (TODO)



        [Test]
        public void test2_point_test()
        {
            /*
             * Input Fact: (3.0,y)
             * Query: Y=?
             * =>
             * Answer: Unknown
             */
            //TODO
        }

        [Test]
        public void test3_point_test()
        {
            //TODO
            /*
             * Input Fact: (3.0,y), x = 2
             * Query: x=?
             * =>
             * Answer: ambiguity, choose the point or the line?
             */
        }

        [Test]
        public void test4_point_test()
        {
            /*
             * Input Fact: A(x,2), x = -3.1
             * Query: A=?
             * =>
             * Answer: A=(-3.1,2)
             * Why: trace
             */
        }

        [Test]
        public void test5_point_test()
        {
            /*
             * Input Fact: B(x,y), x+1=3-1
             * Query1: x =?
             * =>
             * Answer: x = 1
             * Why: 2 trace 
             * =>
             * Query2: B=?
             * =>
             * Answer B=(1,y)
             * Why: 1 trace
             */
        }

        #endregion

        [Test]
        public void Test0()
        {
            //x = 1
            const string fact1 = "x = 1";
            var obj = Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetProperties();
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
        }

        [Test]
        public void Test_Equation_1_point()
        {
            const string input1 = "a=1";
            const string query = "a=";

            Reasoner.Instance.Load(input1);
            object obj = Reasoner.Instance.Load(query);
        }

        [Test]
        public void Test_Equation_2_point()
        {
            const string input1 = "1+1=";
            var obj = Reasoner.Instance.Load(input1);
            var queryExpr = obj as AGQueryExpr;
            Assert.NotNull(queryExpr);

            queryExpr.RetrieveRenderKnowledge();
            Assert.NotNull(queryExpr.RenderKnowledge);
            Assert.True(queryExpr.RenderKnowledge.Count == 1);

            var eqExpr = queryExpr.RenderKnowledge[0] as AGEquationExpr;
            Assert.NotNull(eqExpr);
            Assert.True(eqExpr.Equation.Rhs.Equals(2));
            /*
                        queryExpr.GenerateSolvingTrace();
                        Assert.Null(queryExpr.AutoTrace);
            */
            eqExpr.GenerateSolvingTrace();
            Assert.Null(queryExpr.AutoTrace);

            eqExpr.IsSelected = true;
            eqExpr.GenerateSolvingTrace();
            Assert.NotNull(eqExpr.AutoTrace);
            Assert.True(eqExpr.AutoTrace.Count == 1);
        }

        [Test]
        public void Test_Equation_2_Tutor_point()
        {
            const string input1 = "1+1=";
            var obj = Reasoner.Instance.Load(input1);
            var queryExpr = obj as AGQueryExpr;
            Assert.NotNull(queryExpr);

            const string input2 = "1+1=2";
            var obj1 = Reasoner.Instance.Load(input2);

            var eqExpr = obj1 as AGEquationExpr;
            //TODO
        }

        [Test]
        public void Test_Equation_3_point()
        {
            const string input1 = "a=1";
            const string query = "a+1=";

            Reasoner.Instance.Load(input1);
            object obj = Reasoner.Instance.Load(query);
        }

        #endregion

        #region Real Time Testing

        [Test]
        public void Test_RealTime_Uncertainty_0()
        {
            const string fact1 = "ax+y-1=0";
            var shapeExpr = Reasoner.Instance.Load(fact1) as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var ls = shapeExpr.ShapeSymbol as LineSymbol;
            Assert.NotNull(ls);
            Assert.False(ls.Shape.Concrete);

            const string fact2 = "a=2";
            var goalExpr = Reasoner.Instance.Load(fact2) as AGPropertyExpr;
            Assert.NotNull(goalExpr);
            Assert.True(shapeExpr.ShapeSymbol.CachedSymbols.Count == 1);
        }

        [Test]
        public void Test_RealTime_Uncertainty_1()
        {
            const string fact2 = "a=2";
            var goalExpr = Reasoner.Instance.Load(fact2) as AGPropertyExpr;
            Assert.NotNull(goalExpr);

            const string fact1 = "ax+y-1=0";
            var shapeExpr = Reasoner.Instance.Load(fact1) as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var ls = shapeExpr.ShapeSymbol as LineSymbol;
            Assert.NotNull(ls);
            Assert.False(ls.Shape.Concrete);
            Assert.True(shapeExpr.ShapeSymbol.CachedSymbols.Count == 1);
        }


        #endregion
    }
}