using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSharpLogic;
using AlgebraGeometry;
using AlgebraGeometry.Expr;

namespace ExprSemantic
{
    public partial class Reasoner
    {
        #region Querying Public Interface

        public bool Answer(object query, out object output)
        {
            output = null;
            if (_cache.Count == 0)
            {
                output = Instructions.NoKnowledge;
                return false;
            }

            List<object> lstOptions;

            #region Querying variable (Or Property or EqGoal)

            var variable = query as Var;
            if (variable != null)
            {
                lstOptions = RetrievePropertyResult(variable);
                if (lstOptions.Count == 0)
                {
                    return false;
                }
                else
                {
                    output = lstOptions;
                    return true;
                }
            }

            var term = query as Term;
            if (term != null)
            {
                lstOptions = RetrievePropertyResult(term);
                if (lstOptions.Count == 0)
                {
                    return false;
                }
                else
                {
                    output = lstOptions;
                    return true;
                }
            }
           
            #endregion

            //TODO Querying knowledge(Or Shape itself)
            //TODO Querying Relation

            return false;
        }

        private List<object> RetrievePropertyResult(Term term)
        {
            //TODO
            throw new Exception("Not support now");
        }

        private List<object> RetrievePropertyResult(Var variable)
        {
            var result = new List<object>();
            var properties = GetTermFacts();
            foreach (AGPropertyExpr agProp in properties)
            {
                object obj = ReasonInterpreter.Infer(variable, agProp);
                if (obj != null)
                {
                    Debug.Assert(obj is PropertyQueryResult);
                    result.Add(obj);
                }
            }
            return result;
        }

        #endregion
    }
}