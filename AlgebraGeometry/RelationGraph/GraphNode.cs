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
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.ComponentModel;
    using CSharpLogic;

    public abstract class GraphNode : INotifyPropertyChanged
    {
        private List<GraphEdge> _inEdges;
        public List<GraphEdge> InEdges
        {
            get { return _inEdges; }
        }

        private List<GraphEdge> _outEdges;
        public List<GraphEdge> OutEdges
        {
            get { return _outEdges; }
            set { _outEdges = value; }
        }

        protected GraphNode()
        {
            _inEdges = new List<GraphEdge>();
            _outEdges = new List<GraphEdge>();
        }

        public bool Related { get; set; }

        #region Dynamic System

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        protected void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class EquationNode : GraphNode
    {
        private Equation _equation;
        public Equation Equation
        {
            get { return _equation; }
            set { _equation = value; }
        }

        public EquationNode()
        {
        }

        public EquationNode(Equation eq)
        {
            _equation = eq;
        }
    }

    public class ShapeNode : EquationNode
    {
        private ShapeSymbol _shape;
        public ShapeSymbol ShapeSymbol
        {
            get { return _shape; }
            set
            {
                NotifyPropertyChanged("Shape");
                _shape = value;
            }
        }

        public ShapeNode(ShapeSymbol shape)
        {
            _shape = shape;
        }

        public bool IsShapeType(ShapeType st)
        {
            return _shape.Shape.ShapeType.Equals(st);
        }
    }

    public class GoalNode : EquationNode
    {
        private Goal _goal;
        public Goal Goal
        {
            get { return _goal; }
            set { _goal = value; }
        }

        public GoalNode(Goal goal)
        {
            _goal = goal;
        }
    }

    public class QueryNode : GraphNode
    {
        public Query Query { get; private set; }
        public ObservableCollection<GraphNode> InternalNodes { get; private set; }

        public QueryNode(Query query)
        {
            Query = query;
            InternalNodes = new ObservableCollection<GraphNode>();
            InternalNodes.CollectionChanged += InternalNodes_CollectionChanged;
        }

        public GraphNode SearchInternalNode(object obj)
        {
            foreach (var node in InternalNodes)
            {
                var shapeNode = node as ShapeNode;
                var goalNode = node as GoalNode;
                if (shapeNode != null)
                {
                    if (shapeNode.ShapeSymbol.Equals(obj)) return node;
                }
                if (goalNode != null)
                {
                    if (goalNode.Goal.Equals(obj)) return node;
                }
            }
            return null;
        }

        void InternalNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                #region Add action

                foreach (var gn in e.NewItems)
                {
                    var shapeNode = gn as ShapeNode;
                    var goalNode = gn as GoalNode;
                    var eqNode = gn as EquationNode;
                    if (shapeNode != null)
                    {
                        var shapeSymbol = shapeNode.ShapeSymbol;

                        var shapes = shapeSymbol.RetrieveConcreteShapes();
                        var gShapeSymbol = shapes as ShapeSymbol;
                        var shapeSymbolLst = shapes as IEnumerable<ShapeSymbol>;

                        if (gShapeSymbol != null)
                        {
                            Query.CachedEntities.Add(gShapeSymbol);
                        }
                        if (shapeSymbolLst != null)
                        {
                            foreach (var cachedItem in shapeSymbolLst)
                            {
                                Query.CachedEntities.Add(cachedItem);
                            }
                        }
                    }
                    else if (goalNode != null)
                    {
                        var eqGoal = goalNode.Goal as EqGoal;
                        Query.CachedEntities.Add(eqGoal);
                    }
                    else if (eqNode != null)
                    {
                        var equation = eqNode.Equation;
                        Query.CachedEntities.Add(equation);
                    }
                }

                #endregion
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                #region Delete Action

                foreach (var gn in e.OldItems)
                {
                    var shapeNode = gn as ShapeNode;
                    var goalNode = gn as GoalNode;
                    if (goalNode != null)
                    {
                        var eqGoal = goalNode.Goal as EqGoal;
                        if (Query.CachedEntities.Contains(eqGoal))
                        {
                            Query.CachedEntities.Remove(eqGoal);
                        }
                    }
                    if (shapeNode != null)
                    {
                        var shapeSymbol = shapeNode.ShapeSymbol;
                        var shapes = shapeSymbol.RetrieveConcreteShapes();
                        var gShapeSymbol = shapes as ShapeSymbol;
                        var shapeSymbolLst = shapes as IEnumerable<ShapeSymbol>;

                        if (gShapeSymbol != null)
                        {
                            if (Query.CachedEntities.Contains(gShapeSymbol))
                            {
                                Query.CachedEntities.Remove(gShapeSymbol);
                            }
                        }
                        if (shapeSymbolLst != null)
                        {
                            foreach (var cachedItem in shapeSymbolLst)
                            {
                                if (Query.CachedEntities.Contains(cachedItem))
                                {
                                    Query.CachedEntities.Remove(cachedItem);
                                }
                            }
                        }
                    }
                }

                #endregion
            }
        }

        public IEnumerable<GoalNode> RetrieveGoalNodes()
        {
            return InternalNodes.OfType<GoalNode>().ToList();
        }
        public IEnumerable<ShapeNode> RetrieveShapeNodes()
        {
            return InternalNodes.OfType<ShapeNode>().ToList();
        }
        public IEnumerable<ShapeNode> RetrieveShapeNodes(ShapeType st)
        {
            return InternalNodes.OfType<ShapeNode>().Where(sn => sn.IsShapeType(st)).ToList();
        }
    }

    public static class GraphNodeExtension
    {
        public static bool Satisfy(this GoalNode goalNode, Var variable)
        {
            var eqGoal = goalNode.Goal as EqGoal;
            if (eqGoal != null && eqGoal.Rhs != null)
            {
                return eqGoal.Lhs.Equals(variable);
            }
            return false;
        }

        public static bool Satisfy(this GoalNode goalNode, string variable)
        {
            var eqGoal = goalNode.Goal as EqGoal;
            if (eqGoal != null && eqGoal.Rhs != null)
            {
                return eqGoal.Lhs.ToString().Equals(variable);
            }
            return false;
        }
    }
}