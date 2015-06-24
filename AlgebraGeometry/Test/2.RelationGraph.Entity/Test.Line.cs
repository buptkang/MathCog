using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;
using NUnit.Framework;

namespace AlgebraGeometry.TestGraph
{
    [TestFixture]
    public class TestLine
    {
		[Test]
        public void Test1()
        {
            var graph = new RelationGraph();
            var a = new Var('a');
            var point = new Point(2, a);
            graph.AddNode(point);
		    var line = new Line(1, a, 1.0);
		    graph.AddNode(line);

		    List<Shape> points = graph.RetrieveSpecicShapes(ShapeType.Point);
			Assert.True(points.Count == 1);
		    var pt = points[0] as Point;
			Assert.NotNull(pt);
			Assert.True(pt.CachedSymbols.Count == 0);

            var eqGoal = new EqGoal(a, 1); // a=1
            graph.AddNode(eqGoal);

            points = graph.RetrieveSpecicShapes(ShapeType.Point);
            Assert.True(points.Count == 1);
            pt = points[0] as Point;
            Assert.NotNull(pt);
            Assert.True(pt.CachedSymbols.Count == 1);
            var cachedPt = pt.CachedSymbols.ToList()[0] as Point;
            Assert.NotNull(cachedPt);
            Assert.True(cachedPt.XCoordinate.Equals(2.0));
			Assert.True(cachedPt.YCoordinate.Equals(1.0));

		    var lines = graph.RetrieveSpecicShapes(ShapeType.Line);
			Assert.True(lines.Count == 1);
            var currLine = lines[0] as Line;
            Assert.NotNull(currLine);
			Assert.True(currLine.CachedSymbols.Count == 1);
		    var cachedLine = currLine.CachedSymbols.ToList()[0] as Line;
			Assert.NotNull(cachedLine);
			Assert.True(cachedLine.A.Equals(1.0));
			Assert.True(cachedLine.B.Equals(1.0));
			Assert.True(cachedLine.C.Equals(1.0));
        }
    }
}
