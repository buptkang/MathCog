using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AG.Interpreter.Instructions;
using AlgebraGeometry.Expr;
using ExprSemantic;
using starPadSDK.MathExpr;

namespace AG.Interpreter
{
    public partial class Interpreter : IInterpreter
    {
        public object Eval(Expr expr, object input)
        {
            if (input == null) return null;

            var str = input as string;
            if(str != null)
            {
                //query failed with instruction.
                var qe = new AGQueryExpr(expr)
                {
                   QuerySuccess = false,
                   Instruction = str
                };
                return qe;
            }

            var lst = input as List<object>;
            Debug.Assert(lst != null);

            if (lst.Count == 1)
            {
                object temp = lst[0] as QueryResult;

                #region property eval

                var propQuery = temp as PropertyQueryResult;
                if(propQuery != null)
                {
                    AGQueryExpr agQuery;
                    if (propQuery.QuerySuccess) 
                    {
                        agQuery = new AGQueryExpr(propQuery.Answer);
                        agQuery.QuerySuccess = true;
                        agQuery.Instruction = agQuery.Instruction;
                        agQuery.KnowledgeTrace = propQuery.Trace;
                    }
                    else
                    {
                        agQuery = new AGQueryExpr(expr);
                        agQuery.QuerySuccess = false;
                        agQuery.Instruction = agQuery.Instruction;                       
                    }
                    return agQuery;
                }

                #endregion
            }
            else
            {
                var agQuery = new AGQueryExpr(expr)
                {
                    QuerySuccess = false,
                    Instruction = QueryInstructions.QueryAmbiguity
                };
                return agQuery;
            }
            return null;
        }

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
