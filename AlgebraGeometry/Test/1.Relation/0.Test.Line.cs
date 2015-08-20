using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using NUnit.Framework;

namespace AlgebraGeometry
{
    [TestFixture]
    public partial class TestLine
    {
		[Test]
        public void TestGraph_1()
        {
            var graph = new RelationGraph();
            var a = new Var('a');
            var point = new Point(2, a);
		    var ps = new PointSymbol(point);
            graph.AddNode(ps);
		    var line = new Line(1, a, 1.0);
		    var ls = new LineSymbol(line);
		    graph.AddNode(ls);

		    List<ShapeSymbol> points = graph.RetrieveShapeSymbols(ShapeType.Point);
			Assert.True(points.Count == 1);
		    var pt = points[0] as PointSymbol;
			Assert.NotNull(pt);
			Assert.True(pt.CachedSymbols.Count == 0);

            var eqGoal = new EqGoal(a, 1); // a=1
            graph.AddNode(eqGoal);
            points = graph.RetrieveShapeSymbols(ShapeType.Point);
            Assert.True(points.Count == 1);
            pt = points[0] as PointSymbol;
            Assert.NotNull(pt);
            Assert.True(pt.CachedSymbols.Count == 1);
            var cachedPs = pt.CachedSymbols.ToList()[0] as PointSymbol;
            Assert.NotNull(cachedPs);
		    var cachedPt = cachedPs.Shape as Point;
            Assert.NotNull(cachedPt);
            Assert.True(cachedPt.XCoordinate.Equals(2.0));
			Assert.True(cachedPt.YCoordinate.Equals(1.0));

            var lines = graph.RetrieveShapeSymbols(ShapeType.Line);
			Assert.True(lines.Count == 1);
            var currLine = lines[0] as LineSymbol;
            Assert.NotNull(currLine);
			Assert.True(currLine.CachedSymbols.Count == 1);
		    var cachedLineSymbol = currLine.CachedSymbols.ToList()[0] as LineSymbol;
			Assert.NotNull(cachedLineSymbol);
		    var cachedLine = cachedLineSymbol.Shape as Line;
            Assert.NotNull(cachedLine);
			Assert.True(cachedLine.A.Equals(1.0));
			Assert.True(cachedLine.B.Equals(1.0));
			Assert.True(cachedLine.C.Equals(1.0));
        }
    }
}
