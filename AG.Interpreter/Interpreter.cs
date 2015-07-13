using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgebraGeometry;
using CSharpLogic;
using ExprPatternMatch;
using ExprSemantic;
using GuiLabs.Undo;
using starPadSDK.MathExpr;
using Text = starPadSDK.MathExpr.Text;

namespace AG.Interpreter
{
    public partial class Interpreter : IInterpreter
    {
        #region Singleton, Constructor and Properties

        public static Interpreter _instance;

        public static Interpreter Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Interpreter();
                }
                return _instance;
            }
        }

        public ActionManager ActionManager { get; set; }

        private Reasoner _reasoner;

        public Reasoner Reasoner
        {
            get { return _reasoner; }
            set { _reasoner = value;}
        }

        private Interpreter()
        {
            ActionManager = new ActionManager();
            _queryCache = new ObservableCollection<KeyValuePair<object, object>>();
            _preQueryCache = new Dictionary<object, object>();

            _reasoner = new Reasoner();
        }

        //Key: Expr; Value: AGQueryExpr
        private ObservableCollection<KeyValuePair<object, object>> _queryCache;
        private Dictionary<object, object> _preQueryCache; 

        #endregion

        #region Load and Unload Query

        public object LoadQuery(Expr expr)
        {
            object rTemp = ExprVisitor.Instance.MatchQuery(expr);
            if (rTemp == null) return null;

            var kv = (KeyValuePair<string, object>)rTemp;

            if ("Label".Equals(kv.Key))
            {
                Debug.Assert(kv.Value is Var);
                object queryResult;
                _reasoner.Answer(kv.Value, out queryResult);

                var queryResults = queryResult as List<object>;
                if (queryResults != null)
                {
                    if (queryResults.Count == 1) // deterministic
                    {
                        var propResult = queryResults[0] as PropertyQueryResult;
                        if (propResult != null)
                        {
                            expr = propResult.Answer;
                        }
                        //TODO
                    }
                }

                object obj = Eval(expr, queryResult);

                var newKv = new KeyValuePair<object, object>(expr, obj);
                _queryCache.Add(newKv);

                return obj;
            }
            else if ("Term".Equals(kv.Key))
            {
                Debug.Assert(kv.Value is Term);
                //TODO
                throw new Exception("TODO");
            }

            throw new Exception("TODO");
        }

        public object LoadQuery(string fact)
        {
            Expr expr = Text.Convert(fact);
            object obj = LoadQuery(expr);
            if (obj == null) return null;
            _preQueryCache.Add(fact, expr);
            return obj;
        }

        public bool UnLoadQuery(string obj)
        {
            if (_preQueryCache.ContainsKey(obj))
            {
                var expr = _preQueryCache[obj] as Expr;
                if (expr != null)
                {
                    UnLoadQuery(expr);
                }
                return true;
            }
            return false;
        }

        public bool UnLoadQuery(Expr expr)
        {
            object query = SearchQuery(expr);
            if (query != null)
            {
                var temp = (KeyValuePair<object, object>)query;
                //TODO
                _queryCache.Remove(temp);
                return true;
            }
            return false;
        }

        #endregion

        #region Load and Unload

        public object Load(string str)
        {
            object result = LoadQuery(str);
            if (result == null) return _reasoner.Load(str);
            return result;
        }

        public object Load(Expr expr)
        {
            object result = LoadQuery(expr);
            if (result == null) return _reasoner.Load(expr);
            return result;
        }

        public object Load(Expr expr, ShapeType st)
        {
            object result = LoadQuery(expr);
            if (result == null) return _reasoner.Load(expr, st);
            return result;
        }

        public void UnLoad(string str)
        {
            bool result = UnLoadQuery(str);
            if (!result)
            {
                _reasoner.Unload(str);
            }
        }

        public void UnLoad(Expr expr)
        {
            bool result = UnLoadQuery(expr);
            if (!result)
            {
                _reasoner.Unload(expr);
            }
        } 

        #endregion 
    }
}
