﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgebraGeometry;
using NUnit.Framework;

namespace GeometryLogicInference
{
    [TestFixture]
    public class TestSelection
    {
        [Test]
        public void Test1()
        {
            //TODO
            var graph = new RelationGraph();
            var pt1 = new Point("A", 1.0, 2.0);
            var pt2 = new Point("B", 2.0, 4.0);
            var pt3 = new Point("C", -2.0, 4.0);
            var line1 = new Line(2.0, 4.0, 1.0);
            graph.AddShapeNode(pt1);
            graph.AddShapeNode(pt2);
            graph.AddShapeNode(pt3);
            graph.AddShapeNode(line1);
        }
    }
}
