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

namespace AlgebraGeometry
{
    using CSharpLogic;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

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

            /******
             * current status:
             * (1,y)
             * 
             * next input:
             * x = 2
            ****/

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
            ****/

            var eqGoal2 = new EqGoal(y, 1); // y = 1
            graph.AddNode(eqGoal2);
            shapes = graph.RetrieveShapeSymbols();
            Assert.True(shapes.Count == 1);
            pt = shapes[0] as PointSymbol;
            Assert.False(point.Concrete);
            Assert.True(ps.CachedGoals.Count == 3);
            Assert.True(ps.CachedSymbols.Count == 2);
            foreach (var ss in ps.CachedSymbols)
            {
                Assert.True(ss.Shape.Concrete);
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
             ****/

            var eqGoal3 = new EqGoal(y, 2); // y = 2
            graph.AddNode(eqGoal3);

            shapes = graph.RetrieveShapeSymbols();
            Assert.True(shapes.Count == 1);
            ps = shapes[0] as PointSymbol;
            Assert.NotNull(ps);
            Assert.False(ps.Shape.Concrete);
            Assert.True(ps.CachedGoals.Count == 4);
            Assert.True(ps.CachedSymbols.Count == 4);
            foreach (var css in ps.CachedSymbols)
            {
                Assert.True(css.Shape.Concrete);
            }

            /////////////////////////////////////////////

            graph.DeleteNode(eqGoal3);
            shapes = graph.RetrieveShapeSymbols();
            Assert.True(shapes.Count == 1);
            pt = shapes[0] as PointSymbol;
            Assert.NotNull(pt);
            Assert.False(pt.Shape.Concrete);
            Assert.True(pt.CachedGoals.Count == 3);
            Assert.True(pt.CachedSymbols.Count == 2);
            foreach (var ss in pt.CachedSymbols)
            {
                Assert.True(ss.Shape.Concrete);
            }

            goals = graph.RetrieveGoals();
            Assert.True(goals.Count == 3);


            /////////////////////////////////////////////

            graph.DeleteNode(eqGoal2);
            shapes = graph.RetrieveShapeSymbols();
            Assert.True(shapes.Count == 1);
            pt = shapes[0] as PointSymbol;
            Assert.NotNull(pt);
            Assert.False(pt.Shape.Concrete);
            Assert.True(pt.CachedGoals.Count == 2);
            Assert.True(pt.CachedSymbols.Count == 2);
            foreach (var shape in pt.CachedSymbols)
            {
                Assert.False(shape.Shape.Concrete);
            }
            goals = graph.RetrieveGoals();
            Assert.True(goals.Count == 2);

            /////////////////////////////////////////////

            graph.DeleteNode(point);
            shapes = graph.RetrieveShapeSymbols();
            Assert.True(shapes.Count == 1);
        }


        [Test]
        public void TestGraph_2()
        {
            var graph = new RelationGraph();
            //true positive
            var x = new Var('x');
            var y = new Var('y');
            var point = new Point(x, 2);
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
            Assert.True(gPoint.Concrete);
            Assert.True(1.0.Equals(gPoint.XCoordinate));
            Assert.True(2.0.Equals(gPoint.YCoordinate));
        }
    }
}


