using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgebraGeometry;
using AlgebraGeometry.Expr;
using NUnit.Framework;
using starPadSDK.MathExpr;

namespace ExprSemantic
{
    [TestFixture]
    public class TestRelation
    {
        [Test]
        public void Test1()
        {
            var reasoner = new Reasoner();
            const string fact1 = "A(1,1)";
            reasoner.Load(fact1);

            const string fact2 = "B(2,1)";
            reasoner.Load(fact2);

            //Uniform Search Input(Sketh or Touch) 
            List<AGShapeExpr> selectFacts = reasoner.TestGetShapeFacts();
            Assert.True(selectFacts.Count == 2);
            const string query = "AB";
            object result = reasoner.Load(query);
            var types = result as List<ShapeType>;
            Assert.NotNull(types);
            
            var expr = new CompositeExpr(WellKnownSym.times, new Expr[]{new LetterSym('A'), new LetterSym('B')});
            result = reasoner.Load(expr);
            types = result as List<ShapeType>;
            Assert.NotNull(types);

            reasoner.Unload(query);
            object obj = reasoner.Load(query, ShapeType.Line);
            Assert.NotNull(obj);
            var shapeExpr = obj as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var line = shapeExpr.ShapeSymbol.Shape as Line;
            Assert.NotNull(line);

            selectFacts = reasoner.TestGetShapeFacts();
            Assert.True(selectFacts.Count == 3);
            
            reasoner.Unload(query);
            obj = reasoner.Load(query, ShapeType.LineSegment);
            Assert.NotNull(obj);
            shapeExpr = obj as AGShapeExpr;
            Assert.NotNull(shapeExpr);
            var lineSeg = shapeExpr.ShapeSymbol.Shape as LineSegment;
            Assert.NotNull(lineSeg);
        }
    }
}
