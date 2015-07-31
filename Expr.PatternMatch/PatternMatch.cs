using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AlgebraGeometry;
using CSharpLogic;
using FunctionalCSharp;
using FunctionalCSharp.DiscriminatedUnions;
using starPadSDK.MathExpr;

namespace ExprPatternMatch
{
    public class ExprVisitor
    {
        #region Singleton

        private static ExprVisitor _instance;

        private ExprVisitor()
        {            
        }

        public static ExprVisitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ExprVisitor();                                        
                }

                return _instance;
            }
        }

        #endregion

        public object Match(Expr exp)
        {
            var dict = new Dictionary<PatternEnum, object>();
            object obj;

            bool result = exp.IsLabel(out obj);
            if (result) dict.Add(PatternEnum.Label, obj); //string

            result = exp.IsPoint(out obj); //Algebraic point form
            if (result) dict.Add(PatternEnum.Point, obj); //PointSymbol

            result = exp.IsQuery(out obj);
            if (result) dict.Add(PatternEnum.Query, obj); //EqGoal or Equation

            result = exp.IsEquation(out obj);
            if (result)
            {
                var eq = obj as Equation;
                Debug.Assert(eq != null);

                LineSymbol ls;
                result = eq.IsLineEquation(out ls); //Algebraic line form
                if (result) dict.Add(PatternEnum.Line, ls); //LineSymbol                

                EqGoal eqGoal;
                result = eq.IsEqGoal(out eqGoal); //Property form
                if (result) dict.Add(PatternEnum.Goal, eqGoal); //EqGoal

                QuadraticCurveSymbol qcs;
                result = eq.IsQuadraticCurveEquation(out qcs);
                if (result)
                {
                    CircleSymbol cs;
                    result = qcs.IsCircleEquation(out cs);
                    if(result) dict.Add(PatternEnum.Circle, cs);

                    EllipseSymbol es;
                    result = qcs.IsEllipseEquation(out es);
                    if(result) dict.Add(PatternEnum.Ellipse, es);
                }
            }

            //relation
            LineSymbol lsr;
            result = exp.IsLineRel(out lsr);
            if (result) dict.Add(PatternEnum.Line, lsr); //LineSymbol

            return dict.Count == 1 ? dict.Values.ToList()[0] : dict;
        }
    }

    public enum PatternEnum
    {
        Query,
        Numeric,
        Label,
        Expression,
        Point,
        Line,
        Goal,
        Circle,
        Ellipse
    }
}
