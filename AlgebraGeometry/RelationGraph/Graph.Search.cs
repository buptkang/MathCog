using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class RelationGraph
    {
        private void BuildExplicitConnection(ShapeNode shapeNode)
        {
            if (shapeNode.Shape.Concrete) return;

            #region Point Reification
            var pt = shapeNode.Shape as Point;
            if (pt != null)
            {
                foreach (GraphNode gn in _nodes)
                {
                    var goalNode = gn as GoalNode;
                    if (goalNode != null)
                    {
                        bool result = pt.Reify((EqGoal)goalNode.Goal);
                        if (result)
                        {
                            //create edge between the shape with other goals
                            var edge = new GraphEdge(gn, shapeNode);
                            //_edges.Add(edge);
                            gn.OutEdges.Add(edge);
                            shapeNode.InEdges.Add(edge);                           
                        }                        
                    }
                }

                #region Interaction
                if (KnowledgeUpdated != null)
                    KnowledgeUpdated(this, shapeNode.Shape);
                #endregion
            }
            #endregion

            #region Line Reification

            var line = shapeNode.Shape as Line;
            if (line != null)
            {
                foreach (GraphNode gn in _nodes)
                {
                    var goalNode = gn as GoalNode;
                    if (goalNode != null)
                    {
                        bool result = line.Reify((EqGoal)goalNode.Goal);
                        if (result)
                        {
                            //create edge between the shape with other goals
                            var edge = new GraphEdge(gn, shapeNode);
                            //_edges.Add(edge);
                            gn.OutEdges.Add(edge);
                            shapeNode.InEdges.Add(edge); 
                        }
                    }
                }

                #region Interaction
                if (KnowledgeUpdated != null)
                    KnowledgeUpdated(this, shapeNode.Shape);
                #endregion
            }


            #endregion
        }

        private void BuildExplicitConnection(GoalNode goalNode)
        {
            var eqGoal = goalNode.Goal as EqGoal;
            Debug.Assert(eqGoal != null);
            foreach (GraphNode gn in _nodes)
            {
                var shapeNode = gn as ShapeNode;
                if (shapeNode != null)
                {
                    if (shapeNode.Shape.Concrete) continue;

                    #region Point Reification
                    var pt = shapeNode.Shape as Point;
                    if (pt != null)
                    {
                        bool result = pt.Reify(eqGoal);
                        if (result)
                        {
                            //create edge between the shape with other goals
                            var edge = new GraphEdge(goalNode, shapeNode);
                            //_edges.Add(edge);
                            goalNode.OutEdges.Add(edge);
                            shapeNode.InEdges.Add(edge);

                            //update shapeNode's upper relation
                            UpdateRelation(shapeNode);
                        }

                        #region Interaction

                        if (KnowledgeUpdated != null)
                            KnowledgeUpdated(this, shapeNode.Shape);

                        #endregion
                    }

                    #endregion

                    #region Line Reification
                    var line = shapeNode.Shape as Line;
                    if (line != null)
                    {
                        bool result = line.Reify(eqGoal);
                        if (result)
                        {
                            //create edge between the shape with other goals
                            var edge = new GraphEdge(goalNode, shapeNode);
                            //_edges.Add(edge);
                            goalNode.OutEdges.Add(edge);
                            shapeNode.InEdges.Add(edge);

                            //update shapeNode's upper relation
                            UpdateRelation(shapeNode);
                        }

                        #region Interaction

                        if (KnowledgeUpdated != null)
                            KnowledgeUpdated(this, shapeNode.Shape);

                        #endregion
                    }
                    #endregion
                }
            }
        }

        private void UnBuildExplicitConnection(GoalNode goalNode)
        {
            if(!goalNode.SourceVertex) throw new Exception("TODO");

            var eqGoal = goalNode.Goal as EqGoal;
            Debug.Assert(eqGoal!=null);

            for (int i = 0; i < goalNode.OutEdges.Count; i++)
            {
                GraphEdge outEdge = goalNode.OutEdges[i];
                var shapeNode = outEdge.Target as ShapeNode;
                if(shapeNode == null) throw new Exception("TODO, goal chain");

                var pt = shapeNode.Shape as Point;
                if (pt != null)
                {
                    pt.UnReify(eqGoal);
                    UpdateRelation(shapeNode);

                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, pt);
                    #endregion
                }

                var line = shapeNode.Shape as Line;
                if (line != null)
                {
                    line.UnReify(eqGoal);
                    UpdateRelation(shapeNode);
                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, line);
                    #endregion
                }

                shapeNode.InEdges.Remove(outEdge);
                goalNode.OutEdges.Remove(outEdge);
            }
        }

        /// <summary>
        /// Re-eval shapeNode itself and propagate to upper relation
        /// </summary>
        /// <param name="shapeNode"></param>
        private void UpdateRelation(ShapeNode shapeNode)
        {
            List<GraphNode> nodes = RetrieveOutEdgeNodes(shapeNode);

            foreach (GraphNode node in nodes)
            {
                var sn = node as ShapeNode;
                if (sn != null)
                {
                    List<GraphNode> constructNodes = RetrieveInEdgeNodes(node);
                    if (constructNodes.Count != 2) continue;
                    var sn1 = constructNodes[0] as ShapeNode;
                    var sn2 = constructNodes[1] as ShapeNode;
                    if (sn1 != null && sn2 != null)
                    {
                        var shape1 = sn1.Shape;
                        var shape2 = sn2.Shape;
                        bool result = RelationLogic.Reify(sn.Shape, shape1, shape2);
                        if (result)
                        {
                            #region Interaction

                            if (KnowledgeUpdated != null)
                                KnowledgeUpdated(this, sn.Shape);

                            #endregion
                        }

                        //recursive update
                        UpdateRelation(sn);
                    }
                    else
                    {
                        //TODO
                        continue;
                    }
                }
            }
         }

        #region Search Utilities

        private List<GraphNode> RetrieveOutEdgeNodes(GraphNode node)
        {
            var lst = new List<GraphNode>();
            foreach (var ge in node.OutEdges)
            {
                lst.Add(ge.Target);
            }
            return lst;
        }

        private List<GraphNode> RetrieveInEdgeNodes(GraphNode node)
        {
            var lst = new List<GraphNode>();
            foreach (var ge in node.InEdges)
            {
                lst.Add(ge.Source);
            }
            return lst;
        }

        private GraphNode SearchNode(object obj)
        {
            var shape = obj as Shape;
            if (shape != null)
            {
                foreach (GraphNode gn in _nodes)
                {
                    var shapeNode = gn as ShapeNode;
                    if (shapeNode != null && shapeNode.Shape.Equals(shape))
                    {
                        return gn;
                    }
                }
                return null;
            }
            var goal = obj as Goal;
            if (goal != null)
            {
                foreach (GraphNode gn in _nodes)
                {
                    var goalNode = gn as GoalNode;
                    if (goalNode != null && goalNode.Goal.Equals(goal))
                    {
                        return gn;
                    }
                }
                return null;
            }

            return null;
        }

        #endregion
    }
}
