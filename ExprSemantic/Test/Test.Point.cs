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

namespace ExprSemanticTest
{
    [TestFixture]
    public class TestPoint
    {
        #region Point Pattern Match

        [Test]
        public void Test_Point_PatternMatch0()
        {
            const string fact1 = "(1,1)";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ps = result[0] as AGShapeExpr;
            Assert.NotNull(ps);
            var pt = ps.ShapeSymbol.Shape as Point;
            Assert.NotNull(pt);
            Assert.True(pt.Concrete);
            Assert.True(ps.ShapeSymbol.Shape.CachedSymbols.Count == 0);
        }

        [Test]
        public void Test_Point_PatternMatch1()
        {
            const string fact1 = "A(x,2)";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ps = result[0] as AGShapeExpr;
            Assert.NotNull(ps);
            Assert.False(ps.ShapeSymbol.Shape.Concrete);
            Assert.True(ps.ShapeSymbol.Shape.CachedSymbols.Count == 0);
        }

        #endregion

        #region Point Reification(Substitution)

        [Test]
        public void Test_Substitution_0()
        {
            const string fact1 = "(x,y)";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ps = result[0] as AGShapeExpr;
            Assert.NotNull(ps);
            Assert.False(ps.ShapeSymbol.Shape.Concrete);
            Assert.True(ps.ShapeSymbol.Shape.CachedSymbols.Count == 0);

            const string fact2 = "x=2";
            Reasoner.Instance.Load(fact2);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ps = result[0] as AGShapeExpr;

            var lst = ps.RetrieveGeneratedShapes().ToList();
            Assert.True(lst.Count == 1);
            var gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
           
            var shape = gShapeExpr.ShapeSymbol.Shape as Point;
            Assert.NotNull(shape);
            Assert.False(shape.Concrete);
            Assert.True(shape.XCoordinate.Equals(2.0));

            //Trace checking         
            //Expect substitute goal from given shape (x,y) as (2,y)
            var traceLst = gShapeExpr.KnowledgeTrace;
            Assert.True(traceLst.Count == 1);
            var trace = traceLst[0];
            //Assert.True(trace.Source.ToString().Equals("(x,y)"));
            //Assert.True(trace.Target.ToString().Equals("(2,y)"));

            Reasoner.Instance.Unload(fact2);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ps = result[0] as AGShapeExpr;
            Assert.NotNull(ps);
            IEnumerable<IKnowledge> ks = ps.RetrieveGeneratedShapes();
            Assert.Null(ks);
        }

        [Test]
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
            Assert.True(ps.ShapeSymbol.Shape.CachedSymbols.Count == 0);

            const string fact2 = "x+1=2";
            Reasoner.Instance.Load(fact2);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            ps = result[0] as AGShapeExpr;

            var lst = ps.RetrieveGeneratedShapes().ToList();
            Assert.True(lst.Count == 1);
            var gShapeExpr = lst[0] as AGShapeExpr;
            Assert.NotNull(gShapeExpr);
           
            var shape = gShapeExpr.ShapeSymbol.Shape as Point;
            Assert.NotNull(shape);
            Assert.False(shape.Concrete);
            Assert.True(shape.XCoordinate.Equals(1.0));

            //Trace checking         
            var traceLst = gShapeExpr.KnowledgeTrace;
            Assert.True(traceLst.Count == 3);
        }

        [Test]
        public void Test_Substitution_2()
        {
            const string fact1 = "(2,y)";
            Reasoner.Instance.Load(fact1);
            var result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 1);
            var ps = result[0] as AGShapeExpr;
            Assert.NotNull(ps);
            Assert.False(ps.ShapeSymbol.Shape.Concrete);
            Assert.True(ps.ShapeSymbol.Shape.CachedSymbols.Count == 0);

            const string fact2 = "(3,y)";
            Reasoner.Instance.Load(fact2);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 2);

            const string fact3 = "y=1";
            Reasoner.Instance.Load(fact3);
            result = Reasoner.Instance.TestGetShapeFacts();
            Assert.NotNull(result);
            Assert.True(result.Count == 2);
            var lst = result as List<AGShapeExpr>;
            Assert.NotNull(lst);

            foreach (AGShapeExpr se in lst)
            {
                IEnumerable<IKnowledge> gShapes = se.RetrieveGeneratedShapes();
                Assert.NotNull(gShapes);
                var gShapeLst = gShapes as IList<IKnowledge> ?? gShapes.ToList();
                Assert.True(gShapeLst.Count() == 1);
                var gShape = gShapeLst[0];
                Assert.True(gShape.KnowledgeTrace.Count == 1);
            }
        }

        #endregion

        #region Constraint Solving

        #endregion 
    }
}
