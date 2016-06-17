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
namespace MathCog
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using AlgebraGeometry;
    using CSharpLogic;
    using ExprPatternMatch;
    using starPadSDK.MathExpr;
    using Text = starPadSDK.MathExpr.Text;

    public partial class Reasoner
    {
        #region Properties and Constructor

        //objective model
        public RelationGraph RelationGraph { get; set; }

        public static Reasoner _instance;

        public static Reasoner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Reasoner();
                }
                return _instance;
            }
        }

        public Reasoner()
        {
            RelationGraph = new RelationGraph();
            _cache = new ObservableCollection<KeyValuePair<object, object>>();
            _preCache = new Dictionary<object, object>();
        }

        /// <summary>
        /// Key: Expr; Value: IKnowledge
        /// </summary>
        private ObservableCollection<KeyValuePair<object, object>> _cache;

        public ObservableCollection<KeyValuePair<object, object>> Cache
        {
            get { return _cache; }
            set { _cache = value;}
        }




        /// <summary>
        /// Key: String; Value: Expr
        /// </summary>
        private Dictionary<object, object> _preCache;

        #endregion

        #region Input communication with lower reasoning engine

        public object Load(object obj, ShapeType? st = null, 
            bool tutorMode = false, bool algebraSide = true)
        {
            var str = obj as string;     // text input
            if (str != null) return Load(str, st, tutorMode, algebraSide);
            var expr = obj as Expr;      // sketch input
            if (expr != null) return Load(expr, st, tutorMode, algebraSide);
            var ss = obj as ShapeSymbol;
            if (ss != null) return Load(ss, tutorMode);
            return null;
        }

        //Geometry Side Input Variation
        private object Load(ShapeSymbol rTemp, bool tutorMode = false)
        {
            Expr expr = ExprG.Generate(rTemp);
            object output;
            EvalExprPatterns(expr, rTemp, null, out output, tutorMode);
            var iKnowledge = output as IKnowledge;
            if (iKnowledge != null && !tutorMode)
            {
                _cache.Add(new KeyValuePair<object, object>(rTemp, iKnowledge));
            }
            return output;
        }

        //Algebra Side Input Format
        private object Load(Expr expr, ShapeType? st = null, 
            bool tutorMode = false, bool algebraSide = true)
        {
            if (!tutorMode)
            {
                //pre-fetch
                foreach (KeyValuePair<object, object> pair in _cache)
                {
                    if (pair.Key.Equals(expr))
                    {
                        return pair.Value;
                    }
                }                
            }

            if (tutorMode && !algebraSide)
            {
                //pre-fetch
                foreach (KeyValuePair<object, object> pair in _cache)
                {
                    if (pair.Key.Equals(expr))
                    {
                        return pair.Value;
                    }
                }
            }

            object rTemp = null;
            //TODO, reasoning
/*
            if (tutorMode)
            {
                rTemp = ExprVisitor.Instance.UserMatch(expr);
            }
            else
            {
                rTemp = ExprVisitor.Instance.Match(expr);
            }
*/

            rTemp = ExprVisitor.Instance.Match(expr);

             //input patter match
            Debug.Assert(rTemp != null);
            object output;

            EvalExprPatterns(expr, rTemp, st, out output, tutorMode);
            var iKnowledge = output as IKnowledge;
            if (iKnowledge != null)
            {
                _cache.Add(new KeyValuePair<object, object>(expr, iKnowledge));
            }
            return output;
        }

        private object Load(string fact, ShapeType? st = null, 
            bool tutorMode = false, bool algebraSide = true)
        {
            if (!tutorMode)
            {
                //pre-fetch
                if (_preCache.ContainsKey(fact))
                {
                    var cachedExpr = _preCache[fact] as Expr;
                    Debug.Assert(cachedExpr != null);
                    foreach (KeyValuePair<object, object> pair in _cache)
                    {
                        if (pair.Key.Equals(cachedExpr))
                        {
                            return pair.Value;
                        }
                    }
                    throw new Exception("Cannot reach here!");
                }
            }
            if (tutorMode && !algebraSide)
            {
                //pre-fetch
                if (_preCache.ContainsKey(fact))
                {
                    var cachedExpr = _preCache[fact] as Expr;
                    Debug.Assert(cachedExpr != null);
                    foreach (KeyValuePair<object, object> pair in _cache)
                    {
                        if (pair.Key.Equals(cachedExpr))
                        {
                            return pair.Value;
                        }
                    }
                    throw new Exception("Cannot reach here!");
                }
            }
            Expr expr = Text.Convert(fact);
            object result = Load(expr, st, tutorMode);
            if (result == null) return null;
            if (!tutorMode)
            {
                _preCache.Add(fact, expr);
            }
            return result;
        }

        public bool Unload(object obj)
        {
            var strObj = obj as string;
            var exprObj = obj as Expr;
            var shapeObj = obj as ShapeSymbol;
            if (strObj != null)
            {
                return Unload(strObj);
            }
            if (exprObj != null)
            {
                return Unload(exprObj);
            }
            if (shapeObj != null)
            {
                UnLoad(shapeObj);
            }
            throw new Exception("Cannot reach here!!!");
        }

        //Geometry Side Input Variation
        private object UnLoad(ShapeSymbol rTemp)
        {
            List<KeyValuePair<object, object>> fact
                = _cache.Where(x => x.Key.Equals(rTemp)).ToList();
            if (fact.Count != 0)
            {
                Debug.Assert(fact.Count == 1);
                bool result = UnEvalExprPatterns(fact[0].Value);
                _cache.Remove(fact[0]);
                return result;
            }
            return false;
        }

        private bool Unload(string fact)
        {
            if (!_preCache.ContainsKey(fact)) return false;
            var expr = _preCache[fact] as Expr;
            Debug.Assert(expr != null);

            bool result = Unload(expr);
            _preCache.Remove(fact);
            return result;
        }

        private bool Unload(Expr key)
        {
            List<KeyValuePair<object, object>> fact
                = _cache.Where(x => x.Key.Equals(key)).ToList();
            if (fact.Count != 0)
            {
                Debug.Assert(fact.Count == 1);
                bool result = UnEvalExprPatterns(fact[0].Value);
                _cache.Remove(fact[0]);
                return result;
            }
            return false;
        }

        public void Reset()
        {
            RelationGraph = new RelationGraph();
            _cache = new ObservableCollection<KeyValuePair<object, object>>();
            _preCache = new Dictionary<object, object>();
        }

        #endregion
    }
}