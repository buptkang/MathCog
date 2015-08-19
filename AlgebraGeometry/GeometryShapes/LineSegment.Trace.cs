using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public partial class LineSegmentSymbol : ShapeSymbol
    {
        /// <summary>
        /// from two points to generate line segment
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public List<TraceStep> FromPointsToLineSegment(Point pt1, Point pt2, object target)
        {
            //no need to show this 
            return null;
        }

        public List<TraceStep> FromLineSegmentToDistance(LineSegment ls, object target)
        {
            var lst = new List<TraceStep>();

            string step1metaRule = "The Distance Function between two points it: d^2=(x0-x1)^2+(y0-y1)^2";
            string step1AppliedRule = String.Format(
                "Substitute two points into the distance function d^2=({0}-{1})^2+({2}-{3})^2",
                ls.Pt1.XCoordinate.ToString(), 
                ls.Pt2.XCoordinate.ToString(),
                ls.Pt1.YCoordinate.ToString(),
                ls.Pt2.YCoordinate.ToString());

            var variable = new Var('d');
            var lhs = new Term(Expression.Power, new List<object>() {variable, 2.0});

            var term1  = new Term(Expression.Subtract, new List<object>() {ls.Pt1.XCoordinate, ls.Pt2.XCoordinate});
            var term11 = new Term(Expression.Power, new List<object>() {term1, 2.0});
            var term2  = new Term(Expression.Subtract, new List<object>() {ls.Pt1.YCoordinate, ls.Pt2.YCoordinate});
            var term22 = new Term(Expression.Power, new List<object>() {term2, 2.0});
            var rhs = new Term(Expression.Add, new List<object>() {term11, term22});
            var eq = new Equation(lhs, rhs);

            var pt1X = new Var("x0");
            var pt1Y = new Var("y0");
            var pt2X = new Var("x1");
            var pt2Y = new Var("y1");

            var term1_1 = new Term(Expression.Subtract, new List<object>() {pt1X, pt2X});
            var term11_1 = new Term(Expression.Power, new List<object>() {term1_1, 2.0});
            var term2_1  = new Term(Expression.Subtract, new List<object>() {pt1Y, pt2Y});
            var term22_1 = new Term(Expression.Power, new List<object>() {term2_1, 2.0});
            var rhs_1 = new Term(Expression.Add, new List<object>() {term11_1, term22_1});
            var old_eq = new Equation(lhs, rhs_1);

            var trace = new TraceStep(old_eq, eq, step1metaRule, step1AppliedRule);
            lst.Add(trace);

            EqGoal eqGoal;
            bool result =  eq.IsEqGoal(out eqGoal);
            Debug.Assert(result);
            Debug.Assert(eqGoal != null);

            lst.AddRange(eqGoal.Traces);


            return lst;
        }
    }
}
