using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    /// <summary>
    /// Pattern Match String (Unification)
    /// Graph Search -- Constraint Solving 
    /// Goal oriented Search
    /// </summary>
    public partial class RelationGraph
    {
        // Lazy evaluation of relation based on the existing nodes,
        /// if the relation exists, build the relation.
        private bool ConstraintSatisfy(Query query, out object obj)
        {
            object refObj = query.Constraint1;
            ShapeType? st = query.Constraint2;
            if (refObj == null && st == null) throw new Exception("TODO: brute search to create all relations.");
            bool result = ConstraintSatisfy(refObj != null ? refObj.ToString() : null, st, out obj);
            if (result)
            {
                obj = CreateQueryNode(query, obj);
                query.Success = true;
                return true;
            }
            if (obj != null)
            {
                query.Success = false;
                query.FeedBack = obj;
            }
            return false;
        }

        private QueryNode CreateQueryNode(Query query, object obj)
        {
            var queryNode = new QueryNode(query);
            var unaryDict = obj as Dictionary<GraphNode, object>;
            if (unaryDict != null)
            {
                foreach (KeyValuePair<GraphNode, object> pair in unaryDict)
                {
                    var eqGoal = pair.Value as EqGoal;
                    var shapeSymbol = pair.Value as ShapeSymbol;
                    var queryNode1 = pair.Key as QueryNode;
                    Debug.Assert(queryNode1 == null);
                    if (eqGoal != null)
                    {
                        var gGoalNode = new GoalNode(eqGoal);
                        queryNode.InternalNodes.Add(gGoalNode);
                        var sourceNode = pair.Key;
                        Debug.Assert(sourceNode != null);
                        CreateEdge(sourceNode, gGoalNode);
                        continue;
                    }
                    if (shapeSymbol != null)
                    {
                        var gShapeNode = new ShapeNode(shapeSymbol);
                        queryNode.InternalNodes.Add(gShapeNode);
                        var sourceNode = pair.Key;
                        Debug.Assert(sourceNode != null);
                        CreateEdge(sourceNode, gShapeNode);
                        continue;
                    }
                }
                return queryNode;
            }
            //Binary Dict
            var binaryDict = obj as Dictionary<Tuple<GraphNode, GraphNode>, object>;
            if (binaryDict != null)
            {
                foreach (KeyValuePair<Tuple<GraphNode, GraphNode>, object> pair in binaryDict)
                {
                    var gShape = pair.Value as ShapeSymbol;
                    if (gShape == null) continue; //TODO
                    var shapeNode = new ShapeNode(gShape);
                    queryNode.InternalNodes.Add(shapeNode);
                    var sourceNode1 = pair.Key.Item1;
                    var sourceNode2 = pair.Key.Item2;
                    Debug.Assert(sourceNode1 != null);
                    Debug.Assert(sourceNode2 != null);
                    CreateEdge(sourceNode1, shapeNode);
                    CreateEdge(sourceNode2, shapeNode);
                }
                return queryNode;
            }
            //TODO: TrinaryDict
            throw new Exception("Cannot reach here");
        }

        private void DeleteQueryNode(QueryNode qn)
        {
            
        }
        /// <summary>
        /// This func takes charge of pattern match string with 
        /// existing nodes
        /// Two constraint
        /// Priority: label constraint > shapeType constraint
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="label"></param>
        /// <param name="st"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool ConstraintSatisfy(string label, ShapeType? st, out object obj)
        {
            obj = null;
            var graphNodes = _nodes;

            #region Unary Relation Checking

            var unaryDict = new Dictionary<GraphNode, object>();
            for (var i = 0; i < graphNodes.Count; i++)
            {
                var tempNode = graphNodes[i];
                bool result = RelationLogic.ConstraintCheck(tempNode, label, st, out obj);
                if (result)
                {                   
                    var queryNode = tempNode as QueryNode;
                    if(queryNode == null) unaryDict.Add(tempNode, obj);
                    else
                    {
                        foreach (var tempGn in queryNode.InternalNodes)
                        {
                            if(tempGn.Related) unaryDict.Add(tempGn, obj);
                        }
                    }
                }
            }
            #endregion

            #region Binary Relation Checking
            //Two layer for loop to iterate all Nodes
            //binary relation build up

            // overfitting: 
            // underfitting: 
            var binaryDict = new Dictionary<Tuple<GraphNode, GraphNode>, object>();
            for (var i = 0; i < graphNodes.Count - 1; i++)
            {
                var outerNode = graphNodes[i];
                for (var j = i + 1; j < graphNodes.Count; j++)
                {
                    var innerNode = graphNodes[j];
                    var tuple = new Tuple<GraphNode, GraphNode>(outerNode, innerNode);
                    bool result = RelationLogic.ConstraintCheck(outerNode, innerNode, label, st, out obj);
                    if (result) binaryDict.Add(tuple, obj);
                }
            }
            #endregion

            //TODO analysis
            if (unaryDict.Count != 0)
            {
                obj = unaryDict;
                return true;
            }
            if (binaryDict.Count != 0)
            {
                obj = binaryDict;
                return true;
            }
            return false;
        }
    }
}
