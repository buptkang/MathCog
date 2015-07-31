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
    /// Symbolic Unification
    /// </summary>
    public partial class RelationGraph
    {
        /// <summary>
        /// Build relation with relation-based obj
        /// </summary>
        /// <param name="gn">non-relation based input ShapeNode</param>
        private void UpdateRelation(GraphNode gn)
        {
            var shapeNode = gn as ShapeNode;
            if (shapeNode != null)
            {
                #region Shape Unify
                //TODO
                //find all the relation-based graphNode
              /*  List<GraphNode> relationNodes = RetrieveRelationBasedNode();

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
                        ReifyByRelation(shapeNode); // reify relation nodes
                    }
                }*/
                #endregion
            }

            var goalNode = gn as GoalNode;
            if (goalNode != null)
            {
                var eqGoal = goalNode.Goal as EqGoal;
                Debug.Assert(eqGoal != null);
                //TODO
            }
        }
    }
}
