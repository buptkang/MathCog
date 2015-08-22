using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AlgebraGeometry;
using CSharpLogic;
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

/*                Equation outputEq;
                bool? result1 = eq.Eval(out outputEq, false);
                if (result1 != null) return false;*/

                LineSymbol ls;
                result = eq.IsLineEquation(out ls); //Algebraic line form
                if (result) dict.Add(PatternEnum.Line, ls); //LineSymbol
                eq.UnEval();

                object obj1;
                result = eq.IsEqGoal(out obj1); //Property form
                var eqGoal = obj1 as EqGoal;
                if (result) dict.Add(PatternEnum.Goal, eqGoal); //EqGoal
                eq.UnEval();

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

                if (dict.Count == 0)
                {
                    dict.Add(PatternEnum.Equation, eq);
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
        Equation,
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
