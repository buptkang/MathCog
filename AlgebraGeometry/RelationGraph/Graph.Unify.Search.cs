using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
            obj = null;
            object refObj = query.Constraint1;
            ShapeType? st = query.Constraint2;
            if (refObj == null && st == null) throw new Exception("TODO: brute search to create all relations.");
            bool result = false;
            if (refObj == null) result = ConstraintSatisfy(null, st, out obj);
            else
            {
                var term = refObj as Term;
                if (term != null)
                {
                    result = ConstraintSatisfy(term, out obj);
                }
                else
                {
                    result = ConstraintSatisfy(refObj.ToString(), st, out obj);        
                }
            }
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

            var dict = obj as Dictionary<object, object>;
            if (dict != null)
            {
                #region Unary and Binary Relation

                foreach (KeyValuePair<object, object> pair in dict)
                {
                    var unaryNode  = pair.Key as GraphNode;
                    var binaryNode = pair.Key as Tuple<GraphNode, GraphNode>;
                    var uunaryNode = pair.Key as List<GraphNode>;

                    if (unaryNode != null)
                    {
                        #region Unary Node
                        var eqGoal = pair.Value as EqGoal;
                        var shapeSymbol = pair.Value as ShapeSymbol;
                        if (eqGoal != null)
                        {
                            var gGoalNode = new GoalNode(eqGoal);
                            queryNode.InternalNodes.Add(gGoalNode);
                            CreateEdge(unaryNode, gGoalNode);
                            continue;
                        }
                        if (shapeSymbol != null)
                        {
                            var gShapeNode = new ShapeNode(shapeSymbol);
                            queryNode.InternalNodes.Add(gShapeNode);
                            var sourceNode = pair.Key;
                            Debug.Assert(sourceNode != null);
                            CreateEdge(unaryNode, gShapeNode);
                            continue;
                        }
                        #endregion
                    }

                    if (binaryNode != null)
                    {
                        #region Binary Node
                        var gShape = pair.Value as ShapeSymbol;
                        if (gShape == null) continue; //TODO
                        var shapeNode = new ShapeNode(gShape);
                        queryNode.InternalNodes.Add(shapeNode);
                        var sourceNode1 = binaryNode.Item1;
                        var sourceNode2 = binaryNode.Item2;
                        Debug.Assert(sourceNode1 != null);
                        Debug.Assert(sourceNode2 != null);
                        CreateEdge(sourceNode1, shapeNode);
                        CreateEdge(sourceNode2, shapeNode);
                        #endregion
                    }

                    var findNode = SearchNode(pair.Key);
                    if (findNode != null)
                    {
                        #region Find Node
                        var eqGoal = pair.Value as EqGoal;
                        var shapeSymbol = pair.Value as ShapeSymbol;
                        if (eqGoal != null)
                        {
                            var gGoalNode = new GoalNode(eqGoal);
                            queryNode.InternalNodes.Add(gGoalNode);
                            CreateEdge(findNode, gGoalNode);
                            continue;
                        }
                        if (shapeSymbol != null)
                        {
                            var gShapeNode = new ShapeNode(shapeSymbol);
                            queryNode.InternalNodes.Add(gShapeNode);
                            var sourceNode = pair.Key;
                            Debug.Assert(sourceNode != null);
                            CreateEdge(findNode, gShapeNode);
                            continue;
                        }
                        #endregion
                    }

                    var findNodeInCurrentqQuery = queryNode.SearchInternalNode(pair.Key);
                    if (findNodeInCurrentqQuery != null)
                    {
                        #region Find Node
                        var eqGoal = pair.Value as EqGoal;
                        var shapeSymbol = pair.Value as ShapeSymbol;
                        if (eqGoal != null)
                        {
                            var gGoalNode = new GoalNode(eqGoal);
                            queryNode.InternalNodes.Add(gGoalNode);
                            CreateEdge(findNodeInCurrentqQuery, gGoalNode);
                            continue;
                        }
                        if (shapeSymbol != null)
                        {
                            var gShapeNode = new ShapeNode(shapeSymbol);
                            queryNode.InternalNodes.Add(gShapeNode);
                            var sourceNode = pair.Key;
                            Debug.Assert(sourceNode != null);
                            CreateEdge(findNodeInCurrentqQuery, gShapeNode);
                            continue;
                        }
                        #endregion
                    }

                    if (uunaryNode != null)
                    {
                        var equation = pair.Value as Equation;
                        var eqNode = new EquationNode(equation);
                        queryNode.InternalNodes.Add(eqNode);
                        foreach (GraphNode gn in uunaryNode)
                        {
                            CreateEdge(gn, eqNode);
                        }                       
                    }
                }

                #endregion
            }
              
            return queryNode;
        }

        /// <summary>
        /// a+1=, 2+3+5=
        /// </summary>
        /// <param name="term"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool ConstraintSatisfy(Term term, out object obj)
        {
            var evalObj = term.Eval();
            var generatedEq = new Equation(term, evalObj);
            generatedEq.TransformTermTrace(true);

            var evalTerm = evalObj as Term;
            var evalVar = evalObj as Var;

            var dict = new Dictionary<object, object>();
            var connectLst = new List<GraphNode>();
            if (evalTerm != null)
            {
                for (var i = 0; i < _nodes.Count; i++)
                {
                    var goalNode = _nodes[i] as GoalNode;
                    if (goalNode != null)
                    {
                        if (evalTerm.ContainsVar((EqGoal)goalNode.Goal))
                        {
                            connectLst.Add(goalNode);
                        }
                    }
                }
            }
            if (evalVar != null)
            {               
                for (var i = 0; i < _nodes.Count; i++)
                {
                    var goalNode = _nodes[i] as GoalNode;
                    if (goalNode != null)
                    {
                        var eqGoal = goalNode.Goal as EqGoal;
                        Debug.Assert(eqGoal != null);
                        var lhsVar = eqGoal.Lhs as Var;
                        Debug.Assert(lhsVar != null);
                        if (lhsVar.Equals(evalVar))
                        {
                            connectLst.Add(goalNode);
                        }
                    }
                }
            }
            dict.Add(connectLst, generatedEq);
            obj = dict;
            return true;
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

            var dict = new Dictionary<object, object>();

            #region Unary Relation Checking

            for (var i = 0; i < graphNodes.Count; i++)
            {
                var tempNode = graphNodes[i];
                bool result = RelationLogic.ConstraintCheck(tempNode, label, st, out obj);
                if (result)
                {                   
                    var queryNode = tempNode as QueryNode;
                    if (queryNode == null) dict.Add(tempNode, obj);
                    else
                    {
                        foreach (var tempGn in queryNode.InternalNodes)
                        {
                            if (tempGn.Related) dict.Add(tempGn, obj);
                        }
                    }
                }
            }

            if (dict.Count != 0)
            {
                obj = dict;
                return true;
            }

            #endregion

            #region Binary Relation Checking
            //Two layer for loop to iterate all Nodes
            //binary relation build up

            // overfitting: 
            // underfitting: 
            for (var i = 0; i < graphNodes.Count - 1; i++)
            {
                var outerNode = graphNodes[i];
                for (var j = i + 1; j < graphNodes.Count; j++)
                {
                    var innerNode = graphNodes[j];
                    var tuple = new Tuple<GraphNode, GraphNode>(outerNode, innerNode);
                    bool result = RelationLogic.ConstraintCheck(outerNode, innerNode, label, st, out obj);
                    if (result)
                    {
                        var lst = obj as List<object>;
                        if (lst != null)
                        {
                            object source, target;
                            source = tuple;
                            foreach (var tempObj in lst)
                            {
                                target = tempObj;
                                dict.Add(source, target);
                                source = target;
                            }
                        }
                        else
                        {
                            dict.Add(tuple, obj);
                        }
                    }
                }
            }
            #endregion

            //TODO analysis
            if (dict.Count != 0)
            {
                obj = dict;
                return true;
            }
            return false;
        }
    }
}
