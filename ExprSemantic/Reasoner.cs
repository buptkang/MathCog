using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using AlgebraGeometry;
using AlgebraGeometry.Expr;
using CSharpLogic;
using NUnit.Framework;
using starPadSDK.MathExpr;
using Text = starPadSDK.MathExpr.Text;
using GuiLabs.Undo;

namespace ExprSemantic
{
    public partial class Reasoner
    {
        #region Instance

        private static Reasoner _instance;

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
            _cache = new ObservableCollection<KeyValuePair<object, object>>();
            _preCache = new Dictionary<object, object>();
            _logicGraph = new RelationGraph();
        }

        #endregion

        #region Properties

        private RelationGraph _logicGraph;
        private ObservableCollection<KeyValuePair<object, object>> _cache;
        private Dictionary<object, object> _preCache;
        private List<Var> _globalVarCache;

        #endregion

        #region Evaluation Public Interface

        /// <summary>
        /// Sketch Input
        /// </summary>
        /// <param name="expr"></param>
        public object Load(Expr expr)
        {
            var rTemp = ExprVisitor.Instance.Match(expr);
            Debug.Assert(rTemp != null);
            IKnowledge value = null;
            value = EvalExprPatterns(expr, rTemp);
            _cache.Add(new KeyValuePair<object, object>(expr, value));
            return value ?? null;
        }

        public object Load(string fact)
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
                UndoEvalPropogate(fact[0].Value);
                _cache.Remove(fact[0]);
            }
        }

        #endregion
    }
}