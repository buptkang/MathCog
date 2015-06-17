using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class RelationGraph
    {
        #region Constructor and Properties

        private List<GraphNode> _nodes;
        public List<GraphNode> Nodes
        {
            get { return _nodes;}
            set { _nodes = value; }
        }
/*
        private List<GraphEdge> _edges;
        public List<GraphEdge> Edges
        {
            get { return _edges; }
            set { _edges = value; }
        }
*/        
        public RelationGraph()
        {
            _nodes = new List<GraphNode>();
//            _edges = new List<GraphEdge>();
        }

        #endregion

        #region Input API

        /// <summary>
        /// Add node onto the graph
        /// </summary>
        /// <param name="shape"></param>
        public ShapeNode AddShapeNode(Shape shape)
        {
            var sn = new ShapeNode(shape);
            BuildExplicitConnection(sn);
            _nodes.Add(sn);
            return sn;
        }

        /// <summary>
        /// Add node onto the graph
        /// </summary>
        /// <param name="goal"></param>
        public void AddGoalNode(Goal goal)
        {
            var gn = new GoalNode(goal);
            BuildExplicitConnection(gn);
            _nodes.Add(gn);
        }

        /// <summary>
        /// Delete node from the graph
        /// </summary>
        /// <param name="shape"></param>
        public void DeleteShapeNode(Shape shape)
        {
            var shapeNode = SearchNode(shape) as ShapeNode;
            if (shapeNode != null)
            {
                for (int i = 0 ; i <shapeNode.OutEdges.Count ; i++)
                {
                    GraphEdge outEdge = shapeNode.OutEdges[i];
                    var targetNode = outEdge.Target as ShapeNode;
                    if (targetNode != null)
                    {
                        DeleteShapeNode(targetNode.Shape);
                        shapeNode.OutEdges.Remove(outEdge); 
                    }
                }
                _nodes.Remove(shapeNode);
            }
        }

        /// <summary>
        /// Delete node from the graph
        /// </summary>
        /// <param name="goal"></param>
        public void DeleteGoalNode(Goal goal)
        {
            var goalNode = SearchNode(goal) as GoalNode;
            if (goalNode != null)
            {
                UnBuildExplicitConnection(goalNode);
                _nodes.Remove(goalNode);
            }
        }

        /// <summary>
        /// Add edge onto the graph
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <param name="shapeType"></param>
        public Shape AddRelation(Shape s1, Shape s2, ShapeType shapeType)
        {
            Shape outputShape = RelationLogic.BuildRelation(s1, s2, shapeType);

            if (outputShape != null)
            {
                ShapeNode gShapeNode = AddShapeNode(outputShape);
                var gn1 = SearchNode(s1) as ShapeNode;
                var gn2 = SearchNode(s2) as ShapeNode;

                if (gn1 != null && gn2 != null)
                {
                    var edge1 = new GraphEdge(gn1, gShapeNode);
                    gn1.OutEdges.Add(edge1);
                    gShapeNode.InEdges.Add(edge1);

                    var edge2 = new GraphEdge(gn2, gShapeNode);
                    gn2.OutEdges.Add(edge2);
                    gShapeNode.InEdges.Add(edge2);
                }

                return outputShape;
            }
            return null;
        }

        /// <summary>
        /// such as building a line by using two properties, 
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <param name="shapeType"></param>
        public void AddRelation(Goal g1, Goal g2, ShapeType shapeType)
        {
            throw new Exception("TODO");
        }

        #endregion

        #region Output Test API

        public List<GoalNode> RetrieveGoalNodes()
        {
            var goalNodes = new List<GoalNode>();
            foreach (GraphNode gn in _nodes)
            {
                var goalNode = gn as GoalNode;
                if (goalNode != null)
                {
                    goalNodes.Add(goalNode);
                }
            }
            return goalNodes;
        }

        public List<ShapeNode> RetrieveShapeNodes()
        {
            var shapeNodes = new List<ShapeNode>();
            foreach (GraphNode gn in _nodes)
            {
                var sn = gn as ShapeNode;
                if (sn != null)
                {
                    shapeNodes.Add(sn);
                }
            }
            return shapeNodes;
        }
        
        public List<ShapeNode> RetrieveShapeNodes(ShapeType type)
        {
            var shapeNodes = new List<ShapeNode>();
            foreach (GraphNode gn in _nodes)
            {
                var sn = gn as ShapeNode;
                if (sn != null && sn.Shape.ShapeType.Equals(type))
                {
                    shapeNodes.Add(sn);
                }
            }
            return shapeNodes;
        }

        public List<Shape> RetrieveShapes()
        {
            var lst = new List<Shape>();
            foreach (var gn in _nodes)
            {
                var sn = gn as ShapeNode;
                if (sn != null)
                {
                    lst.Add(sn.Shape);
                }
            }
            return lst;
        }

        public List<Shape> RetrieveSpecicShapes(ShapeType type)
        {
            var lst = new List<Shape>();
            foreach (var gn in _nodes)
            {
                var sn = gn as ShapeNode;
                if (sn != null && sn.Shape.ShapeType.Equals(type))
                {
                    lst.Add(sn.Shape);
                }
            }
            return lst;
        }

        public List<Goal> RetrieveGoals()
        {
            var lst = new List<Goal>();
            foreach (var gn in _nodes)
            {
                var goalNode = gn as GoalNode;
                if (goalNode != null)
                {
                    lst.Add(goalNode.Goal);
                }
            }
            return lst;
        }

        #endregion
    }
}
