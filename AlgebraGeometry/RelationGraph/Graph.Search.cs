using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class RelationGraph
    {
        #region Symbolic Reification

        private void Reify(ShapeNode shapeNode)
        {
            if (shapeNode.Shape.Concrete) return;

            List<GoalNode> goalNodes = RetrieveGoalNodes();
            foreach (GoalNode goalNode in goalNodes)
            {
                var pt = shapeNode.Shape as Point;
                if (pt != null)
                {
                    bool result = pt.Reify((EqGoal)goalNode.Goal);
                    if (result)
                    {
                        var edge = new GraphEdge(goalNode, shapeNode);
                        goalNode.OutEdges.Add(edge);
                        shapeNode.InEdges.Add(edge);
                    }

                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, shapeNode.Shape);
                    #endregion
                    continue;
                }
                var line = shapeNode.Shape as Line;
                if (line != null)
                {
                    bool result = line.Reify((EqGoal)goalNode.Goal);
                    if (result)
                    {
                        var edge = new GraphEdge(goalNode, shapeNode);
                        goalNode.OutEdges.Add(edge);
                        shapeNode.InEdges.Add(edge);
                    }
                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, shapeNode.Shape);
                    #endregion

                    continue;
                }
            }
        }

        private void Reify(GoalNode goalNode)
        {
            var eqGoal = goalNode.Goal as EqGoal;
            Debug.Assert(eqGoal != null);
            foreach (GraphNode gn in _nodes) //breadth first search
            {
                var shapeNode = gn as ShapeNode;
                if (shapeNode == null) continue;

                if (shapeNode.Shape.Concrete) continue;
                bool reifyResult = false;

                #region Shape Type Dynamic Reification
                //Point Reification
                var pt = shapeNode.Shape as Point;
                if (pt != null) reifyResult = pt.Reify(eqGoal);

                if (reifyResult)
                {
                    var edge = new GraphEdge(goalNode, shapeNode);
                    goalNode.OutEdges.Add(edge);
                    shapeNode.InEdges.Add(edge);
                    ReifyByRelation(shapeNode); //dfs
                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, shapeNode.Shape);
                    #endregion
                    continue;
                }
 
                //Line Reification
                var line = shapeNode.Shape as Line;
                if (line != null) reifyResult = line.Reify(eqGoal);
                if (reifyResult)
                {
                    var edge = new GraphEdge(goalNode, shapeNode);
                    goalNode.OutEdges.Add(edge);
                    shapeNode.InEdges.Add(edge);
                    ReifyByRelation(shapeNode); //dfs
                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, shapeNode.Shape);
                    #endregion
                    continue;
                }
                #endregion
            }
        }

        private void UnReify(GoalNode goalNode)
        {
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
                    ReifyByRelation(shapeNode);
                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, pt);
                    #endregion
                }

                var line = shapeNode.Shape as Line;
                if (line != null)
                {
                    line.UnReify(eqGoal);
                    ReifyByRelation(shapeNode);
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
        /// Depth First Search
        /// </summary>
        /// <param name="shapeNode"></param>
        private void ReifyByRelation(ShapeNode shapeNode)
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
                        ReifyByRelation(sn);
                    }
                    else
                    {
                        //TODO
                        continue;
                    }
                }
            }
        }

        #endregion

        #region Symbolic Unification

        /// <summary>
        /// Build relation with relation-based obj
        /// </summary>
        /// <param name="gn">non-relation based input ShapeNode</param>
        private void UpdateRelation(GraphNode gn)
        {
            var shapeNode = gn as ShapeNode;
            if (shapeNode == null) return;//TODO, GoalNode

            //find all the relation-based graphNode
            List<GraphNode> relationNodes = RetrieveRelationBasedNode();

            foreach (GraphNode gnn in relationNodes)
            {
                var relationNode = gnn as ShapeNode;
                Debug.Assert(relationNode != null);
                
                //check relation existence
                if (RelationLogic.VerifyRelation(relationNode.Shape, shapeNode.Shape))
                {
                    var graphEdge = new GraphEdge(shapeNode, relationNode);
                    shapeNode.OutEdges.Add(graphEdge);
                    relationNode.InEdges.Add(graphEdge);
                    ReifyByRelation(shapeNode); // reify relationnodes
                }
            }
        }

        /// <summary>
        /// Build relation with non-relation based obj
        /// TODO: build relation with other relation based obj
        /// </summary>
        /// <param name="shape">relation based input ShapeNode</param>
        /// <returns></returns>
        public void UnifyRelation(ShapeNode relNode)
        {
            Debug.Assert(relNode.Shape.Label != null);
            Debug.Assert(relNode.Shape.RelationStatus);

            //Find all the non-relation based obj
            List<GraphNode> nonRelNodes = RetrieveNonRelatioinBasedNode();

            foreach (GraphNode gn in nonRelNodes)
            {
                var nonRelNode = gn as ShapeNode;
                Debug.Assert(nonRelNode != null);

                //check relation existence
                if (RelationLogic.VerifyRelation(relNode.Shape, nonRelNode.Shape))
                {
                    var graphEdge = new GraphEdge(nonRelNode, relNode);
                    nonRelNode.OutEdges.Add(graphEdge);
                    relNode.InEdges.Add(graphEdge);
                    ReifyByRelation(nonRelNode); // reify Relation Nodes
                }
            }
        }

        #endregion

        #region Search Utilities

        public List<GraphNode> RetrieveRelationBasedNode()
        {
            return (from node in Nodes let shapeNode = node as ShapeNode 
                    where shapeNode != null && 
                          shapeNode.Shape.RelationStatus select node)
                   .ToList();
        }

        public List<GraphNode> RetrieveNonRelatioinBasedNode()
        {
            var lst = new List<GraphNode>();
            foreach (GraphNode node in Nodes)
            {
                var shapeNode = node as ShapeNode;
                if (shapeNode == null) continue;
                if (!shapeNode.Shape.RelationStatus)
                {
                    lst.Add(node);
                }
            }
            return lst;
        }

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

        public List<Var> RetrieveShapeInternalVars()
        {
            var lst = new List<Var>();
            var shapes = RetrieveShapes();
            foreach (Shape shape in shapes)
            {
                lst.AddRange(shape.GetVars());
            }
            return lst;
        }

        #endregion
    }
}
