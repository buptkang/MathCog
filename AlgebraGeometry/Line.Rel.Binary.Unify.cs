using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSharpLogic;
using NUnit.Framework;

namespace AlgebraGeometry
{
    public static class LineRelation
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
}
