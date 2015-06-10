using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpLogic;
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

        private static Reasoner _memory = Reasoner.Instance;

        private Interpreter()
        {
            ActionManager = new ActionManager();
            _queryCache = new ObservableCollection<KeyValuePair<object, object>>();
            _preQueryCache = new Dictionary<object, object>();
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
                _memory.Answer(kv.Value, out queryResult);

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
            if (result == null) return _memory.Load(str);
            else return result;
        }

        public object Load(Expr expr)
        {
            object result = LoadQuery(expr);
            if (result == null) return _memory.Load(expr);
            else return result;
        }

        public void UnLoad(string str)
        {
            bool result = UnLoadQuery(str);
            if (!result)
            {
                _memory.Unload(str);
            }
        }

        public void UnLoad(Expr expr)
        {
            bool result = UnLoadQuery(expr);
            if (!result)
            {
                _memory.Unload(expr);
            }
        } 

        #endregion 
    }
}
