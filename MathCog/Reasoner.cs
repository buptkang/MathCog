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
        /// <summary>
        /// Key: String; Value: Expr
        /// </summary>
        private Dictionary<object, object> _preCache;

        #endregion

        #region Input communication with lower reasoning engine

        public object Load(object obj, ShapeType? st = null, bool tutorMode = false)
        {
            var str = obj as string;     // text input
            if (str != null) return Load(str, st, tutorMode);
            var expr = obj as Expr;      // sketch input
            if (expr != null) return Load(expr, st, tutorMode);
            return null;
        }

        private object Load(Expr expr, ShapeType? st = null, bool tutorMode = false)
        {
            //pre-fetch
            foreach (KeyValuePair<object, object> pair in _cache)
            {
                if (pair.Key.Equals(expr))
                {
                    return pair.Value;
                }
            }
            var rTemp = ExprVisitor.Instance.Match(expr, tutorMode); //input patter match
            Debug.Assert(rTemp != null);
            object output;

            EvalExprPatterns(expr, rTemp, st, out output, tutorMode);
            var iKnowledge = output as IKnowledge;
            if (iKnowledge != null && !tutorMode)
            {
                _cache.Add(new KeyValuePair<object, object>(expr, iKnowledge));
            }
            return output;
        }

        private object Load(string fact, ShapeType? st = null, bool tutorMode = false)
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
            if (strObj != null)
            {
                return Unload(strObj);
            }

            if (exprObj != null)
            {
                return Unload(exprObj);
            }

            throw new Exception("Cannot reach here!!!");
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

        #region Query API

        public object SurfaceValidate(Expr expr)
        {
            var tempObj = ExprVisitor.Instance.Match(expr, false); //input patter match

            return tempObj;
        }

        /// <summary>
        /// In TutorMode, verify user's own input state
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="?"></param>
        /// <returns>List<TraceStep></returns>
        public object Validate(object obj, out object objOutput)
        {
            var expr = obj as Expr;
            var str = obj as string;     // text input
            if (str != null)
            {
                expr = Text.Convert(str);
            }
            Debug.Assert(expr != null);

            var rTemp = ExprVisitor.Instance.Match(expr, true); //input patter match
            Debug.Assert(rTemp != null);

            rTemp = ExprVisitor.Instance.Transform(rTemp);
            object output;
            InternalValidate(expr, rTemp, out output);
            objOutput = rTemp;
            return output;
        }
        #endregion
    }
}