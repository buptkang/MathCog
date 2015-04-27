using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using AGSemantic.KnowledgeBase;
using ExprSemantic.KnowledgeBase;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using starPadSDK.MathExpr;
using starPadSDK.MathExpr.TextInternals;

namespace ExprSemantic.KnowledgeUnification
{
    public class AGUnifier
    {
        #region Singleton

        private static AGUnifier _instance;

        private AGUnifier()
        {

        }

        public static AGUnifier Instance
        {
            get
            {
                _instance = _instance ?? new AGUnifier();
                return _instance;
            }
        }

        private int _currentIndex = 0;
        public IDictionary<string, List<AGExprTermAttribute>> EqCoefficientDict { get; set; }
        public List<AGExprTerm> AGExprTermLst { get; set; }

        #endregion

        #region Algebra Input methods

        private void InitAlgebraicTracer(Expr expr)
        {
            //CurrExpr = expr;
            //PrevExpr = expr;
            //CurrentAlgebraicTracer = new List<AGKnowledgeTracer>();
            EqCoefficientDict = new Dictionary<string, List<AGExprTermAttribute>>
            {
                {"XX", new List<AGExprTermAttribute>()},
                {"X",  new List<AGExprTermAttribute>()},
                {"YY", new List<AGExprTermAttribute>()},
                {"Y",  new List<AGExprTermAttribute>()},
                {"XY", new List<AGExprTermAttribute>()},
                {"C",  new List<AGExprTermAttribute>()}
            };
            AGExprTermLst = new List<AGExprTerm>();
            _currentIndex = 0;
        }

        /// <summary>
        /// Algebra Expression Verification 
        /// This method will check if the current expr can fit to any existing knowledge form. 
        /// If it matches the knowledge, an instance will be added and the method returns true;
        /// else the method return false
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public bool VerifyShapeKnowledge(Expr expr, out IKnowledgeExpr shape, out List<AGKnowledgeTracer> tracer)
        {
            shape = null;
            tracer = null;
            try
            {
                PointExpr pointExpr;
                if (expr.IsPointForm(out pointExpr))
                {
                    shape = pointExpr;
                    tracer = null;
                    return true;
                }

                List<AGKnowledgeTracer> result = RetrieveShapeGeneralForm(expr, out shape);
            
                if (shape == null)
                {
                    return false;
                }
                else
                {
                    if (shape is LineExpr)
                    {
                        tracer = result;
                    }

/*
                    if (shape is CircleExpr)
                    {
                        var circleExpr = shape as CircleExpr;
                        tracer = result.Concat(circleExpr.CircleStandardFormTrace).ToList();
                    }
                    else if (shape is EllipseExpr)
                    {
                        var ellipseExpr = shape as EllipseExpr;
                        tracer = result.Concat(ellipseExpr.EllipseStandardFormTrace).ToList();
                    }
                    else if (shape is LineExpr)
                    {
                        tracer = result;
                    }
*/
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// This method transform the expression into general form of the shape.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="shapeExpr"></param>
        /// <returns></returns>
        private List<AGKnowledgeTracer> RetrieveShapeGeneralForm(Expr expr, out IKnowledgeExpr shapeExpr)
        {
            var CurrentAlgebraicTracer = new List<AGKnowledgeTracer>();

            Expr CurrExpr = expr;
            Expr PrevExpr = expr;
            shapeExpr = null;
            InitAlgebraicTracer(expr);

            if (!(expr is CompositeExpr)) return null;
            var composite = expr as CompositeExpr;
            if (!(composite.Head.Equals(WellKnownSym.equals) && composite.Args.Length == 2))
            {
                return null;
            }

            // Trace 1
            bool success = false;
            ParseOneSideOfEquation(composite.Args[0], true);
            success = ParseOneSideOfEquation(composite.Args[1], false);
            if (success)
            {
                CurrExpr = GenerateExpr();
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGStrategyHint.AlgebraicTransformation, AGStrategyHint.AlgebraicTransformation, AGAppliedRule.AlgebraMovingTerms));
            }

            // Trace 2
            success = false;
            success = Sort();
            if (success)
            {
                PrevExpr = CurrExpr;
                CurrExpr = GenerateExpr();
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.MoveTermsFromRightToLeft, AGStrategyHint.AlgebraicTransformation, AGAppliedRule.AlgebraCommutativeLaw));
            }

