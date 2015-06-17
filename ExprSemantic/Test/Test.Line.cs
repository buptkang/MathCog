using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExprSemantic;
using NUnit.Framework;
using AlgebraGeometry;
using AlgebraGeometry.Expr;

namespace ExprSemanticTest
{
    [TestFixture]
    public class TestLine
    {
        #region Line Pattern Match (Unification)

        [Test]
        public void Test_Line_Concrete1()
        {
            //x=2
            const string fact1 = "x=2";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
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

        [Test]
        public void Test_Line_Concrete2()
        {
            //y=1
            const string fact1 = "y=1";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
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

        [Test]
        public void Test_Line_Concrete3()
        {
            //2x+y+3=0
            const string fact1 = "2x+y+3=0";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
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

        [Test]
        public void Test_Line_Concrete4()
        {
            //2x+3=0
            const string fact1 = "2x+3=0";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
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

        #endregion

        #region Line Substitution (Reification)

        [Test]
        public void Test_Line_Sub1()
        {
            /*
             * ax = 2
             * a =2 
             * 
             */ 
            //ax=2
            const string fact1 = "ax=2";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
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
            Reasoner.Instance.Load(prop1);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            var lst = ls.RetrieveGeneratedShapes().ToList();
            Assert.True(lst.Count == 1);
            var gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
            var shape = gShapeExpr.ShapeSymbol.Shape as Line;
            Assert.NotNull(shape);
            Assert.True(shape.Concrete);
            Assert.True(shape.A.Equals(2.0));
            Assert.Null(shape.B);
            Assert.True(shape.C.Equals(-2.0));

            Reasoner.Instance.Unload(prop1);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            IEnumerable<IKnowledge> iEnum = ls.RetrieveGeneratedShapes();
            Assert.Null(iEnum);
        }

        [Test]
        public void Test_Line_Sub2()
        {
            /*
            * ax + by = 2
            * a = 2 
            * b = 1
            */
            const string fact1 = "ax+by=2";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
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
            Reasoner.Instance.Load(prop1);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            var lst = ls.RetrieveGeneratedShapes().ToList();
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
            Reasoner.Instance.Load(prop2);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            lineSymbol = ls.ShapeSymbol as LineSymbol;
            Assert.NotNull(lineSymbol);
            line = lineSymbol.Shape as Line;
            Assert.NotNull(line);
            lst = ls.RetrieveGeneratedShapes().ToList();
            Assert.True(lst.Count == 1);
            gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
            shape = gShapeExpr.ShapeSymbol.Shape as Line;
            Assert.NotNull(shape);
            Assert.True(shape.Concrete);
            Assert.True(shape.A.Equals(2.0));
            Assert.True(shape.B.Equals(1));
            Assert.True(shape.C.Equals(-2.0));

            Reasoner.Instance.Unload(prop1);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            lst = ls.RetrieveGeneratedShapes().ToList();
            Assert.True(lst.Count == 1);
            gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
            shape = gShapeExpr.ShapeSymbol.Shape as Line;
            Assert.NotNull(shape);
            Assert.False(shape.Concrete);
            Assert.True(shape.A.ToString().Equals("a"));
            Assert.True(shape.B.Equals(1));
            Assert.True(shape.C.Equals(-2.0));

            Reasoner.Instance.Unload(prop2);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            IEnumerable<IKnowledge> iEnum = ls.RetrieveGeneratedShapes();
            Assert.Null(iEnum);
        }

        [Test]
        public void Test_Line_Sub3()
        {
            //ax+by+m=0
            const string fact1 = "ax+by+m=0";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
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
            Reasoner.Instance.Load(prop1);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            var lst = ls.RetrieveGeneratedShapes().ToList();
            Assert.True(lst.Count == 1);
            var gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
            var shape = gShapeExpr.ShapeSymbol.Shape as Line;
            Assert.NotNull(shape);
            Assert.False(shape.Concrete);
            Assert.True(shape.A.ToString().Equals("a"));
            Assert.True(shape.B.ToString().Equals("b"));
            Assert.True(shape.C.Equals(2.0));

            //Unload m=2
            Reasoner.Instance.Unload(prop1);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ls = result[0];
            Assert.NotNull(ls);
            IEnumerable<IKnowledge> iEnum = ls.RetrieveGeneratedShapes();
            Assert.Null(iEnum);
        }

        #endregion

    }
}
