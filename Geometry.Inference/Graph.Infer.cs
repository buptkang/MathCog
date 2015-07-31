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
    public static class GraphExtension
    {
        public static bool Infer(this RelationGraph graph,
           object refObj, ShapeType? st, out object obj)
        {
            obj = null;
            if (refObj == null) return graph.Infer(st, out obj);
            if (refObj is Shape || refObj is EqGoal) throw new Exception("Graph.Infer.cs: Cannot reach here");
            var label = refObj as string;
            if (label != null) return graph.Infer(label, st, out obj);
            return false;
        }

        /// <summary>
        /// Lazy evaluation of relation based on the existing nodes,
        /// if the relation exists, build the relation.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="refObj"></param>
        /// <param name="st"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool Infer(this RelationGraph graph,
            ShapeType? st, out object obj)
        {
            obj = null;
            if (st == null) throw new Exception("TODO: brute search to create all relations.");

            //Two layer for loop to iterate all Nodes
            //binary relation build up
            
            var graphNodes = graph.Nodes;
            if (graphNodes.Count < 2) return false;

            // overfitting: 
            // underfitting: 
            var dict = new Dictionary<Tuple<GraphNode, GraphNode>, object>();

            #region Algorithmic Matching

            for (var i = 0; i < graphNodes.Count-1; i++)
            {
                var outerNode = graphNodes[i];
                for (var j = i + 1; j < graphNodes.Count; j++)
                {
                    var innerNode = graphNodes[j];
                    var tuple = new Tuple<GraphNode, GraphNode>(outerNode, innerNode);

                    var outerShapeNode = outerNode as ShapeNode;
                    var outerGoalNode  = outerNode as GoalNode;
                    var innerShapeNode = innerNode as ShapeNode;
                    var innerGoalNode  = innerNode as GoalNode;

                    bool result;
                    if (outerShapeNode != null)
                    {
                        if (innerShapeNode != null)
                        {
                            result = RelationLogic.CreateRelation(outerShapeNode.Shape, innerShapeNode.Shape, st, out obj);
                            if (result)
                            {
                                dict.Add(tuple, obj);
                            }
                        }
                        else
                        {
                            Debug.Assert(innerGoalNode != null);
                            result = RelationLogic.CreateRelation(outerShapeNode.Shape, innerGoalNode.Goal, st, out obj);
                            if (result)
                            {
                                dict.Add(tuple, obj);
                            }
                        }
                    }
                    else
                    {
                        Debug.Assert(outerGoalNode != null);
                        if (innerShapeNode != null)
                        {
                            result = RelationLogic.CreateRelation(outerGoalNode.Goal, innerShapeNode.Shape, st, out obj);
                            if (result)
                            {
                                dict.Add(tuple, obj);
                            }
                        }
                        else
                        {
                            Debug.Assert(innerGoalNode != null);
                            result = RelationLogic.CreateRelation(outerGoalNode.Goal, innerGoalNode.Goal, st, out obj);
                            if (result)
                            {
                                dict.Add(tuple, obj);
                            }
                        }
                    }
                }
            }
            #endregion

            if (dict.Count == 0)
            {
                return false;
            }
            else
            {
                obj = dict;
                return true;
            }
        }

        /// <summary>
        /// This func takes charge of pattern match string with 
        /// existing nodes
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="label"></param>
        /// <param name="st"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool Infer(this RelationGraph graph,
            string label, ShapeType? st, out object obj)
        {
            obj = null;
            Debug.Assert(label != null);

            //constraint shapeType processing first
            if (st != null)
            {
                bool tempResult = graph.Infer(st, out obj);
                if (tempResult) return true;
            }

            char[] charArr = label.ToCharArray();

            if (charArr.Length == 1)
            {
                //case by case label analysis
                
            }
            else if (charArr.Length == 2)
            {
                #region Analysis
                string str1 = label.ToCharArray()[0].ToString();
                string str2 = label.ToCharArray()[1].ToString();

                GraphNode gn1 = graph.RetrieveObjectByLabel(str1);
                GraphNode gn2 = graph.RetrieveObjectByLabel(str2);

                if (gn1 == null || gn2 == null) return false;

                var shapeNode1 = gn1 as ShapeNode;
                var shapeNode2 = gn2 as ShapeNode;
                bool result;
                if (shapeNode1 != null)
                {
                    if (shapeNode2 != null)
                    {
                        result = RelationLogic.CreateRelation(shapeNode1.Shape, shapeNode2.Shape, st, out obj);                        
                    }
                    else
                    {
                        var goalNode2 = gn2 as GoalNode;
                        Debug.Assert(goalNode2 != null);
                        result = RelationLogic.CreateRelation(shapeNode1.Shape, goalNode2.Goal, st, out obj);
                    }
                }
                else
                {
                    var goalNode1 = gn1 as GoalNode;
                    Debug.Assert(goalNode1 != null);
                    if (shapeNode2 != null)
                    {
                        result = RelationLogic.CreateRelation(goalNode1.Goal, shapeNode2.Shape, st, out obj);
                    }
                    else
                    {
                        var goalNode2 = gn2 as GoalNode;
                        Debug.Assert(goalNode2 != null);
                        result = RelationLogic.CreateRelation(goalNode1.Goal, goalNode2.Goal, st, out obj);
                    }
                }
                #endregion

                if (result)
                {
                    var dict = new Dictionary<Tuple<GraphNode, GraphNode>, object>();
                    dict.Add(new Tuple<GraphNode, GraphNode>(gn1, gn2), obj);
                    obj = dict;
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
