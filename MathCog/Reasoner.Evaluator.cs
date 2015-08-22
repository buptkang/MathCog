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
    using AlgebraGeometry.Expr;
    using CSharpLogic;
    using ExprPatternMatch;
    using starPadSDK.MathExpr;
    public partial class Reasoner
    {
        #region Input Eval and UnEval

        private bool EvalExprPatterns(Expr expr,
                                      object obj,
                                      ShapeType? st,
                                      out object output)
        {
            output = null;

            if (TutorSession)
            {
                return EvalUserModel(expr, obj, st, out output);
            }

            //deterministic
            var ss = obj as ShapeSymbol;
            if (ss != null) return EvalExprPatterns(expr, ss, out output);

            //deterministic
            var goal = obj as EqGoal;
            if (goal != null) return EvalExprPatterns(expr, goal, out output);

            var query = obj as Query;
            if (query != null) return EvalExprPatterns(expr, query, out output);

            var equation = obj as Equation;
            if (equation != null) return EvalExprPatterns(expr, equation, out output);

            //non-deterministic Constraint-Solving
            var str = obj as string;
            var gQuery = new Query(str, st);
            if (str != null) return EvalExprPatterns(expr, gQuery, out output);

            //non-deterministic Multi-Candidate selection
            var dict = obj as Dictionary<PatternEnum, object>;
            //ambiguity of input pattern
            if (dict != null)
            {
                return false;
                //throw new Exception("TODO");
                //return EvalExprPatterns(expr, dict, out output);
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
            return false;
            /*            List<object> objs = dict.Values.ToList();
                        //convert shapesymbol to shape
                        var lst = new List<object>();
                        foreach (object obj in objs)
                        {
                            var ss = obj as ShapeSymbol;
                            if (ss != null)
                            {
                                lst.Add(ss.Shape);
                            }
                            else
                            {
                                lst.Add(obj);
                            }
                        }
                        object obj1 = RelationGraph.Add(lst);
                        return EvalNonDeterministic(expr, obj1, out output);*/
        }

        private bool EvalNonDeterministic(Expr expr, object obj, out object output)
        {
            output = null;
            var shape = obj as Shape;
            if (shape != null)
            {
                var point = shape as Point;
                if (point != null)
                {
                    var ps = new PointSymbol(point);
                    output = new AGShapeExpr(expr, ps);
                    return true;
                }
                var line = shape as Line;
                if (line != null)
                {
                    var ls = new LineSymbol(line);
                    output = new AGShapeExpr(expr, ls);
                    return true;
                }

                var lineSeg = shape as LineSegment;
                if (lineSeg != null)
                {
                    var lss = new LineSegmentSymbol(lineSeg);
                    output = new AGShapeExpr(expr, lss);
                    return true;
                }

                throw new Exception("Cannot reach here");
            }

            var goal = obj as Goal;
            if (goal != null)
            {
                var eqGoal = goal as EqGoal;
                Debug.Assert(eqGoal != null);
                output = new AGPropertyExpr(expr, eqGoal);
                return true;
            }

            var types = obj as List<ShapeType>;
            if (types != null)
            {
                output = types;
                //user interaction required to disambiguate.
                return false;
            }
            return false;
        }

        private bool EvalExprPatterns(Expr expr, Equation eq, out object output)
        {
            RelationGraph.AddNode(eq);
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
        private bool EvalExprPatterns(Expr expr, Query query, out object output)
        {
            object obj = RelationGraph.AddNode(query);
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
        private bool EvalExprPatterns(Expr expr, ShapeSymbol ss, out object output)
        {
            object obj = RelationGraph.AddNode(ss);
            Debug.Assert(obj != null);
            output = new AGShapeExpr(expr, ss);
            return true;
        }

        /// <summary>
        /// Goal Input Patter Match
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="goal"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool EvalExprPatterns(Expr expr, EqGoal goal, out object output)
        {
            object obj = RelationGraph.AddNode(goal);
            output = new AGPropertyExpr(expr, goal);
            return true;
        }

        private void UnEvalExprPatterns(object obj)
        {
            if (TutorSession)
            {
                UnEvalInUserModel(obj);
                return;
            }

            if (_cache.Count == 0) return;
            var shapeSymExpr = obj as AGShapeExpr;
            if (shapeSymExpr != null)
            {
                RelationGraph.DeleteNode(shapeSymExpr.ShapeSymbol);
            }
            var termExpr = obj as AGPropertyExpr;
            if (termExpr != null)
            {
                RelationGraph.DeleteNode(termExpr.Goal);
            }
            var queryExpr = obj as AGQueryExpr;
            if (queryExpr != null)
            {
                RelationGraph.DeleteNode(queryExpr.QueryTag);
            }
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

        #endregion
    }
}
