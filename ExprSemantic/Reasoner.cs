﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using AlgebraGeometry;
using AlgebraGeometry.Expr;
using CSharpLogic;
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
        }

        #endregion

        #region Properties

        private ObservableCollection<KeyValuePair<object, object>> _cache;

        private Dictionary<object, object> _preCache; 

        public ActionManager ActionManager { get; set; }

        #endregion

        #region Evaluation Public Interface

        /// <summary>
        /// Sketch Input
        /// </summary>
        /// <param name="expr"></param>
        public IKnowledge Load(Expr expr)
        {
            object rTemp = ExprVisitor.Instance.Match(expr);
            if (rTemp == null) return null;

            object result;
            if (rTemp is ShapeSymbol)
            {
                var ss = rTemp as ShapeSymbol;
                result = new AGShapeExpr(expr, ss);
            }
            else if (rTemp is EqGoal)
            {
                var eg = rTemp as EqGoal;
                result = new AGPropertyExpr(expr, eg);
            }
            else
            {
                return null;
            }

            //Search Algorithm
            EvalPropogate(result);

            var pair = new KeyValuePair<object, object>(expr, result);
            _cache.Add(pair);
            var k = result as IKnowledge;
            return k ?? null;
        }

        public object Load(string fact)
        {
            starPadSDK.MathExpr.Expr expr = Text.Convert(fact);

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

        public void Load(IEnumerable<string> facts)
        {
            foreach (string f in facts)
            {
                Load(f);
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
            object fact = SearchFact(key);
            if (fact != null)
            {
                var temp = (KeyValuePair<object, object>) fact;
                UndoEvalPropogate(temp.Value);
                _cache.Remove(temp);
            }
        }

        #endregion

    }
}