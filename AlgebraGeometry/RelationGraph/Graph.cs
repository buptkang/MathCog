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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using CSharpLogic;

    public partial class RelationGraph
    {
        #region Constructor and Properties

        private List<GraphNode> _nodes;
        public List<GraphNode> Nodes
        {
            get { return _nodes; }
            set { _nodes = value; }
        }
        public RelationGraph()
        {
            _nodes = new List<GraphNode>();
            _preCache = new Dictionary<object, object>();
        }

        //TODO
        //private List<GraphNode> _selectedNode;

        /// <summary>
        /// Key: Pattern Match Result
        /// Value: Predict Pattern Match Result (GraphNode)
        /// </summary>
        private Dictionary<object, object> _preCache;
        public Dictionary<object, object> Cache
        {
            get { return _preCache; }
        }

        #endregion

        #region public Input API

        public GraphNode AddNode(object obj)
        {
            var shape = obj as ShapeSymbol;
            var goal = obj as Goal;
            var query = obj as Query;
            var equation = obj as Equation;

            if (shape != null)
            {
                var shapeNode = AddShapeNode(shape);
                _preCache.Add(shape, shapeNode);
                ReAddQueryNode();
                return shapeNode;
            }
            if (goal != null)
            {
                var goalNode = AddGoalNode(goal);
                _preCache.Add(goal, goalNode);
                ReAddQueryNode();
                return goalNode;
            }
            if (query != null)
            {
                query.Success = false;
                query.FeedBack = null;
                RemoveQueryFromCache(query);
                query.PropertyChanged += query_PropertyChanged;
                bool result = AddQueryNode(query, out obj);
                _preCache.Add(query.QueryQuid, new Tuple<Query, object>(query, obj));
                if (!result) return null;
                var qn = obj as QueryNode;
                Debug.Assert(qn != null);
                return qn;
            }
            if (equation != null)
            {
                var eqNode = AddEquationNode(equation);
                _preCache.Add(equation, eqNode);
                ReAddQueryNode();
                return eqNode;
            }
            throw new Exception("Cannot reach here");
        }

        /// <summary>
        /// Add node onto the graph
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="relNodes"></param>
        private ShapeNode AddShapeNode(ShapeSymbol shape)
        {
            var sn = new ShapeNode(shape);
            Reify(sn);          //reify shapeNode itself
            UpdateRelation(sn); //build connection
            _nodes.Add(sn);
            return sn;
        }

        private EquationNode AddEquationNode(Equation equation)
        {
            var en = new EquationNode(equation);
            Reify(en);
            _nodes.Add(en);
            return en;
        }

        /// <summary>
        /// Add node onto the graph
        /// </summary>
        /// <param name="goal"></param>
        private GoalNode AddGoalNode(Goal goal)
        {
            var gn = new GoalNode(goal);
            var eqGoal = goal as EqGoal;
            Debug.Assert(eqGoal != null);
            Debug.Assert(eqGoal.Rhs != null);
            Reify(gn);
            //UpdateRelation(goal);
            _nodes.Add(gn);
            return gn;
        }

        private bool AddQueryNode(Query query, out object obj)
        {
            ResetNodeRelatedFlag();
            bool result = ConstraintSatisfy(query, out obj);
            if (!result) return false;
            var qn = obj as QueryNode;
            _nodes.Add(qn);
            return true;
        }

        public void DeleteNode(object obj)
        {
            var shape = obj as ShapeSymbol;
            var goal = obj as EqGoal;
            var query = obj as Query;
            if (shape != null) DeleteShapeNode(shape);
            if (goal != null) DeleteGoalNode(goal);
            if (query != null) DeleteQueryNode(query);
            ReAddQueryNode();
        }

        /// <summary>
        /// Delete node from the graph
        /// </summary>
        /// <param name="shape"></param>
        private void DeleteShapeNode(ShapeSymbol shape)
        {
            var shapeNode = SearchNode(shape) as ShapeNode;
            if (shapeNode != null)
            {
                for (int i = 0; i < shapeNode.OutEdges.Count; i++)
                {
                    GraphEdge outEdge = shapeNode.OutEdges[i];
                    var targetNode = outEdge.Target as ShapeNode;
                    if (targetNode != null)
                    {
                        DeleteShapeNode(targetNode.ShapeSymbol);
                        shapeNode.OutEdges.Remove(outEdge);
                    }
                }
                for (int j = 0; j < shapeNode.InEdges.Count; j++)
                {
                    var inEdge = shapeNode.InEdges[j];
                    var sourceNode = inEdge.Source;
                    sourceNode.OutEdges.Remove(inEdge);
                }
                _nodes.Remove(shapeNode);
                _preCache.Remove(shape);
            }
        }

        /// <summary>
        /// Delete node from the graph
        /// </summary>
        /// <param name="goal"></param>
        private void DeleteGoalNode(Goal goal)
        {
            var goalNode = SearchNode(goal) as GoalNode;
            if (goalNode != null)
            {
                UnReify(goalNode);
                _nodes.Remove(goalNode);
                _preCache.Remove(goal);   
            }
        }

        private void DeleteQueryNode(Query query)
        {
            RemoveQueryFromCache(query);
            query.PropertyChanged -= query_PropertyChanged;
            var queryNode = SearchNode(query) as QueryNode;
            if (queryNode != null)
            {
                foreach (GraphNode gn in queryNode.InternalNodes)
                {
                    foreach (GraphEdge inGe in gn.InEdges)
                    {
                        var sourceNode = inGe.Source;
                        sourceNode.OutEdges.Remove(inGe);
                    }
                    foreach (GraphEdge outGe in gn.OutEdges)
                    {
                        var targetNode = outGe.Target;
                        targetNode.InEdges.Remove(outGe);
                    }
                }
                query.CachedEntities.Clear();
                _nodes.Remove(queryNode);
            }
        }

        void query_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var query = sender as Query;
            DeleteNode(query);
            AddNode(query);
        }

        private void ReAddQueryNode()
        {
            foreach (KeyValuePair<object, object> pair in _preCache.ToList())
            {
                if (pair.Key is Guid)
                {
                    var tuple2 = pair.Value as Tuple<Query, object>;
                    Debug.Assert(tuple2 != null);
                    var query = tuple2.Item1;
                    Debug.Assert(query != null);
                    DeleteNode(query);
                    AddNode(query);
                }
            }
        }

        #endregion

        #region Edge Utilities

        private void CreateEdge(GraphNode source, GraphNode target)
        {
            var graphEdge = new GraphEdge(source, target);
            source.OutEdges.Add(graphEdge);
            target.InEdges.Add(graphEdge);
        }

        #endregion

        #region Search Graph Node Utilities

        public List<GraphNode> RetrieveRelationBasedNode()
        {
            /*            return (from node in Nodes
                                let shapeNode = node as ShapeNode
                                where shapeNode != null &&
                                      shapeNode.Shape
                                select node)
                               .ToList();
            */
            return null;
        }

        public List<GraphNode> RetrieveNonRelatioinBasedNode()
        {
            return null;
            /* var lst = new List<GraphNode>();
            foreach (GraphNode node in Nodes)
            {
                var shapeNode = node as ShapeNode;
                if (shapeNode == null) continue;
                if (!shapeNode.Shape.RelationStatus)
                {
                    lst.Add(node);
                }
            }
            return lst;*/
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
            var shape = obj as ShapeSymbol;
            if (shape != null)
            {
                foreach (var gn in _nodes)
                {
                    var shapeNode = gn as ShapeNode;
                    if (shapeNode != null)
                    {
                        if (shapeNode.ShapeSymbol.Equals(shape)) return gn;
                    }

                    var queryNode = gn as QueryNode;
                    if (queryNode != null)
                    {
                        object returnObj = queryNode.SearchInternalNode(obj);
                        if (returnObj != null) return (GraphNode)returnObj;
                    }
                }
            }
            var goal = obj as EqGoal;
            if (goal != null)
            {
                foreach (var gn in _nodes)
                {
                    var goalNode = gn as GoalNode;
                    if (goalNode != null)
                    {
                        if (goalNode.Goal.Equals(goal)) return gn;
                    }

                    var queryNode = gn as QueryNode;
                    if (queryNode != null)
                    {
                        object returnObj = queryNode.SearchInternalNode(obj);
                        if (returnObj != null) return (GraphNode)returnObj;
                    }
                }
            }
            var query = obj as Query;
            if (query != null)
            {
                foreach (var gn in _nodes)
                {
                    var qn = gn as QueryNode;
                    if (qn != null && qn.Query.QueryQuid.Equals(query.QueryQuid))
                        return qn;
                }
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

        private GraphNode RetrieveObjectByLabel(string label)
        {
            foreach (var node in Nodes)
            {
                var shapeNode = node as ShapeNode;
                if (shapeNode != null)
                {
                    if (shapeNode.ShapeSymbol.Shape.Label.Equals(label))
                    {
                        return shapeNode;
                    }
                }

                var goalNode = node as GoalNode;
                if (goalNode != null)
                {
                    var equalGoal = goalNode.Goal as EqGoal;
                    Debug.Assert(equalGoal != null);
                    if (equalGoal.EqLabel.Equals(label))
                    {
                        return goalNode;
                    }
                }
            }

            return null;
        }

        private void ResetNodeRelatedFlag()
        {
            foreach (var gn in _nodes)
            {
                gn.Related = false;
            }
        }

        #endregion

        #region Search Cache Utilities

        private void RemoveQueryFromCache(Query query)
        {
            object keyObj = null;
            foreach (KeyValuePair<object, object> pair in _preCache)
            {
                if (pair.Key is Guid)
                {
                    Debug.Assert(pair.Key != null);
                    var tempGuid = (Guid)pair.Key;
                    if (tempGuid.CompareTo(query.QueryQuid) == 0)
                    {
                        keyObj = pair.Key;
                        break;
                    }
                }
            }
            if (keyObj != null) _preCache.Remove(query.QueryQuid);
        }

        public object RetrieveCacheValue(object obj)
        {
            return (from pair in Cache where pair.Key.Equals(obj) select pair.Value).FirstOrDefault();
        }

        private List<KeyValuePair<object, object>> RetrieveMultiObjectsPairs()
        {
            var lst = new List<KeyValuePair<object, object>>();
            foreach (KeyValuePair<object, object> pair in Cache)
            {
                //var str = pair.Key as String;
                var lstTemp = pair.Key as List<object>;
                //if (str != null || lstTemp != null)
                if (lstTemp != null)
                {
                    lst.Add(pair);
                }
            }
            return lst;
        }

        #endregion

        #region Output Test API

        public QueryNode RetrieveQueryNode(Query query)
        {
            foreach (var gn in _nodes)
            {
                var qn = gn as QueryNode;
                if (qn != null && qn.Query.QueryQuid.Equals(query.QueryQuid))
                {
                    return qn;
                }
            }
            return null;
        }

        public IEnumerable<GoalNode> RetrieveGoalNodes()
        {
            var goalNodes = new List<GoalNode>();
            foreach (var gn in _nodes)
            {
                var goalNode = gn as GoalNode;
                if (goalNode != null) goalNodes.Add(goalNode);
                var queryNode = gn as QueryNode;
                if (queryNode != null) goalNodes.AddRange(queryNode.RetrieveGoalNodes());
            }
            return goalNodes;
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

        public List<ShapeNode> RetrieveShapeNodes()
        {
            var shapeNodes = new List<ShapeNode>();
            foreach (var gn in _nodes)
            {
                var sn = gn as ShapeNode;
                if (sn != null) shapeNodes.Add(sn);
                var queryNode = gn as QueryNode;
                if (queryNode != null) shapeNodes.AddRange(queryNode.RetrieveShapeNodes());
            }
            return shapeNodes;
        }

        public List<ShapeNode> RetrieveShapeNodes(ShapeType type)
        {
            var shapeNodes = new List<ShapeNode>();
            foreach (GraphNode gn in _nodes)
            {
                var sn = gn as ShapeNode;
                if (sn != null && sn.IsShapeType(type)) shapeNodes.Add(sn);
                var qn = gn as QueryNode;
                if (qn != null) shapeNodes.AddRange(qn.RetrieveShapeNodes(type));
            }
            return shapeNodes;
        }

        public List<ShapeSymbol> RetrieveShapeSymbols()
        {
            var lstNodes = RetrieveShapeNodes();
            return lstNodes.Select(sn => sn.ShapeSymbol).ToList();
        }

        public List<ShapeSymbol> RetrieveShapeSymbols(ShapeType type)
        {
            var lstNodes = RetrieveShapeNodes(type);
            return lstNodes.Select(sn => sn.ShapeSymbol).ToList();
        }

        public List<Shape> RetrieveShapes()
        {
            var lstSs = RetrieveShapeSymbols();
            return lstSs.Select(ss => ss.Shape).ToList();
        }

        public List<Shape> RetrieveShapes(ShapeType st)
        {
            var lstSs = RetrieveShapeSymbols(st);
            return lstSs.Select(ss => ss.Shape).ToList();
        }

        #endregion

        #region  (TODO) Graph Node Selections
        /*
        public void SelectNode(GraphNode gn)
        {
            if (!_relationGraph.Nodes.Contains(gn))
                throw new Exception("Root graph needs to contain select node!");

            _selectedNode.Add(gn);
        }

        public void DeSelectNode(GraphNode gn)
        {
            if (!_relationGraph.Nodes.Contains(gn))
            {
                throw new Exception("Root graph needs to contain select node!");
            }

            if (!_selectedNode.Contains(gn))
            {
                throw new Exception("The node should be selected before.");
            }

            _selectedNode.Remove(gn);
        }*/

        #endregion
    }
}