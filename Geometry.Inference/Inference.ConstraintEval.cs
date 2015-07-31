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
    /// <summary>
    /// Non-Deterministic Constraint Solving
    /// </summary>
    public partial class GeometryInference
    {
        private object Eval(ShapeType? st)
        {
            Debug.Assert(st != null);
            object obj;
            bool result = _relationGraph.Infer(null, st, out obj);
            if (result)
            {
                var dict = obj as Dictionary<Tuple<GraphNode, GraphNode>, object>;
                Debug.Assert(dict != null);
                var list = new List<object>();
                foreach (KeyValuePair<Tuple<GraphNode, GraphNode>, object> pair in dict)
                {
                    var newObj = pair.Value;
                    GraphNode gn = _relationGraph.AddNode(newObj, pair.Key.Item1, pair.Key.Item2);
                    _cache.Add(new KeyValuePair<object, object>(newObj, gn));
                    list.Add(pair.Value);
                }
                return list.Count == 1 ? list[0] : list;
            }
            return null;
        }

        private object Eval(string label, ShapeType? st = null)
        {
            Delete(label);
            object obj1;
            bool result = _relationGraph.Infer(label, st, out obj1);
            if (result)
            {
                var dict = obj1 as Dictionary<Tuple<GraphNode, GraphNode>, object>;
                Debug.Assert(dict != null);
                var list = new List<object>();
                foreach (KeyValuePair<Tuple<GraphNode, GraphNode>, object> pair in dict)
                {
                    var newObj = pair.Value;
                    GraphNode gn = _relationGraph.AddNode(newObj, pair.Key.Item1, pair.Key.Item2);
                    _cache.Add(new KeyValuePair<object, object>(label, gn));
                    list.Add(pair.Value);
                }
                return list.Count == 1 ? list[0] : list;
            }
            else
            {
                var types = obj1 as List<ShapeType>;
                //User Feedback Required
                //if (types != null) throw new Exception("Cannot reach here");
                _cache.Add(new KeyValuePair<object, object>(label, null));
                return types;
                //return label;
            }
        }

        private void UnEval(string label)
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

        private void ReEval()
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
                Eval(label);
            }
        }

        #region List of Object (Goal and Shape)

        private object EvalListObjects(List<object> lst)
        {
            object obj1;
            object inferredObj = null;
            foreach (object objTemp in lst)
            {
                bool exist = _relationGraph.Infer(objTemp, null, out obj1);
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

    }
}
