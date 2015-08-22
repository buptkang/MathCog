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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using AlgebraGeometry;
    using AlgebraGeometry.Expr;
    using CSharpLogic;
    using ExprPatternMatch;
    using starPadSDK.MathExpr;
    using Text = starPadSDK.MathExpr.Text;

    public partial class Reasoner
    {
        #region Properties and Constructor

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

        private Reasoner()
        {
            RelationGraph = new RelationGraph();
            _cache = new ObservableCollection<KeyValuePair<object, object>>();
            _preCache = new Dictionary<object, object>();
            _tutorSession = false;
        }

        /// <summary>
        /// Key: Expr; Value: IKnowledge
        /// </summary>
        private ObservableCollection<KeyValuePair<object, object>> _cache;
        /// <summary>
        /// Key: String; Value: Expr
        /// </summary>
        private Dictionary<object, object> _preCache;

        private bool _tutorSession;

        public bool TutorSession
        {
            get { return _tutorSession; }
            set { _tutorSession = value; }
        }

        #endregion

        #region Input communication with lower reasoning engine

        public object Load(object obj, ShapeType? st = null, bool tutorSession = false)
        {
            _tutorSession = tutorSession;
            var str = obj as string;     // text input
            if (str != null) return Load(str, st);
            var expr = obj as Expr;      // sketch input
            if (expr != null) return Load(expr, st);
            return null;
        }

        private object Load(Expr expr, ShapeType? st = null)
        {
            var rTemp = ExprVisitor.Instance.Match(expr); //input patter match
            Debug.Assert(rTemp != null);
            object output;

            EvalExprPatterns(expr, rTemp, st, out output);
            var iKnowledge = output as IKnowledge;
            if (iKnowledge != null)
            {
                _cache.Add(new KeyValuePair<object, object>(expr, iKnowledge));
            }
            return output;
        }

        private object Load(string fact, ShapeType? st = null)
        {
            Expr expr = Text.Convert(fact);
            object result = Load(expr, st);
            if (result != null)
            {
                _preCache.Add(fact, expr);
                return result;
            }
            return null;
        }

        public void Unload(string fact)
        {
            if (_preCache.ContainsKey(fact))
            {
                var expr = _preCache[fact] as Expr;
                if (expr != null)
                {
                    Unload(expr);
                    _preCache.Remove(fact);
                }
            }
        }

        public void Unload(Expr key)
        {
            List<KeyValuePair<object, object>> fact
                = _cache.Where(x => x.Key.Equals(key)).ToList();
            if (fact.Count != 0)
            {
                Debug.Assert(fact.Count == 1);
                UnEvalExprPatterns(fact[0].Value);
                _cache.Remove(fact[0]);
            }
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