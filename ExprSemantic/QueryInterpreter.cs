using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlgebraGeometry.Expr;
using CSharpLogic;
using AlgebraGeometry;
using ExprSemantic;
using NUnit.Framework;

namespace ExprSemantic
{
    /// <summary>
    /// AGLogic Derive from LogicSharp to make inference
    /// </summary>
    public class ReasonInterpreter
    {
        public static object Infer(Var variable, AGPropertyExpr prop)
        {
            EqGoal queryGoal;
            int derivation = prop.KnowledgeTrace.Count;
            if (derivation == 0) // no derivation
            {
                queryGoal = prop.Goal;
            }
            else
            {
                var traceLst = prop.Goal.Traces;
                queryGoal = traceLst[0].Target as EqGoal;
            }

            object result = LogicSharp.Run(variable, queryGoal);
            //Interpretation of result
            if (result == null) return null;
            var queryResult = new PropertyQueryResult(variable);

            if (LogicSharp.IsNumeric(result))
            {
                queryResult.QuerySuccess = true;
                if (derivation == 0)
                {
                    queryResult.Answer = prop.Expr;
                    //check trace
                    queryResult.Trace = null;
                    queryResult.Instruction = Instructions.GivenKnowledge;
                }
                else
                {
                    var symbolicTraceLst  = prop.KnowledgeTrace;
                    queryResult.Answer = symbolicTraceLst[0].Target;
                    //check trace
                    queryResult.Trace = prop.KnowledgeTrace;
                    queryResult.Instruction = Instructions.ShowTraceFromProperty;
                }
            }
            else
            {
                queryResult.QuerySuccess = false;
                queryResult.Instruction = Instructions.UnassignValue + result;
            }

            return queryResult;
        }

        public static object Infer(Var variable, Shape obj)
        {
            var point = obj as Point;
            if (point != null)
            {
                return Infer(variable, point);
            }

            return null;
            //TODO, line, circle
        }

        private static object Infer(Var variable, Point pt)
        {
            if (pt.Properties.ContainsKey(variable))
            {
                return pt.Properties[variable];
            }
            else
            {
                if (variable.Token.ToString().Equals("x") ||
                    variable.Token.ToString().Equals("X"))
                {
                    return pt.XCoordinate;
                }
                else if (variable.Token.ToString().Equals("Y") ||
                         variable.Token.ToString().Equals("y"))
                {
                    return pt.YCoordinate;
                }
                else
                {
                    return null;
                }
            }               
        }
    }

}
