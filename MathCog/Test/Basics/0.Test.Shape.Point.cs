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
    using AlgebraGeometry;
    using CSharpLogic;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    [TestFixture]
    public class TestPoint
    {
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
    }
}