            // Trace 3
            success = false;
            success = MergeTerms();
            if (success)
            {
                PrevExpr = CurrExpr;
                CurrExpr = GenerateExpr();
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.CommutativeLaw, AGStrategyHint.AlgebraicTransformation, AGAppliedRule.AlgebraMergeLikeTerms));
            }

            //Determine the shape of the expression
            shapeExpr = DetectKnowledgeShape(CurrExpr);

            if (shapeExpr is LineExpr)
            {
                var le = shapeExpr as LineExpr;
                PrevExpr = CurrExpr;
                CurrExpr = le.TwoInterceptsExpr;
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.MergeLaw, AGStrategyHint.AlgebraicTransformation, AGAppliedRule.FindIntercepts));

                PrevExpr = CurrExpr;
                CurrExpr = le.DrawLineExpr;
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.FindIntercepts, AGStrategyHint.AlgebraicTransformation, "Plot the line passing 2 points"));

                //CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(CurrExpr, null,"hello world", AGStrategyHint.AlgebraicTransformation, ""));
            }

            return CurrentAlgebraicTracer;
        }

        /// <summary>
        /// Trace 1: move all the terms from the right equation to the left
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="isLeftSide"></param>
        /// <returns></returns>
        private bool ParseOneSideOfEquation(Expr expr, bool isLeftSide)
        {
            DoubleNumber coefficient;

            if (expr.IsConstantTerm(out coefficient))
            {
                coefficient = isLeftSide ? coefficient : new DoubleNumber(coefficient.Num * -1);
                if (!coefficient.Num.Equals(0.0))
                {
                    EqCoefficientDict["C"].Add(new AGExprTermAttribute(coefficient, _currentIndex++));
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (expr.IsXTerm(out coefficient))
            {
                coefficient = isLeftSide ? coefficient : new DoubleNumber(coefficient.Num * -1);
                EqCoefficientDict["X"].Add(new AGExprTermAttribute(coefficient, _currentIndex++));
                return true;
            }

            if (expr.IsYTerm(out coefficient))
            {
                coefficient = isLeftSide ? coefficient : new DoubleNumber(coefficient.Num * -1);
                EqCoefficientDict["Y"].Add(new AGExprTermAttribute(coefficient, _currentIndex++));
                return true;
            }

            if (expr.IsXXTerm(out coefficient))
            {
                coefficient = isLeftSide ? coefficient : new DoubleNumber(coefficient.Num * -1);
                EqCoefficientDict["XX"].Add(new AGExprTermAttribute(coefficient, _currentIndex++));
                return true;
            }

            if (expr.IsYYTerm(out coefficient))
            {
                coefficient = isLeftSide ? coefficient : new DoubleNumber(coefficient.Num * -1);
                EqCoefficientDict["YY"].Add(new AGExprTermAttribute(coefficient, _currentIndex++));
                return true;
            }

            if (!(expr is CompositeExpr)) return false;

            var compositeExpr = expr as CompositeExpr;
            if (!compositeExpr.Head.Equals(WellKnownSym.plus)) return false;

            Expr[] exprs = compositeExpr.Args;

            for (int i = 0; i < exprs.Length; i++)
            {
                if (exprs[i].IsXTerm(out coefficient))
                {
                    coefficient = isLeftSide ? coefficient : new DoubleNumber(coefficient.Num * -1);
                    EqCoefficientDict["X"].Add(new AGExprTermAttribute(coefficient, _currentIndex + i));
                }
                else if (exprs[i].IsYTerm(out coefficient))
                {
                    coefficient = isLeftSide ? coefficient : new DoubleNumber(coefficient.Num * -1);
                    EqCoefficientDict["Y"].Add(new AGExprTermAttribute(coefficient, _currentIndex + i));
                }
                else if (exprs[i].IsXXTerm(out coefficient))
                {
                    coefficient = isLeftSide ? coefficient : new DoubleNumber(coefficient.Num * -1);
                    EqCoefficientDict["XX"].Add(new AGExprTermAttribute(coefficient, _currentIndex + i));
                }
                else if (exprs[i].IsYYTerm(out coefficient))
                {
                    coefficient = isLeftSide ? coefficient : new DoubleNumber(coefficient.Num * -1);
                    EqCoefficientDict["YY"].Add(new AGExprTermAttribute(coefficient, _currentIndex + i));
                }
                else if (exprs[i].IsConstantTerm(out coefficient))
                {
                    coefficient = isLeftSide ? coefficient : new DoubleNumber(coefficient.Num * -1);
                    if (!coefficient.Num.Equals(0.0))
                    {
                        EqCoefficientDict["C"].Add(new AGExprTermAttribute(coefficient, _currentIndex + i));
                    }
                }
                else
                {

                }
            }
            _currentIndex += exprs.Length; //record the current term index in the left side of equation.
            return true;
        }

        /// <summary>
        /// Trace 2: Sort all terms
        /// </summary>
        /// <returns></returns>
        public bool Sort()
        {
            TransformToListOfAGExpr();
            bool isSort = false;
            //Insert Sort Algo
            string name;
            AGExprTermAttribute currTerm;
            for (int i = 1; i < _currentIndex; i++)
            {
                AGExprTerm term = AGExprTermLst[i];
                int j = i;
                while (j > 0 && term.CompareTo(AGExprTermLst[j - 1]) < 0)
                {
                    AGExprTermLst[j] = AGExprTermLst[j - 1];
                    j -= 1;
                    isSort = true;
                }
                AGExprTermLst[j] = term;
            }

            TransformFromListOfAGExpr();

            return isSort;
        }

        /// <summary>
        /// Rule: Merging terms with the same variable 
        /// Trace 3: Merge terms
        /// </summary>
        private bool MergeTerms()
        {
            AGExprTermLst = new List<AGExprTerm>();
            bool isMerge = false;
            foreach (KeyValuePair<string, List<AGExprTermAttribute>> pair in EqCoefficientDict)
            {
                string name = pair.Key;
                List<AGExprTermAttribute> terms = pair.Value;

                if (terms.Count == 0) continue;

                double mergeCoeff = 0.0;
                if (!isMerge)
                {
                    isMerge = terms.Count > 1;
                }
                mergeCoeff += terms.Sum(term => term.Coefficient.Num);

                if (mergeCoeff.Equals(0.0)) continue;

                AGExprTermLst.Add(new AGExprTerm(mergeCoeff, name));
            }

            _currentIndex = AGExprTermLst.Count;

            TransformFromListOfAGExpr();

            return isMerge;
        }

        #region Transform Ellipse from General Form to Standard Form
/*
        public List<AGKnowledgeTracer> TransformEllipseFromGeneralFormToStandardForm(ShapeExpr ellipseExpr)
        {
            Expr CurrExpr = ellipseExpr.GeneralExpr;
            Expr PrevExpr;

            var CurrentAlgebraicTracer = new List<AGKnowledgeTracer>();

            //1. Move constant to the right
            Expr outputExpr;
            if (MoveConstantFromLeftToRight(CurrExpr, out outputExpr))
            {
                PrevExpr = CurrExpr;
                CurrExpr = outputExpr;
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.FactorizeCircle4));
            }

            //2. Normalize the coefficient on the constant term
            if (NormalizeEllipseCoefficient(CurrExpr, out outputExpr))
            {
                PrevExpr = CurrExpr;
                CurrExpr = outputExpr;
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.FactorizeEllipse1));
            }

            // 3.Calculate the real coefficient on the variable's coefficient.
            if (CalculateEllipseCoefficient(CurrExpr, out outputExpr))
            {
                PrevExpr = CurrExpr;
                CurrExpr = outputExpr;
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.FactorizeEllipse2));
            }

            // 4. Square root form of ellipse Coefficient
            if (ShowSquareRootFormEllipse(CurrExpr, out outputExpr))
            {
                PrevExpr = CurrExpr;
                CurrExpr = outputExpr;
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.FactorizeCircle5));
            }

            return CurrentAlgebraicTracer;
        }
 */ 

        #region Step 4

        private bool ShowSquareRootFormEllipse(Expr expr, out Expr outputExpr)
        {
            outputExpr = expr.Clone();
            Expr[] exprs = AGShapeUtils.ExtractTerms(outputExpr);
            foreach (Expr temp in exprs)
            {
                UpdateSquareRootForm(temp);
            }
            return true;
        }

        private void UpdateSquareRootForm(Expr expr)
        {
            var compositeExpr = expr as CompositeExpr;
            compositeExpr = compositeExpr.Args[1] as CompositeExpr;

            var doubleNumber = compositeExpr.Args[0] as IntegerNumber;
            double radius = Math.Sqrt(double.Parse(doubleNumber.Num.ToString()));

            Number numExpr;
            if ((radius % 1).Equals(0))
            {
                numExpr = new IntegerNumber(Int32.Parse(radius.ToString()));
            }
            else
            {
                numExpr = new DoubleNumber(radius);
            }

            var substituteExpr = new CompositeExpr(WellKnownSym.power, new Expr[]
            {
               numExpr,
               new IntegerNumber(2) 
            });

            compositeExpr.Args[0] = substituteExpr;
        }

        #endregion 

        #region Step 3

        private bool CalculateEllipseCoefficient(Expr expr, out Expr outputExpr)
        {
            outputExpr = expr.Clone();
            Expr[] exprs = AGShapeUtils.ExtractTerms(outputExpr);

            bool eliminated1 = EliminateCoefficient(exprs[0]);
            bool eliminated2 = EliminateCoefficient(exprs[1]);

            if (eliminated1 || eliminated2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool EliminateCoefficient(Expr expr)
        {
            var compositeExpr = expr as CompositeExpr;
            Expr divisorExpr = compositeExpr.Args[0];
            Expr dividentExpr = compositeExpr.Args[1];

            double divisor = RetrieveDivisor(divisorExpr);
            double divident = RetrieveDivideNumber(dividentExpr);
            double gcdNumber = AGShapeUtils.GCD(divisor, divident);

            if (gcdNumber.Equals(1.0))
            {
                return false;
            }

            UpdateDivisor(divisorExpr, divisor / gcdNumber, out compositeExpr.Args[0]);
            UpdateDivident(dividentExpr, divident / gcdNumber);
            return true;
        }

        private void UpdateDivisor(Expr expr, double value, out Expr outputExpr)
        {
            outputExpr = null;
            var compositeExpr = expr as CompositeExpr;
            Number numExpr;
            if ((value % 1).Equals(0))
            {
                numExpr = new IntegerNumber(Int32.Parse(value.ToString()));
            }
            else
            {
                numExpr = new DoubleNumber(value);
            }

            if (value.Equals(1.0))
            {
                outputExpr = compositeExpr.Args[1];
            }
            else
            {
                compositeExpr.Args[0] = numExpr;
                outputExpr = compositeExpr;
            }

        }

        private void UpdateDivident(Expr expr, double value)
        {
            var compositeExpr = expr as CompositeExpr;
            var doubleNumber = compositeExpr.Args[0] as IntegerNumber;
            Number numExpr;
            if ((value % 1).Equals(0))
            {
                numExpr = new IntegerNumber(Int32.Parse(value.ToString()));
            }
            else
            {
                numExpr = new DoubleNumber(value);
            }
            //compositeExpr.Args[0] = numExpr;
            if (value.Equals(1.0))
            {
                expr = compositeExpr.Args[1];
            }
            else
            {
                compositeExpr.Args[0] = numExpr;
            }
        }

        private double RetrieveDivisor(Expr expr)
        {
            var compositeExpr = expr as CompositeExpr;
            DoubleNumber doubleNumber;
            if (expr.IsXXTerm(out doubleNumber))
            {
                return doubleNumber.Num;
            }

            if (expr.IsYYTerm(out doubleNumber))
            {
                return doubleNumber.Num;
            }

            return 0.0;
        }

        private double RetrieveDivideNumber(Expr expr)
        {
            var compositeExpr = expr as CompositeExpr;
            var doubleExpr = compositeExpr.Args[0] as IntegerNumber;
            return double.Parse(doubleExpr.Num.ToString());
        }

        #endregion

        /// <summary>
        /// Step 2 for Ellipse
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="outputExpr"></param>
        /// <returns></returns>
        private bool NormalizeEllipseCoefficient(Expr expr, out Expr outputExpr)
        {
            outputExpr = null;
            var compositeExpr = expr as CompositeExpr;
            var doubleExpr = compositeExpr.Args[1] as IntegerNumber;
            if (doubleExpr.Num.Equals("1"))
            {
                return false;
            }
            else
            {
                outputExpr = expr.Clone() as CompositeExpr;

                Expr[] exprs = AGShapeUtils.ExtractTerms(outputExpr);

                Expr tempExpr = new CompositeExpr(WellKnownSym.divide, doubleExpr);
                exprs[0] = new CompositeExpr(WellKnownSym.times, new Expr[]
                {
                    exprs[0],
                    tempExpr   
                });

                tempExpr = new CompositeExpr(WellKnownSym.divide, doubleExpr);
                exprs[1] = new CompositeExpr(WellKnownSym.times, new Expr[]
                {
                    exprs[1],
                    tempExpr   
                });

                Expr[] exprs2 = outputExpr.Args();
                exprs2[1] = new IntegerNumber(1);
                return true;
            }

        }

        #endregion

        #region Circle General Form to Standard Form
/*
        public List<AGKnowledgeTracer> TransformCircleFromGeneralFormToStandardForm(ShapeExpr shapeExpr)
        {
            Expr CurrExpr = shapeExpr.GeneralExpr;
            Expr PrevExpr;
            var CurrentAlgebraicTracer = new List<AGKnowledgeTracer>();

            var circleExpr = shapeExpr as CircleExpr;
            var circle = circleExpr.AGShape as Circle;
            //Circle Step 1
            if (NormalizeCircleCoefficient(circle))
            {
                PrevExpr = CurrExpr;
                CurrExpr = GenerateExpr();
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.FactorCircle0));
            }

            //Circle Step 2 Factorization
            if (PrepareForPerfectSquare(circle))
            {
                PrevExpr = CurrExpr;
                CurrExpr = GenerateExpr();
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.FactorizeCircle1));
            }

            //Circle Step 3 Merge
            if (MergeCircleAdditionalConstants(circle))
            {
                PrevExpr = CurrExpr;
                CurrExpr = GenerateExpr();
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.FactorizeCircle2));
            }

            //Circle Step 4 factorize the circle without using the hashtable
            Expr outputExpr;
            if (FactorizeCircle(CurrExpr, circle, out outputExpr))
            {
                PrevExpr = CurrExpr;
                CurrExpr = outputExpr;
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.FactorizeCircle3));
            }

            //Circle Step 5: Move constant term to the right (both circle and ellipse) 
            if (MoveConstantFromLeftToRight(CurrExpr, out outputExpr))
            {
                PrevExpr = CurrExpr;
                CurrExpr = outputExpr;
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.FactorizeCircle4));
            }
            
            //Circle Step 6: Square the Number
            if (ShowSquareRootForm(CurrExpr, out outputExpr))
            {
                PrevExpr = CurrExpr;
                CurrExpr = outputExpr;
                CurrentAlgebraicTracer.Add(new AGKnowledgeTracer(PrevExpr, CurrExpr, AGKnowledgeHints.FactorizeCircle5));
            }
            
            return CurrentAlgebraicTracer;
        }
*/


        /// <summary>
        /// Circle Step 1 
        /// </summary>
        /// <param name="circle"></param>
        /// <returns></returns>
        private bool NormalizeCircleCoefficient(Circle circle)
        {
            if (circle.A.Equals(1.0)) return false;

            //6. divide the coefficient of X^2
            foreach (KeyValuePair<string, List<AGExprTermAttribute>> pair in EqCoefficientDict)
            {
                if (pair.Value.Count == 1)
                {
                    var attribute = pair.Value[0] as AGExprTermAttribute;

                    attribute.Coefficient.Num = attribute.Coefficient.Num / circle.A;
                }
            }
            return true;
        }

        /// <summary>
        /// Circle Step 2
        /// </summary>
        /// <param name="circle"></param>
        /// <returns></returns>
        private bool PrepareForPerfectSquare(Circle circle)
        {
            bool isFactorized = false;

            AGExprTermLst = new List<AGExprTerm>();

            double xConstCoeff = 0.0;
            if (!circle.D.Equals(0.0)) // has X coefficient
            {
                isFactorized = true;
                xConstCoeff = Math.Pow((Math.Abs(circle.D) * 0.5), 2.0);
                AGExprTermLst.Add(new AGExprTerm(circle.A, "XX"));
                AGExprTermLst.Add(new AGExprTerm(circle.D, "X"));
                AGExprTermLst.Add(new AGExprTerm(xConstCoeff, "C"));
            }
            else
            {
                AGExprTermLst.Add(new AGExprTerm(circle.A, "XX"));
            }

            double yConstCoeff = 0.0;
            if (!circle.E.Equals(0.0)) // has y coefficient
            {
                isFactorized = true;
                yConstCoeff = Math.Pow((Math.Abs(circle.E) * 0.5), 2.0);
                AGExprTermLst.Add(new AGExprTerm(circle.B, "YY"));
                AGExprTermLst.Add(new AGExprTerm(circle.E, "Y"));
                AGExprTermLst.Add(new AGExprTerm(yConstCoeff, "C"));
            }
            else
            {
                AGExprTermLst.Add(new AGExprTerm(circle.B, "YY"));
            }

            if (!circle.D.Equals(0.0))
            {
                AGExprTermLst.Add(new AGExprTerm(-1.0 * xConstCoeff, "C"));
            }

            if (!circle.E.Equals(0.0))
            {
                AGExprTermLst.Add(new AGExprTerm(-1.0 * yConstCoeff, "C"));
            }

            if (!circle.F.Equals(0.0))
            {
                AGExprTermLst.Add(new AGExprTerm(circle.F, "C"));
            }

            TransformFromListOfAGExpr();
            return isFactorized;
        }

        /// <summary>
        /// Step 3 Merge
        /// </summary>
        /// <param name="circle"></param>
        /// <returns></returns>
        private bool MergeCircleAdditionalConstants(Circle circle)
        {
            if (!circle.D.Equals(0.0) && !circle.E.Equals(0.0))
            {
                var term6 = AGExprTermLst[6] as AGExprTerm;
                var term7 = AGExprTermLst[7] as AGExprTerm;

                if (AGExprTermLst.Count == 8)
                {
                    AGExprTermLst.Remove(term6);
                    AGExprTermLst.Remove(term7);

                    double val = term6.Coefficient.Num + term7.Coefficient.Num;
                    AGExprTermLst.Add(new AGExprTerm(val, "C"));
                    TransformFromListOfAGExpr();
                    return true;
                }
                else
                {
                    var term8 = AGExprTermLst[8] as AGExprTerm;

                    AGExprTermLst.Remove(term6);
                    AGExprTermLst.Remove(term7);
                    AGExprTermLst.Remove(term8);

                    double val = term6.Coefficient.Num + term7.Coefficient.Num + term8.Coefficient.Num;
                    AGExprTermLst.Add(new AGExprTerm(val, "C"));
                    TransformFromListOfAGExpr();
                    return true;
                }

            }
            else if (circle.D.Equals(0.0) && !circle.E.Equals(0.0))
            {
                if (AGExprTermLst.Count == 6)
                {
                    var term5 = AGExprTermLst[4] as AGExprTerm;
                    var term6 = AGExprTermLst[5] as AGExprTerm;
                    AGExprTermLst.Remove(term5);
                    AGExprTermLst.Remove(term6);

                    double val = term6.Coefficient.Num + term5.Coefficient.Num;
                    AGExprTermLst.Add(new AGExprTerm(val, "C"));
                    TransformFromListOfAGExpr();
                    return true;
                }
                else // 5 items
                {
                    return false;
                }
            }
            else if (!circle.D.Equals(0.0) && circle.E.Equals(0.0))
            {
                if (AGExprTermLst.Count == 6)
                {
                    var term5 = AGExprTermLst[4] as AGExprTerm;
                    var term6 = AGExprTermLst[5] as AGExprTerm;
                    AGExprTermLst.Remove(term5);
                    AGExprTermLst.Remove(term6);

                    double val = term6.Coefficient.Num + term5.Coefficient.Num;
                    AGExprTermLst.Add(new AGExprTerm(val, "C"));
                    TransformFromListOfAGExpr();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Step 4 Factorization
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="circle"></param>
        /// <param name="outputExpr"></param>
        /// <returns></returns>
        private bool FactorizeCircle(Expr expr, Circle circle, out Expr outputExpr)
        {
            bool isChanged = false;

            Expr[] exprs = AGShapeUtils.ExtractTerms(expr);

            var outExprs = new List<Expr>();

            bool isXFactorized = false;
            if (!circle.D.Equals(0.0)) // X factorize
            {
                isXFactorized = true;
                outExprs.Add(ComposePowerTerm('x', circle.D));
            }
            else
            {
                var composite = new CompositeExpr(WellKnownSym.power, new Expr[]
                {
                    new LetterSym('x'),
                    new IntegerNumber(2) 
                });
                outExprs.Add(composite);
            }

            bool isYFactorized = false;
            if (!circle.E.Equals(0.0)) // y factorize
            {
                isYFactorized = true;
                outExprs.Add(ComposePowerTerm('y', circle.E));
            }
            else
            {
                var composite = new CompositeExpr(WellKnownSym.power, new Expr[]
                {
                    new LetterSym('y'),
                    new IntegerNumber(2) 
                });
                outExprs.Add(composite);
            }

            if (isXFactorized ^ isYFactorized)
            {
                if (exprs.Length == 5)
                {
                    outExprs.Add(exprs[4]);
                }
                isChanged = true;
            }
            else
            {
                if (isXFactorized & isYFactorized)
                {
                    if (exprs.Length == 7)
                    {
                        outExprs.Add(exprs[6]);
                    }
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                outputExpr = new CompositeExpr(WellKnownSym.plus, outExprs.ToArray());
                outputExpr = new CompositeExpr(WellKnownSym.equals, new Expr[]
                {
                    outputExpr, 
                    new IntegerNumber(0) 
                });
            }
            else
            {
                outputExpr = expr;
            }

            return isChanged;
        }

        /// <summary>
        /// Circle and Ellipse  
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="outputExpr"></param>
        /// <returns></returns>
        private bool MoveConstantFromLeftToRight(Expr expr, out Expr outputExpr)
        {
            Expr[] exprs = AGShapeUtils.ExtractTerms(expr);

            if (exprs.Length == 3)
            {
                outputExpr = new CompositeExpr(WellKnownSym.plus, new Expr[]
                {
                    exprs[0],
                    exprs[1]
                });

                var removeMinus = exprs[2] as CompositeExpr;
                if (removeMinus == null || !removeMinus.Head.Equals(WellKnownSym.minus))
                {
                    throw new Exception("AGUnifier.MoveConstantFromLeftToRight");
                }

                outputExpr = new CompositeExpr(WellKnownSym.equals, new Expr[]
                {
                    outputExpr, 
                    removeMinus.Args[0]
                });

                return true;
            }
            else
            {
                outputExpr = null;
                return false;
            }
        }

        /// <summary>
        /// Step 6 Circle Square Number Repr
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="outputExpr"></param>
        /// <returns></returns>
        private bool ShowSquareRootForm(Expr expr, out Expr outputExpr)
        {
            outputExpr = expr.Clone();
            var compositeExpr = outputExpr as CompositeExpr;
            var doubleNum = compositeExpr.Args[1] as IntegerNumber;

            double radius = Math.Sqrt(double.Parse(doubleNum.Num.ToString()));

            Number numExpr;
            if ((radius % 1).Equals(0))
            {
                numExpr = new IntegerNumber(Int32.Parse(radius.ToString()).ToString());
            }
            else
            {
                numExpr = new DoubleNumber(radius);
            }
            var substituteExpr = new CompositeExpr(WellKnownSym.power, new Expr[]
            {
               numExpr,
               new IntegerNumber(2) 
            });

            compositeExpr.Args[1] = substituteExpr;
            return true;
        }

        #endregion

        #region Utilities

        private Expr ComposePowerTerm(char name, double coefficient)
        {
            CompositeExpr expr;
            if (coefficient < 0)
            {
                expr = new CompositeExpr(WellKnownSym.minus, new Expr[]
               {
                  new LetterSym(name),
                  new DoubleNumber(Math.Abs(coefficient) * 0.5) 
               });
            }
            else
            {
                double num = Math.Abs(coefficient) * 0.5;

                Number numExpr;

                if ((num % 1).Equals(0))
                {
                    numExpr = new IntegerNumber(Int32.Parse(num.ToString()).ToString());
                }
                else
                {
                    numExpr = new DoubleNumber(num);
                }

                expr = new CompositeExpr(WellKnownSym.plus, new Expr[]
               {
                  new LetterSym(name),
                  numExpr
               });
            }

            expr = new CompositeExpr(WellKnownSym.power, new Expr[]
            {
                expr,
                new IntegerNumber(2), 
            });

            return expr;
        }

        #endregion

        #region Expr and Shape bilateral translation

        /// <summary>
        /// Parse Hash Table, Generate the corresponding ShapeExpr
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private ShapeExpr DetectKnowledgeShape(Expr expr)
        {
            if (EqCoefficientDict["XX"].Count == 0 &&
                EqCoefficientDict["YY"].Count == 0 &&
                EqCoefficientDict["XY"].Count == 0)
            {
                var aList = EqCoefficientDict["X"] as List<AGExprTermAttribute>;
                var bList = EqCoefficientDict["Y"] as List<AGExprTermAttribute>;
                if (aList.Count == 0 || bList.Count == 0) throw new Exception("Error to generate the knowledge.");
                double aExpr = aList[0].Coefficient.Num;
                double bExpr = bList[0].Coefficient.Num;
                var cList = EqCoefficientDict["C"] as List<AGExprTermAttribute>;
                double cExpr = 0.0;
                if (cList.Count != 0)
                {
                    cExpr = cList[0].Coefficient.Num;
                }

                return new LineExpr(new Line(aExpr, bExpr, cExpr), expr);
            }

            if (EqCoefficientDict["XX"].Count != 0 &&
                EqCoefficientDict["YY"].Count != 0 &&
                EqCoefficientDict["XY"].Count == 0)
            {
                var xxCoeffLst = EqCoefficientDict["XX"] as List<AGExprTermAttribute>;
                var xxCoeffExpr = xxCoeffLst[0];
                var yyCoeffLst = EqCoefficientDict["YY"] as List<AGExprTermAttribute>;
                var yyCoeffExpr = yyCoeffLst[0];
                var xCoeffLst = EqCoefficientDict["X"] as List<AGExprTermAttribute>;
                var xCoeffExpr = xCoeffLst.Count != 0 ? xCoeffLst[0] : null;
                var yCoeffLst = EqCoefficientDict["Y"] as List<AGExprTermAttribute>;
                var yCoeffExpr = yCoeffLst.Count != 0 ? yCoeffLst[0] : null;
                var constCoeffLst = EqCoefficientDict["C"] as List<AGExprTermAttribute>;
                var constCoeffExpr = constCoeffLst.Count != 0 ? constCoeffLst[0] : null;

                double xx = xxCoeffExpr != null ? xxCoeffExpr.Coefficient.Num : 0.0;
                double yy = yyCoeffExpr != null ? yyCoeffExpr.Coefficient.Num : 0.0;
                double x = xCoeffExpr != null ? xCoeffExpr.Coefficient.Num : 0.0;
                double y = yCoeffExpr != null ? yCoeffExpr.Coefficient.Num : 0.0;
                double c = constCoeffExpr != null ? constCoeffExpr.Coefficient.Num : 0.0;

                if (xx.Equals(yy))
                {
                    return new CircleExpr(new Circle(xx, yy, x, y, c), expr);
                }
                else
                {
                    return new EllipseExpr(new Ellipse(xx, yy, x, y, c), expr);
                }
            }

            return null;
        }

        /// <summary>
        /// Generate Expr from Hash Table.
        /// </summary>
        /// <returns></returns>
        private Expr GenerateExpr()
        {
            var exprs = new List<Expr>();
            for (int i = 0; i < _currentIndex; i++)
            {
                AGExprTermAttribute term;
                string name;
                if (FindTerm(i, out name, out term))
                {
                    exprs.Add(ConstructExpr(name, term.Coefficient));
                }
                else
                {
                    throw new Exception("Generate Expr Exception");
                }
            }
            var compositeExpr = new CompositeExpr(WellKnownSym.plus, exprs.ToArray());
            compositeExpr = new CompositeExpr(WellKnownSym.equals, new Expr[]
            {
                compositeExpr,
                new IntegerNumber(0) 
            });
            return compositeExpr;
        }

        private bool FindTerm(int index, out string name, out AGExprTermAttribute term)
        {
            foreach (KeyValuePair<string, List<AGExprTermAttribute>> pair in EqCoefficientDict)
            {
                string tempName = pair.Key;
                foreach (AGExprTermAttribute attribute in pair.Value)
                {
                    if (attribute.IndexOfExpr != index) continue;
                    name = tempName;
                    term = attribute;
                    return true;
                }
            }

            name = null;
            term = null;
            return false;
        }

        private Expr ConstructExpr(string name, DoubleNumber coefficient)
        {
            Expr expr;
            if (name.Equals("C"))
            {
                double number = coefficient.Num;
                if ((number % 1).Equals(0)) // integer number
                {
                    string integer = Int32.Parse(number.ToString()).ToString();

                    if (coefficient.Num < 0.0)
                    {
                        var integerExpr = new IntegerNumber(Math.Abs(coefficient.Num).ToString());
                        expr = new CompositeExpr(WellKnownSym.minus, new Expr[]
                        {
                            integerExpr
                        });
                    }
                    else
                    {
                        expr = new IntegerNumber(integer);
                    }
                }
                else //double number
                {
                    if (coefficient.Num < 0.0)
                    {
                        expr = new DoubleNumber(Math.Abs(coefficient.Num));
                        expr = new CompositeExpr(WellKnownSym.minus, new Expr[]
                        {
                            expr
                        });
                    }
                    else
                    {
                        expr = coefficient;
                    }
                }
            }
            else if (name.Equals("X"))
            {
                bool isNegative = coefficient.Num < 0.0;
                bool isCoefficientOne = Math.Abs(coefficient.Num).Equals(1.0);

                var build = new StringBuilder();
                build.Append(Math.Abs(coefficient.Num).ToString());
                build.Append("x");

                if (isCoefficientOne)
                {
                    expr = new LetterSym('x');
                }
                else
                {
                    expr = new WordSym(build.ToString());
                }

                if (isNegative)
                {
                    expr = new CompositeExpr(WellKnownSym.minus, new Expr[]
                    {
                        expr
                    });
                }
            }
            else if (name.Equals("Y"))
            {
                bool isNegative = coefficient.Num < 0.0;
                bool isCoefficientOne = Math.Abs(coefficient.Num).Equals(1.0);

                var build = new StringBuilder();
                build.Append(Math.Abs(coefficient.Num).ToString());
                build.Append("y");

                if (isCoefficientOne)
                {
                    expr = new LetterSym('y');
                }
                else
                {
                    expr = new WordSym(build.ToString());
                }

                if (isNegative)
                {
                    expr = new CompositeExpr(WellKnownSym.minus, new Expr[]
                    {
                        expr
                    });
                }
            }
            else if (name.Equals("XY"))
            {
                bool isNegative = coefficient.Num < 0.0;
                bool isCoefficientOne = Math.Abs(coefficient.Num).Equals(1.0);

                var build = new StringBuilder();
                build.Append(Math.Abs(coefficient.Num).ToString());
                build.Append("xy");

                if (isCoefficientOne)
                {
                    expr = new WordSym("xy");
                }
                else
                {
                    expr = new WordSym(build.ToString());
                }

                if (isNegative)
                {
                    expr = new CompositeExpr(WellKnownSym.minus, new Expr[]
                    {
                        expr
                    });
                }
            }
            else if (name.Equals("XX"))
            {
                bool isNegative = coefficient.Num < 0.0;
                bool isCoefficientOne = Math.Abs(coefficient.Num).Equals(1.0);

                var build = new StringBuilder();
                build.Append(Math.Abs(coefficient.Num).ToString());
                build.Append("x");

                if (isCoefficientOne)
                {
                    expr = new LetterSym('x');
                }
                else
                {
                    expr = new WordSym(build.ToString());
                }

                expr = new CompositeExpr(WellKnownSym.power, new Expr[]
                {
                   expr,
                   new IntegerNumber(2) 
                });

                if (isNegative)
                {
                    expr = new CompositeExpr(WellKnownSym.minus, new Expr[]
                    {
                        expr
                    });
                }
            }
            else if (name.Equals("YY"))
            {
                bool isNegative = coefficient.Num < 0.0;
                bool isCoefficientOne = Math.Abs(coefficient.Num).Equals(1.0);

                var build = new StringBuilder();
                build.Append(Math.Abs(coefficient.Num).ToString());
                build.Append("y");

                if (isCoefficientOne)
                {
                    expr = new LetterSym('y');
                }
                else
                {
                    expr = new WordSym(build.ToString());
                }

                expr = new CompositeExpr(WellKnownSym.power, new Expr[]
                {
                   expr,
                   new IntegerNumber(2) 
                });

                if (isNegative)
                {
                    expr = new CompositeExpr(WellKnownSym.minus, new Expr[]
                    {
                        expr
                    });
                }
            }
            else
            {
                throw new Exception("AGUnifier.ConstructExpr Error");
            }

            return expr;
        }

        #endregion

        #region AGExprTerm and AGExprTermAttribute Transformations

        /// <summary>
        /// Transform from IDictionary<string, List<AGExprTermAttribute>> to List<AGExprTerm>
        /// Transfrom to List<AGExprTerm>
        /// </summary>
        /// <returns></returns>
        void TransformToListOfAGExpr()
        {
            AGExprTermLst = new List<AGExprTerm>();

            string name;
            AGExprTermAttribute termAttri;
            for (int i = 0; i < _currentIndex; i++)
            {
                FindTerm(i, out name, out termAttri);
                AGExprTermLst.Add(new AGExprTerm(termAttri.Coefficient, name));
            }
        }

        /// <summary>
        /// Transform from List<AGExprTerm>
        /// Transform to IDictionary<string, List<AGExprTermAttribute>> to List<AGExprTerm>
        /// </summary>
        void TransformFromListOfAGExpr()
        {
            _currentIndex = AGExprTermLst.Count;

            EqCoefficientDict = new Dictionary<string, List<AGExprTermAttribute>>
            {
                {"XX", new List<AGExprTermAttribute>()},
                {"X",  new List<AGExprTermAttribute>()},
                {"YY", new List<AGExprTermAttribute>()},
                {"Y",  new List<AGExprTermAttribute>()},
                {"XY", new List<AGExprTermAttribute>()},
                {"C",  new List<AGExprTermAttribute>()}
            };

            for (int i = 0; i < AGExprTermLst.Count; i++)
            {
                EqCoefficientDict[AGExprTermLst[i].Variables].Add(new AGExprTermAttribute(AGExprTermLst[i].Coefficient, i));
            }
        }

        #endregion

        #endregion

        #region Geometry Input Unification

        public List<AGKnowledgeTracer> RetrieveLineTracers(ShapeExpr se)
        {
            var tracers = new List<AGKnowledgeTracer>();

            var le = se as LineExpr;

            string strategy = AGStrategyHint.QueryLineStandardFormStrategyFromGeometry;
            string why1 = "Substitute two points into the general line equation.";

            Expr how11 = starPadSDK.MathExpr.Text.Convert("a×2 + b×0 + c = 0 ");
            Expr how12 = starPadSDK.MathExpr.Text.Convert("a×0 + b×3 + c = 0 ");

            Expr how1 = new CompositeExpr(new WordSym(""), new Expr[] { how11, how12 });
            string appliedRule1 = AGAppliedRule.AlgebraSubsitituion;
            var tracer = new AGKnowledgeTracer(se.GeneralExpr, how1, "hello world", strategy, appliedRule1);
            tracers.Add(tracer);

            var why2 = "Eliminate the term with 0 coefficients.";
            Expr how21 = starPadSDK.MathExpr.Text.Convert("2×a + c = 0");
            Expr how22 = starPadSDK.MathExpr.Text.Convert("3×b + c = 0");

            Expr how2 = new CompositeExpr(new WordSym(""), new Expr[] { how21, how22 });
            string appliedRule2 = AGAppliedRule.AlgebraElimination;
            tracer = new AGKnowledgeTracer(how1, how2, why1, strategy, appliedRule2);
            tracers.Add(tracer);

            var why3 = "Let c = -6, which is the least common multiplier between coefficients of a and b, calculate a and b.";
            Expr how31 = starPadSDK.MathExpr.Text.Convert("2×a = 6");
            Expr how32 = starPadSDK.MathExpr.Text.Convert("a = 3");
            Expr how33 = starPadSDK.MathExpr.Text.Convert("3×b = 6");
            Expr how34 = starPadSDK.MathExpr.Text.Convert("b = 2");

            Expr how3 = new CompositeExpr(new WordSym(""), new Expr[] { how31, how32, how33, how34 });
            string appliedRule3 = "Find least common multiplier between coefficients of a and b";
            tracer = new AGKnowledgeTracer(how2, how3, why2, strategy, appliedRule3);
            tracers.Add(tracer);

            var why4 = "Substitute the value of a, b, c into the general form of line.";
            Expr how41 = starPadSDK.MathExpr.Text.Convert("3x + 2y – 6 = 0");

            //Expr how4 = new CompositeExpr(new WordSym("L:"), new Expr[]{how41});
            Expr how4 = new CompositeExpr(new WordSym("L:"), new Expr[] { how41 });
            string appliedRule4 = AGAppliedRule.FitABCToGeneralForm;
            //tracer = new AGKnowledgeTracer(how3, how4, why3, strategy, appliedRule4);
            tracer = new AGKnowledgeTracer(how3, how41, why3, strategy, appliedRule4);
            tracers.Add(tracer);

            tracer = new AGKnowledgeTracer(how4, null, why4, strategy, "");
            tracers.Add(tracer);

            return tracers;
        }

        public List<AGKnowledgeTracer> RetrieveLineTracers2(IKnowledgeExpr se, Expr fakeExpr)
        {
            var tracers = new List<AGKnowledgeTracer>();

            var le = se as LineExpr;

            string strategy = AGStrategyHint.QueryLineStandardFormStrategyFromGeometry;
            string why1 = "Substitute two points into the general line equation.";

            Expr how11 = starPadSDK.MathExpr.Text.Convert("a×2 + b×0 + c = 0 ");
            Expr how12 = starPadSDK.MathExpr.Text.Convert("a×0 + b×3 + c = 0 ");

            Expr how1 = new CompositeExpr(new WordSym(""), new Expr[] { how11, how12 });
            string appliedRule1 = AGAppliedRule.AlgebraSubsitituion;
            var tracer = new AGKnowledgeTracer(se.GeneralExpr, how1, "hello world", strategy, appliedRule1);
            tracers.Add(tracer);

            var why2 = "Eliminate the term with 0 coefficients.";
            Expr how21 = starPadSDK.MathExpr.Text.Convert("2×a + c = 0");
            Expr how22 = starPadSDK.MathExpr.Text.Convert("3×b + c = 0");

            Expr how2 = new CompositeExpr(new WordSym(""), new Expr[] { how21, how22 });
            string appliedRule2 = AGAppliedRule.AlgebraElimination;
            tracer = new AGKnowledgeTracer(how1, how2, why1, strategy, appliedRule2);
            tracers.Add(tracer);

            var why3 = "Let c = -6, which is the least common multiplier between coefficients of a and b, calculate a and b.";
            Expr how31 = starPadSDK.MathExpr.Text.Convert("2×a = 6");
            Expr how32 = starPadSDK.MathExpr.Text.Convert("a = 3");
            Expr how33 = starPadSDK.MathExpr.Text.Convert("3×b = 6");
            Expr how34 = starPadSDK.MathExpr.Text.Convert("b = 2");

            Expr how3 = new CompositeExpr(new WordSym(""), new Expr[] { how31, how32, how33, how34 });
            string appliedRule3 = "Find least common multiplier between coefficients of a and b";
            tracer = new AGKnowledgeTracer(how2, how3, why2, strategy, appliedRule3);
            tracers.Add(tracer);

            var why4 = "Substitute the value of a, b, c into the general form of line.";
            Expr how41 = starPadSDK.MathExpr.Text.Convert("3x + 2y – 6 = 0");

            //Expr how4 = new CompositeExpr(new WordSym("L:"), new Expr[]{how41});
            Expr how4 = new CompositeExpr(new WordSym("L:"), new Expr[] { how41 });
            string appliedRule4 = AGAppliedRule.FitABCToGeneralForm;
            //tracer = new AGKnowledgeTracer(how3, how4, why3, strategy, appliedRule4);
            tracer = new AGKnowledgeTracer(how3, how4, why3, strategy, appliedRule4);
            tracers.Add(tracer);

            tracer = new AGKnowledgeTracer(how4, fakeExpr, why4, strategy, "");
            tracers.Add(tracer);


            return tracers;
        }

        public List<AGKnowledgeTracer> RetrieveDistanceTracers()
        {
            var tracers = new List<AGKnowledgeTracer>();
            var strategy = AGStrategyHint.DistanceBetweenPointAndLine;

            var why0 = "From Circle and Line Relationship, " +
                       "\nyou query the distance bewtween point and line.";

            var how01 = new CompositeExpr(new WordSym("L:"), 
                                        new Expr[] { starPadSDK.MathExpr.Text.Convert("x+y-1") });

            var how00 = new CompositeExpr(new WordSym("P:"), 
                                        new Expr[]{new IntegerNumber(-1), new IntegerNumber(-1)});

            var how0 = new CompositeExpr(new WordSym(""),
                                        new Expr[] {how00, how01});

            var tracer0 = new AGKnowledgeTracer(new WordSym("CircleLine"), how0, why0, strategy, "Knolwedge Extraction");
            tracers.Add(tracer0);

            var why1 = "The distance between a point (X0,Y0) and a line ax+by+c = 0 is" +
                       "\n (|aX0 + bY0 + c| divide by Math.Sqrt(a^2 + b^2) )";
            var how1 = new CompositeExpr(new WordSym(""), new Expr[]
            {
                starPadSDK.MathExpr.Text.Convert("X0 ＝ -1"),
                starPadSDK.MathExpr.Text.Convert("Y0 ＝ -1"),
                starPadSDK.MathExpr.Text.Convert("a ＝ 1"),
                starPadSDK.MathExpr.Text.Convert("b ＝ 1"),
                starPadSDK.MathExpr.Text.Convert("c ＝ -1")
            });
            string appliedRule1 = "Extract Coefficients from Expressions.";
            var tracer1 = new AGKnowledgeTracer(how0, how1, why1, strategy, appliedRule1);
            tracers.Add(tracer1);

            var why2 = "(X0,Y0) is the coordinate point, a, b, c are coefficients of the line.";
/*
            var how20 = starPadSDK.MathExpr.Text.Convert("A＝｜aX0 + bYo + c｜");
            var how21 = starPadSDK.MathExpr.Text.Convert("A＝｜1 ×(-1)+ 1 ×(-1)-1｜");
            var how22 = starPadSDK.MathExpr.Text.Convert("A＝3");
 */ 
            //var how2 = new CompositeExpr(new WordSym(""), new Expr[]{ how20, how21, how22});
            var how20 = starPadSDK.MathExpr.Text.Convert("A＝｜aX0 + bYo + c｜");
            string appliedRule2 = "Substitute values in divisor and absolute value.";
            var tracer20 = new AGKnowledgeTracer(how1, how20, why2, strategy, appliedRule2);
            tracers.Add(tracer20);
    
            var how21 = starPadSDK.MathExpr.Text.Convert("A＝｜1 ×（-1）+ 1 ×（-1）-1｜");
            var tracer21 = new AGKnowledgeTracer(how20, how21, why2, strategy, appliedRule2);
            tracers.Add(tracer21);

            var how2 = starPadSDK.MathExpr.Text.Convert("A＝3");
            var tracer22 = new AGKnowledgeTracer(how21, how2, why2, strategy, appliedRule2);
            tracers.Add(tracer22);

            var why3 = "Algebraic Expression Manipulation and Calculation.";
            var how30 = new CompositeExpr(WellKnownSym.root, new Expr[]
            {
                new IntegerNumber(2), 
                new CompositeExpr(WellKnownSym.plus, new Expr[]
                {
                    new CompositeExpr(WellKnownSym.power, new Expr[]
                    {
                         new LetterSym('a'),
                         new IntegerNumber(2), 
                    }),
                    new CompositeExpr(WellKnownSym.power, new Expr[]
                    {
                         new LetterSym('b'),
                         new IntegerNumber(2), 
                    }) 
                })                
            });
            how30 = new CompositeExpr(WellKnownSym.equals, new Expr[]
            {
                new LetterSym('B'),
                how30
            });
              
            //var how3 = starPadSDK.MathExpr.Text.Convert("B ＝〔a∧2 + b∧2〕→ √〔1 + 1〕→ √〔2〕");
            string appliedRule3 = "substitute values in dividend and absolute value.";
            var tracer3 = new AGKnowledgeTracer(how2, how30, why3, strategy, appliedRule3);
            tracers.Add(tracer3);

            var how31 = new CompositeExpr(WellKnownSym.root, new Expr[]
            {
                new IntegerNumber(2), 
                new CompositeExpr(WellKnownSym.plus, new Expr[]
                {
                   new IntegerNumber(1),
                   new IntegerNumber(1) 
                })                
            });

            how31 = new CompositeExpr(WellKnownSym.equals, new Expr[]
            {
                new LetterSym('B'),
                how31
            });

            tracer3 = new AGKnowledgeTracer(how30, how31, why3, strategy, appliedRule3);
            tracers.Add(tracer3);

            var how3 = starPadSDK.MathExpr.Text.Convert("B ＝ √2");
            tracer3 = new AGKnowledgeTracer(how31, how3, why3, strategy, appliedRule3);
            tracers.Add(tracer3);

            var why4 = "D = A/B";           
            var how4 = new CompositeExpr(WellKnownSym.equals, new Expr[]
            {
                new LetterSym('D'), 
                new CompositeExpr(WellKnownSym.times, new Expr[]
                {
                    new CompositeExpr(WellKnownSym.times, new Expr[]
                    {
                        new IntegerNumber(3),
                        new CompositeExpr(WellKnownSym.root, new Expr[]
                        {
                            new IntegerNumber(2),
                            new IntegerNumber(2), 
                        }) 
                    }),
                    new CompositeExpr(WellKnownSym.divide, new Expr[]
                    {
                        new IntegerNumber(2), 
                    }), 
                })
            });
                
            //starPadSDK.MathExpr.Text.Convert("D ＝ 3 × √〔2〕／2");

            var appliedRule4 = "calculate distance value";
            var tracer4 = new AGKnowledgeTracer(how3, how4, why4, strategy, appliedRule4);
            tracers.Add(tracer4);

            return tracers;
        }

        public List<AGKnowledgeTracer> RetrieveDistanceTracers2()
        {
            var tracers = new List<AGKnowledgeTracer>();
            var strategy = AGStrategyHint.DistanceBetweenTwoPoints;

            var why0 = "Substitute two points into the equation (x0-x1)^2 + (y0-y1)^2 = d^2";
            var how0 = new CompositeExpr(new WordSym("d:"),
                                        new Expr[] { starPadSDK.MathExpr.Text.Convert("(3-6)^2+(4-v)^2=5^2") });
            var tracer0 = new AGKnowledgeTracer(new WordSym("Distance"), how0, why0, strategy, "Apply Substitution Rule.");
            tracers.Add(tracer0);

            var how1 = new CompositeExpr(new WordSym("d:"),
                                        new Expr[] { starPadSDK.MathExpr.Text.Convert("(-3)^2+(4-v)^2=5^2") });
            var why1 = "Arithmetic Calculuation";
            var tracer1 = new AGKnowledgeTracer(how0, how1, why1, strategy, "Calculation");
            tracers.Add(tracer1);

            var how2 = new CompositeExpr(new WordSym("d:"),
                                        new Expr[] { starPadSDK.MathExpr.Text.Convert("9+(4-v)^2=25") });
            var why2 = "Arithmetic Calculuation";
            var tracer2 = new AGKnowledgeTracer(how1, how2, why2, strategy, "Exponent Calculation");
            tracers.Add(tracer2);

            var how3 = new CompositeExpr(new WordSym("d:"),
                                        new Expr[] { starPadSDK.MathExpr.Text.Convert("(4-v)^2=16") });
            var why3 = "Arithmetic Calculuation";
            var tracer3 = new AGKnowledgeTracer(how2, how3, why3, strategy, "Merge Terms.");
            tracers.Add(tracer3);

            var how4 = new CompositeExpr(new WordSym(""), new Expr[] { starPadSDK.MathExpr.Text.Convert("v=0"), starPadSDK.MathExpr.Text.Convert("v=8") });
            var why4 = "Arithmetic Calculuation";
            //
            var tracer4 = new AGKnowledgeTracer(how3, how4, why4, strategy, "Square Root Calculation.");
            tracers.Add(tracer4);
           
            var tracer6 = new AGKnowledgeTracer(how4, null, "", strategy, "");
            tracers.Add(tracer6);

            return tracers;
        }


        #endregion
    }

    public class AGExprTerm : IComparable
    {
        public DoubleNumber Coefficient { get; set; }
        public string Variables { get; set; }

        public AGExprTerm(DoubleNumber _number, string _variable)
        {
            Coefficient = _number;
            Variables = _variable;
        }

        public int CompareTo(object obj)
        {
            var term = obj as AGExprTerm;
            if (term == null) return 1;
            if (Variables.Equals(term.Variables))
            {
                return 0;
            }
            else
            {
                if (Variables.Equals("XX"))
                {
                    return -1;
                }
                else if (Variables.Equals("X"))
                {
                    if (term.Variables.Equals("XX"))
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else if (Variables.Equals("YY"))
                {
                    if (term.Variables.Equals("XX") || term.Variables.Equals("X"))
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else if (Variables.Equals("Y"))
                {
                    if (term.Variables.Equals("XX") || term.Variables.Equals("X") || term.Variables.Equals("YY"))
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return 1;
                }
            }

        }
    }

    public class AGExprTermAttribute
    {
        public DoubleNumber Coefficient { get; set; }

        public int IndexOfExpr { get; set; }

        public AGExprTermAttribute(DoubleNumber _coeff, int _index)
        {
            Coefficient = _coeff;
            IndexOfExpr = _index;
        }
    }
}