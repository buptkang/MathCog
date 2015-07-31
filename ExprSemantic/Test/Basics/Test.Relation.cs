using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgebraGeometry;
using AlgebraGeometry.Expr;
using CSharpLogic;
using ExprSemantic;
using NUnit.Framework;
using starPadSDK.MathExpr;

namespace ExprSemanticTest
{
    [TestFixture]
    public class TestRelation
    {
        [Test]
        public void Test1()
        {
            const string fact1 = "A(1,1)";
            Reasoner.Instance.Load(fact1);

            const string fact2 = "B(2,1)";
            Reasoner.Instance.Load(fact2);

            //Uniform Search Input(Sketh or Touch) 
            List<AGShapeExpr> selectFacts = Reasoner.Instance.TestGetShapeFacts();
            Assert.True(selectFacts.Count == 2);
            const string query = "AB";
            object result = Reasoner.Instance.Load(query);
            var types = result as List<ShapeType>;
            Assert.NotNull(types);
            
            var expr = new CompositeExpr(WellKnownSym.times, new Expr[]{new LetterSym('A'), new LetterSym('B')});
            result = Reasoner.Instance.Load(expr);
            types = result as List<ShapeType>;
            Assert.NotNull(types);

            Reasoner.Instance.Unload(query);
            object obj = Reasoner.Instance.Load(query, ShapeType.Line);
            Assert.NotNull(obj);
            var shapeExpr = obj as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var line = shapeExpr.ShapeSymbol.Shape as Line;
            Assert.NotNull(line);

            selectFacts = Reasoner.Instance.TestGetShapeFacts();
            Assert.True(selectFacts.Count == 3);

            Reasoner.Instance.Unload(query);
            obj = Reasoner.Instance.Load(query, ShapeType.LineSegment);
            Assert.NotNull(obj);
            shapeExpr = obj as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var lineSeg = shapeExpr.ShapeSymbol.Shape as LineSegment;
            Assert.NotNull(lineSeg);
        }
    }
}
