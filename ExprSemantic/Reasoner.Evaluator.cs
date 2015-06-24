using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using AlgebraGeometry;
using AlgebraGeometry.Expr;
using CSharpLogic;
using GeometryLogicInference;
using NUnit.Framework;
using starPadSDK.MathExpr;

namespace ExprSemantic
{
    public partial class Reasoner
    {
        #region Input Eval and UnEval

        private void UnEvalExprPatterns(object obj)
        {
            if (_cache.Count == 0) return;

            var shapeSymExpr = obj as AGShapeExpr;
            if (shapeSymExpr != null)
            {
                GeometryInference.Instance.Delete(shapeSymExpr.ShapeSymbol.Shape);
            }

            var termExpr = obj as AGPropertyExpr;
            if (termExpr != null)
            {
                GeometryInference.Instance.Delete(termExpr.Goal);
            }
        }

        private bool EvalExprPatterns(Expr expr, object obj, 
                                      out object output)
        {
            output = null;

            //deterministic
            var ss = obj as ShapeSymbol;
            if (ss != null) return EvalExprPatterns(expr, ss, out output);

            //deterministic
            var goal = obj as EqGoal;
            if (goal != null) return EvalExprPatterns(expr, goal, out output);

            //non-deterministic
            var str = obj as string;
            if (str != null) return EvalExprPatterns(expr, str, out output);

            //non-deterministic
            var dict = obj as Dictionary<PatternEnum, object>;
            //ambiguity of input pattern
            if (dict != null) return EvalExprPatterns(expr, dict, out output);

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
            List<object> objs = dict.Values.ToList();
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
            object obj1 = GeometryInference.Instance.Add(lst);
            return EvalNonDeterministic(expr, obj1, out output);
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

        /// <summary>
        /// Relation Input Pattern Match
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="str"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool EvalExprPatterns(Expr expr, string str,
            out object output)
        {
            output = null;
            object obj = GeometryInference.Instance.Add(str);
            return EvalNonDeterministic(expr, obj, out output);           
        }

        /// <summary>
        /// ShapeSymbol Input Pattern Match
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="ss"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool EvalExprPatterns(Expr expr, ShapeSymbol ss,
            out object output)
        {
            object obj = GeometryInference.Instance.Add(ss.Shape);
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
        private bool EvalExprPatterns(Expr expr, EqGoal goal,
            out object output)
        {
            object obj = GeometryInference.Instance.Add(goal);
            Debug.Assert(obj != null);
            output = new AGPropertyExpr(expr, goal);
            return true;
        }

        #endregion

        #region cache utilities

        private List<AGPropertyExpr> GetTermFacts()
        {
            var lst = new List<AGPropertyExpr>();
            foreach (KeyValuePair<object, object> pair in _cache)
            {
                var temp = pair.Value as AGPropertyExpr;
                if (temp != null)
                {
                    lst.Add(temp);
                }
            }
            return lst;     
        }

        private List<AGShapeExpr> GetShapeFacts()
        {
            var lst = new List<AGShapeExpr>();
            foreach (KeyValuePair<object, object> pair in _cache)
            {
                var temp = pair.Value as AGShapeExpr;
                if (temp != null)
                {
                    lst.Add(temp);
                }
            }
            return lst;
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

        #endregion
    }
}
