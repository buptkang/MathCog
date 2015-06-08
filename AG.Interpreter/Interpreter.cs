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

        private ObservableCollection<KeyValuePair<object, object>> _queryCache;
        private Dictionary<object, object> _preQueryCache; 

        #endregion

        #region Load and Unload Query

        public bool LoadQuery(Expr expr)
        {
            object rTemp = ExprVisitor.Instance.MatchQuery(expr);
            if (rTemp == null) return false;

            var kv = (KeyValuePair<string, object>)rTemp;

            if ("Label".Equals(kv.Key))
            {
                Debug.Assert(kv.Value is Var);
                object queryResult;
                _memory.Answer(kv.Value, out queryResult);
                var newKv = new KeyValuePair<object, object>(expr, queryResult);
                _queryCache.Add(newKv);
                return true;
            }
            else if ("Term".Equals(kv.Key))
            {
                Debug.Assert(kv.Value is Term);
                //TODO
                throw new Exception("TODO");
            }

            throw new Exception("TODO");
            return true;
        }

        public bool LoadQuery(string fact)
        {
            starPadSDK.MathExpr.Expr expr = Text.Convert(fact);
            bool result = LoadQuery(expr);
            if (result)
            {
                _preQueryCache.Add(fact, expr);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UnLoadQuery(object obj)
        {
            if (_preQueryCache.ContainsKey(obj))
            {
                var expr = _preQueryCache[obj] as Expr;
                if (expr != null)
                {
                    UnloadQuery(expr);
                }
                return true;
            }
            return false;
        }

        public bool UnloadQuery(Expr expr)
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

        public void Load(string str)
        {
            bool result = LoadQuery(str);
            if (!result)
            {
                _memory.Load(str);
            }
        }

        public void Load(Expr expr)
        {
            bool result = LoadQuery(expr);
            if (!result)
            {
                _memory.Load(expr);
            }
        }
 


        #endregion 
    }
}
