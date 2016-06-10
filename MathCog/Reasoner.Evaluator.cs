/*******************************************************************************
 * Copyright (c) 2015 Bo Kang
 *   
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *  
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *******************************************************************************/

namespace MathCog
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using AlgebraGeometry;
    using CSharpLogic;
    using ExprPatternMatch;
    using starPadSDK.MathExpr;

    public partial class Reasoner
    {
        #region Input Eval and UnEval

        private bool EvalExprPatterns(Expr expr,
                                      object obj,
                                      ShapeType? st,
                                      out object output,
                                      bool userInput = false)
        {
            output = null;

            //non-deterministic Multi-Candidate selection
            var dict = obj as Dictionary<PatternEnum, object>;
            //ambiguity of input pattern
            if (dict != null)
            {
                EvalExprPatterns(expr, dict, out output);
                obj = output;
            }

            //deterministic
            var ss = obj as ShapeSymbol;
            if (ss != null) return EvalExprPatterns(expr, ss, out output, userInput);

            //deterministic
            var goal = obj as EqGoal;
            if (goal != null) return EvalExprPatterns(expr, goal, out output, userInput);

            var query = obj as Query;
            if (query != null) return EvalExprPatterns(expr, query, out output, userInput);

            var equation = obj as Equation;
            if (equation != null) return EvalExprPatterns(expr, equation, out output, userInput);

            //non-deterministic Constraint-Solving
            var str = obj as string;
            var gQuery = new Query(str, st);
            if (str != null) return EvalExprPatterns(expr, gQuery, out output, userInput);

            if (userInput)
            {
                var iknowledge = new IKnowledge(expr);
                iknowledge.Tag = obj;
                output = iknowledge;
            }
            return false;
        }

        /// <summary>
        /// Relation Input Pattern Match
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="dict"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool EvalExprPatterns(Expr expr, Dictionary<PatternEnum, object> dict,
            out object output)
        {
            output = null;
            if (dict.Values.Count == 0) return false;
            List<object> objs = dict.Values.ToList();
            //convert shapesymbol to shape
            var lst = new List<object>();
            foreach (object obj in objs)
            {
                var shapeSymbol = obj as ShapeSymbol;
                var eqGoal = obj as EqGoal;

                bool relExist;
                if (shapeSymbol != null)
                {
                    relExist = RelationGraph.RelationExist(shapeSymbol);
                    if (relExist)
                    {
                        lst.Add(obj);
                    }
                }

                if (eqGoal != null)
                {
                    relExist = RelationGraph.RelationExist(eqGoal);
                    if (relExist)
                    {
                        lst.Add(obj);
                    }
                }
            }

            if (lst.Count == 0)
            {
               
                output = dict.Values.ToList()[0];
                return true;
            }

            if (lst.Count == 1)
            {
                output = lst[0];
                return true;
            }

            if (lst.Count != 0)
            {
                //TODO Non-deterministic selection
                output = lst[0];                
            }
            return true;
        }

        private bool EvalExprPatterns(Expr expr, Equation eq, out object output, bool userInput = false)
        {
            object obj = null;
            if (!userInput)
            {
                obj = RelationGraph.AddNode(eq);
            }
            output = new AGEquationExpr(expr, eq);
            return true;
        }

        /// <summary>
        /// Relation Input Pattern Match
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="str"></param>
        /// <param name="st"></param>
        /// <param name="query"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool EvalExprPatterns(Expr expr, Query query, out object output, bool userInput = false)
        {
            object obj = null;
            if (!userInput)
            {
                obj = RelationGraph.AddNode(query);
            }
            output = new AGQueryExpr(expr, query);
            return obj != null;
            //return EvalNonDeterministic(expr, obj, out output);
        }

        /// <summary>
        /// ShapeSymbol Input Pattern Match
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="ss"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool EvalExprPatterns(Expr expr, ShapeSymbol ss, out object output, bool userInput = false)
        {
            if (!userInput) RelationGraph.AddNode(ss);

            //expr = ExprG.Generate(ss);
            output = new AGShapeExpr(expr, ss);
            return true;
        }

        /// <summary>
        /// Goal Input Patter Match
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="goal"></param>
        /// <param name="output"></param>
        /// <param name="userInput"></param>
        /// <returns></returns>
        private bool EvalExprPatterns(Expr expr, EqGoal goal, out object output, bool userInput = false)
        {
            object obj = null;
            if (!userInput)
            {
                obj = RelationGraph.AddNode(goal);
            }
            output = new AGPropertyExpr(expr, goal);
            return true;
        }

        private bool UnEvalExprPatterns(object obj)
        {
            if (_cache.Count == 0) return false;
            var shapeSymExpr = obj as AGShapeExpr;
            if (shapeSymExpr != null)
            {
                return RelationGraph.DeleteNode(shapeSymExpr.ShapeSymbol);
            }
            var termExpr = obj as AGPropertyExpr;
            if (termExpr != null)
            {
                return RelationGraph.DeleteNode(termExpr.Goal);
            }
            var queryExpr = obj as AGQueryExpr;
            if (queryExpr != null)
            {
                return RelationGraph.DeleteNode(queryExpr.QueryTag);
            }
            return false;
        }

        #endregion

        #region cache utilities

        private List<AGQueryExpr> GetQueryFacts()
        {
            return _cache.Select(pair => pair.Value).OfType<AGQueryExpr>().ToList();
        }

        private List<AGPropertyExpr> GetTermFacts()
        {
            return _cache.Select(pair => pair.Value).OfType<AGPropertyExpr>().ToList();
        }

        private List<AGShapeExpr> GetShapeFacts()
        {
            return _cache.Select(pair => pair.Value).OfType<AGShapeExpr>().ToList();
        }

        #endregion

        #region Test Purpose

        public List<AGShapeExpr> TestGetShapeFacts()
        {
            return GetShapeFacts();
        }

        public List<AGPropertyExpr> TestGetProperties()
        {
            return GetTermFacts();
        }

        public List<AGQueryExpr> TestGetQuerys()
        {
            return GetQueryFacts();
        }

        public IKnowledge SearchKnowledge(string handler)
        {
            if (!_preCache.ContainsKey(handler)) return null;
            var expr = _preCache[handler] as Expr;

            foreach (KeyValuePair<object, object> pair in _cache)
            {
                if (pair.Key.Equals(expr))
                {
                    var obj = pair.Value as IKnowledge;
                    if (obj != null) return obj;
                }
            }

            return null;
        }


        #endregion
    }
}
