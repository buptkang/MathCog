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

        public object Load(object obj, ShapeType? st = null, bool userInput = false)
        {
            var str = obj as string;     // text input
            if (str != null) return Load(str, st, userInput);
            var expr = obj as Expr;      // sketch input
            if (expr != null) return Load(expr, st, userInput);
            return null;
        }

        private object Load(Expr expr, ShapeType? st = null, bool userInput = false)
        {
            var rTemp = ExprVisitor.Instance.Match(expr); //input patter match
            Debug.Assert(rTemp != null);
            object output;

            EvalExprPatterns(expr, rTemp, st, out output, userInput);
            var iKnowledge = output as IKnowledge;
            if (iKnowledge != null)
            {
                _cache.Add(new KeyValuePair<object, object>(expr, iKnowledge));
            }
            return output;
        }

        private object Load(string fact, ShapeType? st = null, bool userInput = false)
        {
            Expr expr = Text.Convert(fact);
            object result = Load(expr, st, userInput);
            if (result != null)
            {
                _preCache.Add(fact, expr);
                return result;
            }
            return null;
        }

        public bool Unload(string fact, out bool userInput)
        {
            userInput = false;
            if (!_preCache.ContainsKey(fact)) return false;
            var expr = _preCache[fact] as Expr;
            Debug.Assert(expr != null);

            bool result = Unload(expr, out userInput);
            _preCache.Remove(fact);
            return result;
        }

        public bool Unload(Expr key, out bool userInput)
        {
            userInput = false;
            List<KeyValuePair<object, object>> fact
                = _cache.Where(x => x.Key.Equals(key)).ToList();
            if (fact.Count != 0)
            {
                Debug.Assert(fact.Count == 1);
                bool result = UnEvalExprPatterns(fact[0].Value, out userInput);
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