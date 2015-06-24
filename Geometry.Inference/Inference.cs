using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgebraGeometry;
using CSharpLogic;

namespace GeometryLogicInference
{
    public class GeometryInference
    {
        #region Singleton, Constructor and Properties

        private static GeometryInference _instance;

        private GeometryInference()
        {
            _relationGraph = new RelationGraph();
            _selectedNode = new List<GraphNode>();
            _cache = new ObservableCollection<KeyValuePair<object, object>>();
        }

        public static GeometryInference Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GeometryInference();
                }
                return _instance;
            }
        }

        private List<GraphNode> _selectedNode;

        private RelationGraph _relationGraph;
        public RelationGraph CurrentGraph
        {
            get
            {
                if (_selectedNode.Count == 0)
                {
                    return _relationGraph;
                }
                else
                {
                    return new RelationGraph(_selectedNode);
                }
            }
        }
        
        /// <summary>
        /// Key: Pattern Match Result
        /// Value: Predict Pattern Match Result
        /// </summary>
        private ObservableCollection<KeyValuePair<object, object>> _cache;

        public ObservableCollection<KeyValuePair<object, object>> Cache
        {
            get { return _cache; }
        }

        #endregion

        #region API to communicate with relation graph

        /// <summary>
        /// Take charge of input uncertainty 
        /// </summary>
        /// <param name="obj">Shape, Goal, Label, or list of objects </param>
        /// <returns></returns>
        public object Add(object obj)
        {
            object returnObj = null;

            #region Deterministic input

            var shape = obj as Shape;
            if (shape != null)
            {
                Add(shape);
                ReEvalCach(); //re-check uncertainty
                returnObj = shape;
            }

            var goal = obj as Goal;
            if (goal != null)
            {
                Add(goal);
                ReEvalCach();
                returnObj =  goal;
            }

            #endregion

            #region Non-Deterministic input 
            //TODO Bayesian Inference upon relation graph

            var label = obj as string;
            if (label != null)
            {
                returnObj = EvalLabel(label);
            }

            var lst = obj as List<object>;
            if (lst != null)
            {
                returnObj =  EvalListObjects(lst);
            }

            #endregion

            CheckUncertaintyOnGraph(obj); // Uncertainty Analysis
            
            return returnObj;
        }

        public void Delete(object obj)
        {
            #region Determinstic input

            var shape = obj as Shape;
            if (shape != null)
            {
                Delete(shape);
                ReEvalCach();
            }

            var goal = obj as Goal;
            if (goal != null)
            {
                Delete(goal);
                ReEvalCach();
            }

            #endregion

            #region Non-Deterministic input
            //TODO Bayesian Inference upon relation graph

            var label = obj as string;
            if (label != null)
            {
                UnEvalLabel(label);
            }

            var lst = obj as List<object>;
            if (lst != null)
            {
                UnEvalListObjects(lst);
            }

            #endregion

            CheckUncertaintyOnGraph(obj); // Uncertainty Analysis
        }

        #endregion

        #region Non-Deterministic Label and List<object>

        #region Label Analysis

        private object EvalLabel(string label)
        {
            object obj1;
            bool result = _relationGraph.InferVariable(label, out obj1);
            if (result)
            {
                var unifiedShape = obj1 as Shape;
                if (unifiedShape != null)
                {
                    //object value = Add(unifiedShape);
                    GraphNode gn = _relationGraph.AddNode(unifiedShape);
                    _cache.Add(new KeyValuePair<object, object>(label, gn));
                    return unifiedShape;
                }
            }
            else
            {
                var types = obj1 as List<ShapeType>;
                //User Feedback Required
                if (types != null)
                {
                    _cache.Add(new KeyValuePair<object, object>(label, types));
                    return types;
                }
            }
            _cache.Add(new KeyValuePair<object, object>(label, null));
            return label;
        }

        private void UnEvalLabel(string label)
        {
            KeyValuePair<object, object>? resultPair = null;
            foreach (KeyValuePair<object, object> pair in _cache)
            {
                var tempLabel = pair.Key as string;
                if (tempLabel != null && tempLabel.Equals(label))
                {
                    resultPair = pair;
                }
            }

            if (resultPair != null)
            {
                object obj = resultPair.Value.Value;
                var shapeNode = obj as ShapeNode;
                if (shapeNode != null)
                {
                    _relationGraph.DeleteShapeNode(shapeNode.Shape);
                }
                var goalNode = obj as GoalNode;
                if (goalNode != null)
                {
                    _relationGraph.DeleteGoalNode(goalNode.Goal);
                }
                _cache.Remove(resultPair.Value);
            }
        }

        private void ReEvalLabel()
        {
            string label = null;
            foreach (KeyValuePair<object, object> pair in _cache.ToList())
            {
                var tempLabel = pair.Key as string;
                if (tempLabel != null)
                {
                    label = tempLabel;
                    var sn = pair.Value as ShapeNode;
                    if (sn != null)
                    {
                        _relationGraph.DeleteShapeNode(sn.Shape);
                    }

                    var gn = pair.Value as GoalNode;
                    if (gn != null)
                    {
                        _relationGraph.DeleteGoalNode(gn.Goal);
                    }

                    _cache.Remove(pair);
                }
            }

            if (label != null)
            {
                EvalLabel(label);
            }
        }

        #endregion

        #region List of Object (Goal and Shape)

        private object EvalListObjects(List<object> lst)
        {
            object obj1;
            object inferredObj = null;
            foreach (object objTemp in lst)
            {
                bool exist = _relationGraph.InferVariable(objTemp, out obj1);
                if (exist) inferredObj = objTemp;
            }

            if (inferredObj != null) // deterministic
            {
                if (inferredObj is Goal || inferredObj is Shape)
                {
                    //object result = Add(inferredObj);
                    GraphNode gn = _relationGraph.AddNode(inferredObj);
                    _cache.Add(new KeyValuePair<object, object>(lst, gn));
                    return inferredObj;
                }
            }
            else //non-deterministic
            {
                //Heuristic: prefer shape instead of goal
                foreach (object currObj in lst)
                {
                    if (currObj is Shape)
                    {
                        //object result = Add(currObj);
                        GraphNode result = _relationGraph.AddNode(currObj);
                        _cache.Add(new KeyValuePair<object, object>(lst, result));
                        return currObj;
                    }
                }
            }

            return lst;
        }

        private void UnEvalListObjects(List<object> lst)
        {
            KeyValuePair<object, object>? resultPair = null;
            foreach (KeyValuePair<object, object> pair in _cache)
            {
                var tempLst = pair.Key as List<object>;
                if (tempLst != null && tempLst.Equals(lst))
                {
                    resultPair = pair;
                }
            }

            if (resultPair != null)
            {
                object obj = resultPair.Value.Value;
                var shapeNode = obj as ShapeNode;
                if (shapeNode != null)
                {
                    _relationGraph.DeleteShapeNode(shapeNode.Shape);
                }
                var goalNode = obj as GoalNode;
                if (goalNode != null)
                {
                    _relationGraph.DeleteGoalNode(goalNode.Goal);
                }
                _cache.Remove(resultPair.Value);
            }
        }

        #endregion

        private void ReEvalCach()
        {
            ReEvalLabel();
        }

        /// <summary>
        /// Case by case heuristics
        /// </summary>
        /// <param name="obj"></param>
        private void CheckUncertaintyOnGraph(object obj)
        {
            var lst = RetrieveMultiObjectsPairs();
            if (lst.Count == 0) return;

            var shape = obj as Shape;
            if (shape == null) return;

            foreach (KeyValuePair<object, object> pair in lst.ToList())
            {
                var alterLst = pair.Key as List<object>;
                Debug.Assert(alterLst != null);

                var shapeNode = pair.Value as ShapeNode;
                if (shapeNode != null)
                {
                    _relationGraph.DeleteShapeNode(shapeNode.Shape);
                }
                var goalNode = pair.Value as GoalNode;
                if (goalNode != null)
                {
                    _relationGraph.DeleteGoalNode(goalNode.Goal);
                }

                _cache.Remove(pair);
                Add(alterLst);
            }
        }

        #endregion

        #region Deterministic Shape and Goal

        private object Add(Shape shape)
        {
            GraphNode gn = _relationGraph.AddNode(shape);
            _cache.Add(new KeyValuePair<object, object>(shape, gn));
            return shape;
        }

        private void Delete(Shape shape)
        {
            _relationGraph.DeleteShapeNode(shape);
            foreach (KeyValuePair<object, object> pair in _cache.ToList())
            {
                if (pair.Key.Equals(shape))
                {
                    _cache.Remove(pair);
                }
            }
        }

        private object Add(Goal goal)
        {
            GraphNode gn = _relationGraph.AddNode(goal);
            _cache.Add(new KeyValuePair<object, object>(goal, gn));
            return goal;
        }

        private void Delete(Goal goal)
        {
            _relationGraph.DeleteGoalNode(goal);
            foreach (KeyValuePair<object, object> pair in _cache.ToList())
            {
                if (pair.Key.Equals(goal))
                {
                    _cache.Remove(pair);
                }
            }
        }

        #endregion

        #region Utilities

        public object SearchCacheValue(object obj)
        {
            foreach (KeyValuePair<object, object> pair in Cache)
            {
                if (pair.Key.Equals(obj))
                {
                    return pair.Value;
                }
            }
            return null;
        }

        private List<KeyValuePair<object, object>> RetrieveMultiObjectsPairs()
        {
            var lst = new List<KeyValuePair<object, object>>();
            foreach (KeyValuePair<object, object> pair in _cache)
            {
                //var str = pair.Key as String;
                var lstTemp = pair.Key as List<object>;
                //if (str != null || lstTemp != null)
                if(lstTemp != null)
                {
                    lst.Add(pair);
                }
            }
            return lst;
        }

        #endregion

        #region Graph Node Selections

        public void SelectNode(GraphNode gn)
        {
            if(!_relationGraph.Nodes.Contains(gn)) 
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
        }

        #endregion
    }
}