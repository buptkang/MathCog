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
    public partial class GeometryInference
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

        //  TODO Bayesian Inference upon relation graph
        /// <summary>
        /// Constraint Solving Program
        /// overfitting and underfitting issues
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="constraint"></param>
        /// <param name="st"></param>
        /// <param name="isAdd"></param>
        /// <returns></returns>
        private object CSP(object constraint, 
                           ShapeType? st = null, bool isAdd = true)
        {
            if (isAdd)
            {
                if (constraint == null) return Eval(st);
                object returnObj = null;

                var label = constraint as string;
                if (label != null)
                {
                    returnObj = Eval(label, st);
                }

                /*
                var lst = constraint as List<object>;
                if (lst != null)
                {
                    returnObj = EvalListObjects(lst);
                }*/

                //CheckUncertaintyOnGraph(constraint); // Uncertainty Analysis
                return returnObj;
            }
            else //delete
            {               
                var label = constraint as string;
                if (label != null)
                {
                    UnEval(label);
                }

                /*  var lst = constraint as List<object>;
                if (lst != null)
                {
                    UnEvalListObjects(lst);
                }

                CheckUncertaintyOnGraph(obj); // Uncertainty Analysis*/
                return null;
            }
        }

        /// <summary>
        /// Take charge of input uncertainty 
        /// </summary>
        /// <param name="obj">Shape, Goal, Label, or list of objects </param>
        /// <param name="st"></param>
        /// <returns></returns>
        public object Add(object obj, ShapeType? st = null)
        {
            #region Deterministic Input
            var shape = obj as Shape;
            if (shape != null)
            {
                Add(shape);
                ReEval(); //re-check uncertainty
                return shape;
            }
            var goal = obj as EqGoal;
            if (goal != null)
            {
                Add(goal);
                ReEval();
                return goal;
            }
            #endregion

            //Non-Deterministic Input (Constraint Solving)
            return CSP(obj, st);
        }

        public void Delete(object obj)
        {
            #region Determinstic input

            var shape = obj as Shape;
            if (shape != null)
            {
                Delete(shape);
                ReEval();
            }

            var goal = obj as Goal;
            if (goal != null)
            {
                Delete(goal);
                ReEval();
            }

            #endregion

            CSP(obj, null, false);
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
    }
}