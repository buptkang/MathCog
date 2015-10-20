using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AlgebraGeometry;
using CSharpLogic;
using starPadSDK.MathExpr;
using starPadSDK.UnicodeNs;

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

            result = exp.IsEquationLabel(out obj);
            if (result)
            {
                var eq = obj as Equation;
                Debug.Assert(eq != null);

                LineSymbol ls = null;

                try
                {
                    result = eq.IsLineEquation(out ls); //Algebraic line form
                }
                catch (Exception)
                {
                }

                if (result) dict.Add(PatternEnum.Line, ls); //LineSymbol
                eq.UnEval();
                eq.CachedEntities.Clear();

                object obj1 = null;
                try
                {
                    result = eq.IsEqGoal(out obj1); //Property form
                }
                catch (Exception)
                {

                }
                var eqGoal = obj1 as EqGoal;
                var eqGoalLst = obj1 as List<object>;
                if (result)
                {
                    if(eqGoal    != null) dict.Add(PatternEnum.Goal, eqGoal); //EqGoal
                    if(eqGoalLst != null) dict.Add(PatternEnum.Goal, eqGoalLst); // list of EqGoal
                }
                if (dict.Count != 0)
                {
                    return dict.Count == 1 ? dict.Values.ToList()[0] : dict;
                }
                eq.UnEval();
                eq.CachedEntities.Clear();

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


        public object UserMatch(Expr exp)
        {
            var dict = new Dictionary<PatternEnum, object>();
            object obj;

            bool result = exp.IsLabel(out obj);
            if (result) dict.Add(PatternEnum.Label, obj); //string

            result = exp.IsPoint(out obj); //Algebraic point form
            if (result) dict.Add(PatternEnum.Point, obj); //PointSymbol

            result = exp.IsQuery(out obj);
            if (result) dict.Add(PatternEnum.Query, obj); //EqGoal or Equation

            if (dict.Count != 0)
            {
                return dict.Count == 1 ? dict.Values.ToList()[0] : dict;
            }

            result = exp.IsNumeric(out obj);
            if (result)
            {
                dict.Add(PatternEnum.Numeric, obj);
                return obj;
            }

            result = exp.IsTerm(out obj);
            if (result)
            {
                dict.Add(PatternEnum.Term, obj);
                return obj;
            }

            result = exp.IsExpression(out obj);
            if (result)
            {
                dict.Add(PatternEnum.Expression, obj);
                return obj;
            }

            result = exp.IsEquationLabel(out obj);
            if (result)
            {
                var eq = obj as Equation;
                Debug.Assert(eq != null);

                if (dict.Count == 0)
                {
                    dict.Add(PatternEnum.Equation, eq);
                    return obj;
                }

                LineSymbol ls;
                result = eq.IsLineEquation(out ls, false); //Algebraic line form
                if (result) 
                {
                    dict.Add(PatternEnum.Line, ls); //LineSymbol
                    //return ls;
                }
                eq.UnEval();

                object obj1;
                result = eq.IsEqGoal(out obj1, false); //Property form
                var eqGoal = obj1 as EqGoal;
                if (result && eqGoal!=null)
                {
                    dict.Add(PatternEnum.Goal, ls); //LineSymbol
                    //return eqGoal;
                }
                eq.UnEval();
            }

            //relation
            LineSymbol lsr;
            result = exp.IsLineRel(out lsr);
            if (result) dict.Add(PatternEnum.Line, lsr); //LineSymbol

            return dict.Count == 1 ? dict.Values.ToList()[0] : dict;

        }


        public object Transform(object obj)
        {
            var term = obj as Term;
            var expression = obj as Expression;
            var equation = obj as Equation;
            var str = obj as string;
            if (term != null)
            {
                return new Query(term);                
            }
            if (str != null)
            {
                return new Query(str);
            }
            if (expression != null)
            {
                return new Query(expression);
            }
            if (equation != null)
            {
                var term1 = equation.Lhs as Term;
                if (term1 != null)
                {
                    return new Query(term1);
                }
            }
            return obj;
        }
    }

    public enum PatternEnum
    {
        Term,
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
