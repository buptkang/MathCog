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
            ActionManager = new ActionManager();
            _cache = new ObservableCollection<KeyValuePair<object, object>>();
            _preCache = new Dictionary<object, object>();
            _globalVarCache = new List<Var>();
        }

        #endregion

        #region Properties

        private ObservableCollection<KeyValuePair<object, object>> _cache;
        private Dictionary<object, object> _preCache;
        private List<Var> _globalVarCache;

        public ActionManager ActionManager { get; set; }

        #endregion

        #region Evaluation Public Interface

        /// <summary>
        /// Sketch Input
        /// </summary>
        /// <param name="expr"></param>
        public object Load(Expr expr)
        {
            var rTemp = ExprVisitor.Instance.Match(expr) as Dictionary<PatternEnum, object>;
            Debug.Assert(rTemp != null);

            var lst = new List<object>();
            foreach (KeyValuePair<PatternEnum, object> pair in rTemp)
            {
                object result;
                switch (pair.Key)
                {
                    case PatternEnum.Line:
                        var ls = pair.Value as ShapeSymbol;
                        result = new AGShapeExpr(expr, ls);
                        break;
                    case PatternEnum.Point:
                        var ps = pair.Value as ShapeSymbol;
                        result = new AGShapeExpr(expr, ps);
                        break;
                    case PatternEnum.Goal:
                        var eg = pair.Value as EqGoal;
                        result = new AGPropertyExpr(expr, eg);
                        break;
                    default:
                        continue;
                }
                lst.Add(result);
            }

            if (lst.Count == 0) return null;

            IKnowledge value;
            if (lst.Count == 1) value = lst[0] as IKnowledge;
            else value = SearchBySemantic(lst);

            //Cache all Var from current IKnowledge
            //ExtractVariables(value);

            //Search Algorithm
            EvalPropogate(value);
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