using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CSharpLogic;

namespace AlgebraGeometry
{
    [TestFixture]
    public partial class TestPoint
    {
        [Test]
        public void TestGraph_1()
        {
            var graph = new RelationGraph();
            //true positive
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, y);
            var ps = new PointSymbol(point);
            graph.AddNode(ps);
            Assert.True(graph.Nodes.Count == 1);
            var eqGoal = new EqGoal(x, 1); // x=1
            graph.AddNode(eqGoal);
        
            List<ShapeSymbol> shapes = graph.RetrieveShapeSymbols();
            Assert.True(shapes.Count == 1);
            var pt = shapes[0] as PointSymbol;
            Assert.NotNull(pt);
            Assert.True(pt.Equals(ps));
            Assert.True(pt.CachedGoals.Count == 1);
            Assert.True(pt.CachedSymbols.Count == 1);
            var gPointSymbol = pt.CachedSymbols.ToList()[0] as PointSymbol;
            Assert.NotNull(gPointSymbol);
            var gPoint = gPointSymbol.Shape as Point;
            Assert.NotNull(gPoint);
            Assert.False(gPoint.Concrete);
            Assert.True(1.0.Equals(gPoint.XCoordinate));
            Assert.True(y.Equals(gPoint.YCoordinate));
        }    
    }
}

/*            /******
             * current status:
             * (1,y)
             * 
             * next input:
             * x = 2
            ****#1#

            var eqGoal1 = new EqGoal(x, 2); // x=2
            graph.AddNode(eqGoal1);
            shapes = graph.RetrieveShapeSymbols();
            Assert.True(shapes.Count == 1);
            pt = shapes[0] as PointSymbol;
            Assert.NotNull(pt);
            Assert.True(pt.Equals(ps));
            Assert.True(pt.CachedGoals.Count == 2);
            Assert.True(pt.CachedSymbols.Count == 2);
            Assert.False(point.Concrete);

            /******
            * current status:
            * (1,y)
            * (2,y)
            * 
            * next input:
            * y = 1
            ****#1#

            var eqGoal2 = new EqGoal(y, 1); // y = 1
            graph.AddNode(eqGoal2);
            shapes = graph.RetrieveShapeSymbols();
            Assert.True(shapes.Count == 1);
            pt = shapes[0] as PointSymbol;
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 3);
            Assert.True(point.CachedSymbols.Count == 2);
            foreach (var shape in point.CachedSymbols)
            {
                Assert.True(shape.Concrete);
            }

            var goals = graph.RetrieveGoals();
            Assert.True(goals.Count == 3);

            /******
           * current status:
           * (1,1)
           * (2,1)
           * 
           * next input:
           * y = 2
           ****#1#

            var eqGoal3 = new EqGoal(y, 2); // y = 2
            graph.AddNode(eqGoal3);

            shapes = graph.RetrieveShapes();
            Assert.True(shapes.Count == 1);
            pt = shapes[0] as Point;

            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 4);
            Assert.True(point.CachedSymbols.Count == 4);
            foreach (var shape in point.CachedSymbols)
            {
                Assert.True(shape.Concrete);
            }

            /////////////////////////////////////////////

            graph.DeleteNode(eqGoal3);
            shapes = graph.RetrieveShapes();
            Assert.True(shapes.Count == 1);
            pt = shapes[0] as Point;
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 3);
            Assert.True(point.CachedSymbols.Count == 2);
            foreach (var shape in point.CachedSymbols)
            {
                Assert.True(shape.Concrete);
            }

            goals = graph.RetrieveGoals();
            Assert.True(goals.Count == 3);

            /////////////////////////////////////////////

            graph.DeleteNode(eqGoal2);
            shapes = graph.RetrieveShapes();
            Assert.True(shapes.Count == 1);
            pt = shapes[0] as Point;
            Assert.False(point.Concrete);
            Assert.True(point.CachedGoals.Count == 2);
            Assert.True(point.CachedSymbols.Count == 2);
            foreach (var shape in point.CachedSymbols)
            {
                Assert.False(shape.Concrete);
            }
            goals = graph.RetrieveGoals();
            Assert.True(goals.Count == 2);

            /////////////////////////////////////////////

            graph.DeleteNode(point);
            shapes = graph.RetrieveShapes();
            Assert.True(shapes.Count == 0);*/
