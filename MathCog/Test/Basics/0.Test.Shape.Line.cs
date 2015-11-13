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

namespace MathCog
{
    using AlgebraGeometry;
    using NUnit.Framework;

    [TestFixture]
    public class TestLine
    {
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
    }
}