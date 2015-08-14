using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    /// <summary>
    /// Symbolic Reification, bottom-up process
    /// </summary>
    public partial class RelationGraph
    {
        private void Reify(EquationNode eqNode)
        {
            //TODO
            var equation = eqNode.Equation;
            equation.Eval();
        }

        private void Reify(ShapeNode shapeNode)
        {
            if (shapeNode.ShapeSymbol.Shape.Concrete) return;

            IEnumerable<GoalNode> goalNodes = RetrieveGoalNodes();
            foreach (GoalNode goalNode in goalNodes)
            {
                var pt = shapeNode.ShapeSymbol as PointSymbol;
                if (pt != null)
                {
                    bool result = pt.Reify((EqGoal)goalNode.Goal);
                    if (result)
                    {
                        var edge = new GraphEdge(goalNode, shapeNode);
                        goalNode.OutEdges.Add(edge);
                        shapeNode.InEdges.Add(edge);
                    }
                    /*
                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, shapeNode.Shape);
                    #endregion
                     * */
                    continue;
                }
                var line = shapeNode.ShapeSymbol as LineSymbol;
                if (line != null)
                {
                    bool result = line.Reify((EqGoal)goalNode.Goal);
                    if (result)
                    {
                        var edge = new GraphEdge(goalNode, shapeNode);
                        goalNode.OutEdges.Add(edge);
                        shapeNode.InEdges.Add(edge);
                    }
                    /*
                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, shapeNode.Shape);
                    #endregion
                    */
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

                if (shapeNode.ShapeSymbol.Shape.Concrete) continue;
                bool reifyResult = false;

                #region Shape Type Dynamic Reification
                //Point Reification
                var pt = shapeNode.ShapeSymbol as PointSymbol;
                if (pt != null) reifyResult = pt.Reify(eqGoal);

                if (reifyResult)
                {
                    var edge = new GraphEdge(goalNode, shapeNode);
                    goalNode.OutEdges.Add(edge);
                    shapeNode.InEdges.Add(edge);
                    ReifyByRelation(shapeNode); //dfs
                    /*
                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, shapeNode.Shape);
                    #endregion
                     */ 
                    continue;
                }
 
                //Line Reification
                var line = shapeNode.ShapeSymbol as LineSymbol;
                if (line != null) reifyResult = line.Reify(eqGoal);
                if (reifyResult)
                {
                    var edge = new GraphEdge(goalNode, shapeNode);
                    goalNode.OutEdges.Add(edge);
                    shapeNode.InEdges.Add(edge);
                    ReifyByRelation(shapeNode); //dfs
                    /*
                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, shapeNode.Shape);
                    #endregion
                     */ 
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

                var pt = shapeNode.ShapeSymbol as PointSymbol;
                if (pt != null)
                {
                    pt.UnReify(eqGoal);
                    ReifyByRelation(shapeNode);
                    /*
                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, pt);
                    #endregion
                     */
                }

                var line = shapeNode.ShapeSymbol as LineSymbol;
                if (line != null)
                {
                    line.UnReify(eqGoal);
                    ReifyByRelation(shapeNode);
                    /*
                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, line);
                    #endregion
                     */ 
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
                        var shapeSymbol1 = sn1.ShapeSymbol;
                        var shapeSymbol2 = sn2.ShapeSymbol;
                        bool result = RelationLogic.Reify(sn.ShapeSymbol, shapeSymbol1, shapeSymbol2);
                        if (result)
                        {
                            /*
                            #region Interaction
                            if (KnowledgeUpdated != null)
                                KnowledgeUpdated(this, sn.Shape);
                            #endregion
                             */ 
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
    }
}