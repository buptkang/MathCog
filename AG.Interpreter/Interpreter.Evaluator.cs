using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using starPadSDK.MathExpr;

namespace AG.Interpreter
{
    public partial class Interpreter : IInterpreter
    {
        private object SearchQuery(object key)
        {
            foreach (KeyValuePair<object, object> pair in _queryCache)
            {
                if (key.Equals(pair.Key))
                    return pair;
            }
            return null;
        }

        public object SearchCurrentQueryResult(object key)
        {
            Expr expr = null;
            var str = key as String;
            if (str != null)
            {
                if(_preQueryCache.ContainsKey(str))
                {
                    expr = _preQueryCache[key] as Expr;
                    Debug.Assert(expr!=null);
                }
                else
                {
                    return null;
                }
            }
            if (expr == null)
            {
                expr = key as Expr;
                if (expr == null) return null;
            }

            foreach (KeyValuePair<object, object> pair in _queryCache)
            {
                if (pair.Key.Equals(expr))
                {
                    return pair.Value;
                }
            }
            return null;
        }

        public int GetNumberOfQueries()
        {
            return _queryCache.Count;
        }
    }
}
