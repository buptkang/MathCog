using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSharpLogic;
using NUnit.Framework;
using System.ComponentModel;
using System.Linq.Expressions;

namespace AlgebraGeometry
{
    public static class LineBinaryRelation
    {
        /// <summary>
        /// construct a line through two points
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static LineSymbol Unify(PointSymbol pt1, PointSymbol pt2)
        {
            //point identify check
            if (pt1.Equals(pt2)) return null;

            //Line build process
            if (pt1.Shape.Concrete && pt1.Shape.Concrete)
            {
                var point1 = pt1.Shape as Point;
                var point2 = pt2.Shape as Point;
                Debug.Assert(point1 != null);
                Debug.Assert(point2 != null);
                var line = LineGenerationRule.GenerateLine(point1, point2);
                return line;
            }
            else
            {
                //lazy evaluation    
                //Constraint solving on Graph
                var line = new Line(null); //ghost line
                return new LineSymbol(line);
            }
        }

        /// <summary>
        /// construct a line through a point and a goal,
        /// e.g A(1,2) ^ S = 2=> Conjunctive Norm Form
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static LineSymbol Unify(PointSymbol pt, EqGoal goal)
        {
            throw new Exception("TODO");
        }

        public static LineSymbol Unify(EqGoal goal, PointSymbol pt)
        {
            return Unify(pt, goal);
        }

        /// <summary>
        /// construct a line through two goals
        /// e.g  m=2, k=3 => conjunctive norm form
        /// </summary>
        /// <param name="goal1"></param>
        /// <param name="goal2"></param>
        /// <returns></returns>
        public static LineSymbol Unify(EqGoal goal1, EqGoal goal2)
        {
            var variable1 = goal1.Lhs as Var;
            var variable2 = goal2.Lhs as Var;
            Debug.Assert(variable1 != null);
            Debug.Assert(variable2 != null);

            var dict = new Dictionary<string, object>();

            string slopeKey = "slope";
            string interceptKey = "intercept";
            if (variable1.ToString().Equals(LineAcronym.Slope1))
            {
                dict.Add(slopeKey, goal1.Rhs);
            }
            if (variable1.ToString().Equals(LineAcronym.Intercept1))
            {
                dict.Add(interceptKey, goal1.Rhs);
            }
            if (variable2.ToString().Equals(LineAcronym.Slope1))
            {
                if (dict.ContainsKey(slopeKey)) return null;
                dict.Add(slopeKey, goal2.Rhs);
            }
            if (variable2.ToString().Equals(LineAcronym.Intercept1))
            {
                if (dict.ContainsKey(interceptKey)) return null;
                dict.Add(interceptKey, goal2.Rhs);
            }

            if (dict.Count == 2 &&
                dict[slopeKey] != null &&
                dict[interceptKey] != null)
            {
                if (LogicSharp.IsNumeric(dict[slopeKey]) &&
                    LogicSharp.IsNumeric(dict[interceptKey]))
                {
                    double dSlope, dIntercept;
                    LogicSharp.IsDouble(dict[slopeKey], out dSlope);
                    LogicSharp.IsDouble(dict[interceptKey], out dIntercept);
                    var line = LineGenerationRule.GenerateLine(dSlope, dIntercept);
                    var ls = new LineSymbol(line) {OutputType = LineType.SlopeIntercept};
                    return ls;
                }
                else
                {
                    //lazy evaluation    
                    //Constraint solving on Graph
                    var line = new Line(null); //ghost line
                    var ls = new LineSymbol(line);
                    ls.OutputType = LineType.SlopeIntercept;
                    return ls;                    
                }
            }
            return null;
        }
    }

    public static class LineUnaryRelation
    {
        public static bool Unify(this LineSymbol shapeSymbol, object constraint, out object output)
        {
            output = shapeSymbol.Unify(constraint);
            return output != null;
        }

        public static object Unify(this LineSymbol ls, object constraint)
        {
            var refObj = constraint as string;
            Debug.Assert(refObj != null);

            switch (refObj)
            {
                case LineAcronym.A:
                case LineAcronym.A1:
                    break;
                case LineAcronym.B:
                case LineAcronym.B1:
                    break;
                case LineAcronym.C:
                case LineAcronym.C1:
                    break;
                case LineAcronym.Slope1:
                case LineAcronym.Slope2:
                case LineAcronym.Slope3:
                    return ls.InferSlope(refObj);
                case LineAcronym.Intercept1:
                case LineAcronym.Intercept2:
                case LineAcronym.Intercept3:
                    return true;
                case LineAcronym.GeneralForm1:
                case LineAcronym.GeneralForm2:
                case LineAcronym.GeneralForm3:
                case LineAcronym.GeneralForm4:
                    return ls.InferGeneralForm(refObj);
                case LineAcronym.SlopeInterceptForm1:
                case LineAcronym.SlopeInterceptForm2:
                    return ls.InferSlopeInterceptForm(refObj);
            }

            return null;
        }

        private static LineSymbol InferSlopeInterceptForm(this LineSymbol inputLineSymbol, string label)
        {
            //TODO
            var line = inputLineSymbol.Shape as Line;
            Debug.Assert(line != null);
            var ls = new LineSymbol(line);
            ls.OutputType = LineType.SlopeIntercept;
            return ls;
        }

        private static LineSymbol InferGeneralForm(this LineSymbol inputLineSymbol, string label)
        {
            var line = inputLineSymbol.Shape as Line;
            Debug.Assert(line != null);
            var ls = new LineSymbol(line);
            ls.OutputType = LineType.GeneralForm;
            return ls;
        }

        private static EqGoal InferSlope(this LineSymbol inputLineSymbol, string label)
        {
            var line = inputLineSymbol.Shape as Line;
            Debug.Assert(line != null);
            var goal =  new EqGoal(new Var(label), line.Slope);

            if (line.InputType == LineType.SlopeIntercept)
            {
                //Debug.Assert(line.Slope != null);
                /*                var newGoal = new EqGoal(egGoal.Lhs, line.Slope);
                                egGoal.CachedEntities.Add(newGoal);*/
                //TODO
                
            }
            
            if (line.InputType == LineType.GeneralForm)
            {
                #region Infer Slope Procedure

                var steps = inputLineSymbol.FromLineGeneralFormToSlopeTrace(goal);

                if (goal.Traces != null)
                {
                    goal.Traces.AddRange(steps);
                }
                else
                {
                    goal.Traces = steps;
                }

                /*Debug.Assert(line.A != null);
                Debug.Assert(line.B != null);
                Debug.Assert(line.C != null);

                if (line.Concrete)
                {
                    double slope = (-1*(double)line.A)/(double)line.B;
                    var newGoal = new EqGoal(egGoal.Lhs, slope);
                    egGoal.CachedEntities.Add(newGoal);
                }
                else
                {
                    throw new Exception("TODO");
                    //ax+by+c=0
                    var term1 = new Term(Expression.Multiply, new List<object>() { -1, line.A });
                    var term2 = new Term(Expression.Divide, new List<object>() { term1, line.B });
                    object obj = term2.Eval();
                    if (LogicSharp.IsNumeric(obj))
                    {
                        var newGoal = new EqGoal(egGoal.Lhs, line.Slope);
                        egGoal.CachedEntities.Add(newGoal);
                        return;
                    }
                }*/
                #endregion
            }
            
            if (line.InputType == LineType.Relation)
            {
                //TODO
                /*  var newGoal = new EqGoal(egGoal.Lhs, line.Slope);
                  egGoal.CachedEntities.Add(newGoal);*/
            }

            return goal;
        }
    }
}
