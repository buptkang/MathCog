using AlgebraGeometry.Expr;
using GeometryLogicInference;
using starPadSDK.MathExpr;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Text = starPadSDK.MathExpr.Text;

namespace ExprSemantic
{
    public partial class Reasoner
    {
        #region Properties and Constructor

        public Reasoner()
        {
            _cache = new ObservableCollection<KeyValuePair<object, object>>();
            _preCache = new Dictionary<object, object>();

            GeometryInference.Instance.Cache.CollectionChanged += Cache_CollectionChanged;
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

        #region Feedback communication with lower reasoning engine

        /// <summary>
        /// GraphNode Update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Cache_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
            }
        }

        #endregion

        #region Input communication with lower reasoning engine

        public object Load(object obj)
        {
            var str = obj as string;
            if (str != null) return Load(str);
            var expr = obj as Expr;
            if (expr != null) return Load(expr);
            return null;
        }

        /// <summary>
        /// Sketch Input
        /// </summary>
        /// <param name="expr"></param>
        private object Load(Expr expr)
        {
            var rTemp = ExprVisitor.Instance.Match(expr); //input patter match
            Debug.Assert(rTemp != null);
            object output;
            bool result = EvalExprPatterns(expr, rTemp, out output);
            if (result)
            {
                var iKnowledge = output as IKnowledge;
                if (iKnowledge != null)
                {
                    _cache.Add(new KeyValuePair<object, object>(expr, iKnowledge)); 
                }
            }
            return output;
        }

        private object Load(string fact)
        {
            Expr expr = Text.Convert(fact);
            object result = Load(expr);
            if (result != null)
            {
                _preCache.Add(fact, expr);
                return result;
            }
            else
            {
                return null;
            }
        }

        public void Unload(string fact)
        {
            if (_preCache.ContainsKey(fact))
            {
                var expr = _preCache[fact] as Expr;
                if (expr != null)
                {
                    Unload(expr);
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

        #endregion
    }
}