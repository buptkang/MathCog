// /*******************************************************************************
//  * Analytical Geometry Semantic Parsing 
//  * <p>
//  * Copyright (C) 2014  Bo Kang, Hao Hu
//  * <p>
//  * This program is free software; you can redistribute it and/or modify it under
//  * the terms of the GNU General Public License as published by the Free Software
//  * Foundation; either version 2 of the License, or any later version.
//  * <p>
//  * This program is distributed in the hope that it will be useful, but WITHOUT
//  * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//  * FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
//  * details.
//  * <p>
//  * You should have received a copy of the GNU General Public License along with
//  * this program; if not, write to the Free Software Foundation, Inc., 51
//  * Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
//  ******************************************************************************/

using System;
using System.Data;
using ExprSemantic.KnowledgeBase;
using Microsoft.FSharp.Text.Lexing;
using starPadSDK.MathExpr.TextInternals;

namespace AGSemantic.KnowledgeBase
{
    using System.Collections.Generic;
    using System.Linq;
    using starPadSDK.MathExpr;

    public class AGShapeExprMatcher
    {
        #region Singleton

        private static AGShapeExprMatcher _matcher;

        public static AGShapeExprMatcher Instance
        {
            get { return _matcher ?? (_matcher = new AGShapeExprMatcher()); }
        }

        private AGShapeExprMatcher()
        {
            InitShapeTemplates();
        }

        private void InitShapeTemplates()
        {
            PointTemplates = AGTemplateExpressions.PointTemplates.Select(Text.Convert).ToList();
            LineImplicitTemplate = Text.Convert(AGTemplateExpressions.LineImplicitTemplate);
            CircleImplicitTemplate = Text.Convert(AGTemplateExpressions.CircleImplicitTemplate);
            EllipseImplicitTemplate = Text.Convert(AGTemplateExpressions.EllipseImplicitTemplate);
        }

        #endregion

        #region Properties

        public List<Expr> PointTemplates;
        public Expr LineImplicitTemplate;
        public Expr CircleImplicitTemplate;
        public Expr EllipseImplicitTemplate;
        public Expr HyperbolaImplicitTemplate;
        public Expr ParabolaImplicitTemplate;

        #endregion

        #region Method

        public ShapeExpr MatchExpr(Expr expr)
        {
            ShapeExpr matchedShape;
            if (MatchLineImplicitTemplate(expr, out matchedShape))
            {
                return matchedShape;
            }
            else if (MatchCircleImplicitTemplate(expr, out matchedShape))
            {
                return matchedShape;
            }
            else if (MatchEllipseImplicitTemplate(expr, out matchedShape))
            {
                return matchedShape;
            }
            else
            {
                return null;
            }
        }

        #region Line Matching

        private bool MatchLineImplicitTemplate(Expr expr, out ShapeExpr shape)
        {
            if (!(expr is CompositeExpr))
            {
                shape = null;
                return false;
            }
            var compositeExpr         = expr as CompositeExpr;
            var compositeExprTemplate = LineImplicitTemplate as CompositeExpr;

            if (!compositeExprTemplate.Head.Equals(compositeExpr.Head)
                || compositeExprTemplate.Args.Length != compositeExpr.Args.Length)
            {
                shape = null;
                return false;                
            }

            if (!(compositeExpr.Args[1] is IntegerNumber) || (!(compositeExpr.Args[0] is CompositeExpr))) 
            {
                shape = null;
                return false;                                
            }

            var number = compositeExpr.Args[1] as IntegerNumber;
            if (number.Num != 0)
            {
                shape = null;
                return false;                                                
            }

            var termsTemplate = compositeExprTemplate.Args[0] as CompositeExpr;
            var terms         = compositeExpr.Args[0] as CompositeExpr;

            if (!(termsTemplate.Head.Equals(terms.Head) 
                && termsTemplate.Args.Length.Equals(terms.Args.Length)))
            {
                shape = null;
                return false;                                 
            }

            double xValue, yValue, cValue;            
            if (RetrieveXTermCoefficient(terms.Args[0], out xValue) &&
                RetrieveYTermCoefficient(terms.Args[1], out yValue) &&
                RetrieveConstantCoefficient(terms.Args[2], out cValue))
            {
                var line = new Line(xValue, yValue, cValue);
                shape    = new LineExpr(expr, line);
                return true;
            }
            else
            {
                shape = null;
                return false;                
            }
        }

        public bool RetrieveXTermCoefficient(Expr expr, out double xCoefficient)
        {
            if (!(expr is CompositeExpr))
            {
                xCoefficient = 0.0;
                return false;
            }

            bool isNegative = false;
            var compositeExpr = expr as CompositeExpr;
            if (compositeExpr.Head == WellKnownSym.minus)
            {
                isNegative = true;
                if (!(compositeExpr.Args[0] is CompositeExpr))
                {
                    xCoefficient = 0.0;
                    return false;
                }
                compositeExpr = compositeExpr.Args[0] as CompositeExpr;
            }
            else if (compositeExpr.Head == WellKnownSym.times)
            {
                                 
            }
            else
            {
                xCoefficient = 0.0;
                return false;
            }

            if (compositeExpr.Args[1] is LetterSym)
            {
                var letterSym = compositeExpr.Args[1] as LetterSym;
                if (!(letterSym.Letter.ToString().Equals("x") || letterSym.Letter.ToString().Equals("X")))
                {
                    xCoefficient = 0.0;
                    return false;
                }
            }
            else
            {
                xCoefficient = 0.0;
                return false;
            }

            if (compositeExpr.Args[0] is DoubleNumber)
            {
                var doubleExpr = compositeExpr.Args[0] as DoubleNumber;
                xCoefficient = doubleExpr.Num;
                xCoefficient = isNegative ? -1*xCoefficient : xCoefficient;
                return true;
            }
            else if (compositeExpr.Args[0] is IntegerNumber)
            {
                var integerExpr = compositeExpr.Args[0] as IntegerNumber;
                xCoefficient = double.Parse(integerExpr.Num.ToString());
                xCoefficient = isNegative ? -1 * xCoefficient : xCoefficient;
                return true;
            }
            else
            {
                xCoefficient = 0.0;
                return false;
            }
        }

        public bool RetrieveYTermCoefficient(Expr expr, out double yCoefficient)
        {
            if (!(expr is CompositeExpr))
            {
                yCoefficient = 0.0;
                return false;
            }

            bool isNegative = false;
            var compositeExpr = expr as CompositeExpr;
            if (compositeExpr.Head == WellKnownSym.minus)
            {
                isNegative = true;
                if (!(compositeExpr.Args[0] is CompositeExpr))
                {
                    yCoefficient = 0.0;
                    return false;
                }
                compositeExpr = compositeExpr.Args[0] as CompositeExpr;
            }
            else if (compositeExpr.Head == WellKnownSym.times)
            {

            }
            else
            {
                yCoefficient = 0.0;
                return false;
            }

            if (compositeExpr.Args[1] is LetterSym)
            {
                var letterSym = compositeExpr.Args[1] as LetterSym;
                if (!(letterSym.Letter.ToString().Equals("y") || letterSym.Letter.ToString().Equals("Y")))
                {
                    yCoefficient = 0.0;
                    return false;
                }
            }
            else
            {
                yCoefficient = 0.0;
                return false;
            }

            if (compositeExpr.Args[0] is DoubleNumber)
            {
                var doubleExpr = compositeExpr.Args[0] as DoubleNumber;
                yCoefficient = doubleExpr.Num;
                yCoefficient = isNegative ? -1 * yCoefficient : yCoefficient;
                return true;
            }
            else if (compositeExpr.Args[0] is IntegerNumber)
            {
                var integerExpr = compositeExpr.Args[0] as IntegerNumber;
                yCoefficient = double.Parse(integerExpr.Num.ToString());
                yCoefficient = isNegative ? -1 * yCoefficient : yCoefficient;
                return true;
            }
            else
            {
                yCoefficient = 0.0;
                return false;
            }
        }

        public bool RetrieveConstantCoefficient(Expr expr, out double constCoefficient)
        {
            bool isNegative = false;

            if (expr is CompositeExpr)
            {
                var compositeExpr = expr as CompositeExpr;
                if (compositeExpr.Head == WellKnownSym.minus && compositeExpr.Args.Length == 1)
                {
                    isNegative = true;
                    expr = compositeExpr.Args[0];
                }
                else
                {
                    constCoefficient = 0.0;
                    return false;
                }

                if (expr is IntegerNumber)
                {
                    var integerExpr = compositeExpr.Args[0] as IntegerNumber;
                    constCoefficient = double.Parse(integerExpr.Num.ToString());
                    constCoefficient = isNegative ? -1 * constCoefficient : constCoefficient;
                    return true;   
                }
                else if (expr is DoubleNumber)
                {
                    var doubleExpr = compositeExpr.Args[0] as DoubleNumber;
                    constCoefficient = doubleExpr.Num;
                    constCoefficient = isNegative ? -1 * constCoefficient : constCoefficient;
                    return true;
                }
                else
                {
                    constCoefficient = 0.0;
                    return false;
                }
            }else if (expr is IntegerNumber)
            {
                var integerExpr = expr as IntegerNumber;
                constCoefficient = double.Parse(integerExpr.Num.ToString());
                constCoefficient = isNegative ? -1 * constCoefficient : constCoefficient;
                return true;     
            }
            else if (expr is DoubleNumber)
            {
                var doubleExpr = expr as DoubleNumber;
                constCoefficient = doubleExpr.Num;
                constCoefficient = isNegative ? -1 * constCoefficient : constCoefficient;
                return true;               
            }
            else
            {
                constCoefficient = 0.0;
                return false;
            }
        }

        #endregion

        #region Circle Matching

        private bool MatchCircleImplicitTemplate(Expr expr, out ShapeExpr shape)
        {
            if (!(expr is CompositeExpr))
            {
                shape = null;
                return false;
            }
            var compositeExpr = expr as CompositeExpr;

            if (!compositeExpr.Head.Equals(WellKnownSym.equals)
                || compositeExpr.Args.Length != 2)
            {
                shape = null;
                return false;
            }

            Expr leftExpr  = compositeExpr.Args[0];
            Expr rightExpr = compositeExpr.Args[1];

            double radius;
            Point center;
            if (RetrieveCircleRadiusCoefficient(rightExpr, out radius) &&
                RetrieveCirlcePointCoefficient(leftExpr, out center))
            {
                var circle = new Circle(center, radius);
                shape = new CircleExpr(expr, circle);
                return true;
            }
            else
            {
                shape = null;
                return false;
            }
        }

        private bool RetrieveCircleRadiusCoefficient(Expr expr, out double radius)
        {
            if (!(expr is CompositeExpr))
            {
                radius = 0.0;
                return false;
            }

            var compositeExpr = expr as CompositeExpr;
            if (!(compositeExpr.Head.Equals(WellKnownSym.power) &&
                compositeExpr.Args.Length == 2))
            {
                radius = 0.0;
                return false;
            }

            if (!(compositeExpr.Args[1] is IntegerNumber))
            {
                radius = 0.0;
                return false;
            }

            var powerNumber = compositeExpr.Args[1] as IntegerNumber;
            if (!(powerNumber.Num == 2))
            {
                radius = 0.0;
                return false;
            }

            Expr radiusNumber = compositeExpr.Args[0];
            if (radiusNumber is IntegerNumber)
            {
                var radiusInteger = radiusNumber as IntegerNumber;
                radius = double.Parse(radiusInteger.Num.ToString());
            }
            else if (radiusNumber is DoubleNumber)
            {
                var radiusDouble = radiusNumber as DoubleNumber;
                radius = radiusDouble.Num;
            }
            else
            {
                radius = 0.0;
                return false;
            }

            return true;
        }

        private bool RetrieveCirlcePointCoefficient(Expr expr, out Point point)
        {
            if (!(expr is CompositeExpr))
            {
                point = null;
                return false;                
            }

            var compositeExpr = expr as CompositeExpr;
            if (!(compositeExpr.Head.Equals(WellKnownSym.plus) &&
                compositeExpr.Args.Length == 2))
            {
                point = null;
                return false;
            }

            if (!(compositeExpr.Args[0] is CompositeExpr && compositeExpr.Args[1] is CompositeExpr))
            {
                point = null;
                return false;                
            }

            var xPowerTerm = compositeExpr.Args[0] as CompositeExpr;
            var yPowerTerm = compositeExpr.Args[1] as CompositeExpr;

            if (!(xPowerTerm.Head.Equals(WellKnownSym.power) &&
                  yPowerTerm.Head.Equals(WellKnownSym.power) &&
                  xPowerTerm.Args.Length == 2 &&
                  yPowerTerm.Args.Length == 2 ))
            {
                point = null;
                return false;                                
            }

            Expr xPowerExpr = xPowerTerm.Args[1];
            Expr yPowerExpr = yPowerTerm.Args[1];

            if (!(xPowerExpr is IntegerNumber && yPowerExpr is IntegerNumber))
            {
                point = null;
                return false;                                                 
            }

            var xPowerNumber = xPowerExpr as IntegerNumber;
            var yPowerNumber = yPowerExpr as IntegerNumber;
            if (!(xPowerNumber.Num == 2 && yPowerNumber.Num == 2))
            {
                point = null;
                return false;
            }

            Expr xTermExpr = xPowerTerm.Args[0];
            Expr yTermExpr = yPowerTerm.Args[0];

            if (!(xTermExpr is CompositeExpr && yTermExpr is CompositeExpr))
            {
                point = null;
                return false;                                    
            }

            var xTermComposite = xTermExpr as CompositeExpr;
            var yTermComposite = yTermExpr as CompositeExpr;

            if (!(xTermComposite.Head.Equals(WellKnownSym.plus) &&
                  yTermComposite.Head.Equals(WellKnownSym.plus) &&
                  xTermComposite.Args[0] is LetterSym &&
                  yTermComposite.Args[0] is LetterSym))
            {
                point = null;
                return false; 
            }

            var xTermSym = xTermComposite.Args[0] as LetterSym;
            var yTermSym = yTermComposite.Args[0] as LetterSym;

            if (!(xTermSym.Letter.ToString().Equals("x") || xTermSym.Letter.ToString().Equals("X")))
            {
                point = null;
                return false;                 
            }

            if (!(yTermSym.Letter.ToString().Equals("y") || yTermSym.Letter.ToString().Equals("Y")))
            {
                point = null;
                return false;                 
            }

            Expr xCordTerm = xTermComposite.Args[1];
            Expr yCordTerm = yTermComposite.Args[1];

            double xCor, yCor;

            if (xCordTerm is CompositeExpr)  // X - 1 e.g
            {
                var xCordComposite = xCordTerm as CompositeExpr;
                if (!(xCordComposite.Head.Equals(WellKnownSym.minus) &&
                     xCordComposite.Args.Length == 1))
                {
                    point = null;
                    return false;                      
                }

                if (xCordComposite.Args[0] is IntegerNumber)
                {
                    var xCordInteger = xCordComposite.Args[0] as IntegerNumber;
                    xCor = double.Parse(xCordInteger.Num.ToString());
                }
                else if (xCordComposite.Args[0] is DoubleNumber)
                {
                    var xCordDouble = xCordComposite.Args[0] as DoubleNumber;
                    xCor = xCordDouble.Num;
                }
                else
                {
                    point = null;
                    return false;                     
                }
            }
            else if (xCordTerm is IntegerNumber)
            {
                var xCordInteger = xCordTerm as IntegerNumber;
                xCor = -1 * double.Parse(xCordInteger.Num.ToString());
            }
            else if (xCordTerm is DoubleNumber)
            {
                var xCordDouble = xCordTerm as DoubleNumber;
                xCor = -1 * xCordDouble.Num;               
            }
            else
            {
                point = null;
                return false; 
            }

            if (yCordTerm is CompositeExpr)  // X - 1 e.g
            {
                var yCordComposite = yCordTerm as CompositeExpr;
                if (!(yCordComposite.Head.Equals(WellKnownSym.minus) &&
                     yCordComposite.Args.Length == 1))
                {
                    point = null;
                    return false;
                }

                if (yCordComposite.Args[0] is IntegerNumber)
                {
                    var yCordInteger = yCordComposite.Args[0] as IntegerNumber;
                    yCor = double.Parse(yCordInteger.Num.ToString());
                }
                else if (yCordComposite.Args[0] is DoubleNumber)
                {
                    var yCordDouble = yCordComposite.Args[0] as DoubleNumber;
                    yCor = yCordDouble.Num;
                }
                else
                {
                    point = null;
                    return false;
                }
            }
            else if (yCordTerm is IntegerNumber)
            {
                var yCordInteger = yCordTerm as IntegerNumber;
                yCor = -1 * double.Parse(yCordInteger.Num.ToString());
            }
            else if (yCordTerm is DoubleNumber)
            {
                var yCordDouble = yCordTerm as DoubleNumber;
                yCor = -1 * yCordDouble.Num;
            }
            else
            {
                point = null;
                return false;
            }
 
            point = new Point(xCor, yCor);
            return true;
        }

        #endregion

        #region Ellipse Matching

        private bool MatchEllipseImplicitTemplate(Expr expr, out ShapeExpr shape)
        {
            if (!(expr is CompositeExpr))
            {
                shape = null;
                return false;
            }

            var compositeExpr = expr as CompositeExpr;

            if (!compositeExpr.Head.Equals(WellKnownSym.equals)
                || compositeExpr.Args.Length != 2)
            {
                shape = null;
                return false;
            }

            Expr leftExpr = compositeExpr.Args[0];
            Expr rightExpr = compositeExpr.Args[1];

            if (!(rightExpr is IntegerNumber && leftExpr is CompositeExpr))
            {
                shape = null;
                return false;
            }

            var rightNumber = rightExpr as IntegerNumber;
            if (!rightNumber.Num.IsOne)
            {
                shape = null;
                return false;                
            }
            var leftCompositeExpr = leftExpr as CompositeExpr;

            if (!(leftCompositeExpr.Head.Equals(WellKnownSym.plus) &&
                  leftCompositeExpr.Args.Length == 2))
            {
                shape = null;
                return false;                     
            }

            Expr xTerm = leftCompositeExpr.Args[0];
            Expr yTerm = leftCompositeExpr.Args[1];

            if (!(xTerm is CompositeExpr && yTerm is CompositeExpr))
            {
                shape = null;
                return false; 
            }

            var xCompositeTerm = xTerm as CompositeExpr;
            var yCompositeTerm = yTerm as CompositeExpr;


            /////////////////////////////////////////////////////////////////
                        
            if (!(xCompositeTerm.Head.Equals(WellKnownSym.times) &&
                  yCompositeTerm.Head.Equals(WellKnownSym.times) &&
                  xCompositeTerm.Args.Length == 2 &&
                  yCompositeTerm.Args.Length == 2))
            {
                shape = null;
                return false;                     
            }

            Expr xCordTerm = xCompositeTerm.Args[0];
            Expr xSemiTerm = xCompositeTerm.Args[1];
            Expr yCordTerm = yCompositeTerm.Args[0];
            Expr ySemiTerm = yCompositeTerm.Args[1];

            if (!(xCordTerm is CompositeExpr && xSemiTerm is CompositeExpr &&
                  yCordTerm is CompositeExpr && ySemiTerm is CompositeExpr))
            {
                shape = null;
                return false;                                                            
            }

            var xCordCompositeTerm = xCordTerm as CompositeExpr;
            var xSemiCompositeTerm = xSemiTerm as CompositeExpr;
            var yCordCompositeTerm = yCordTerm as CompositeExpr;
            var ySemiCompositeTerm = ySemiTerm as CompositeExpr;

            if (!(xSemiCompositeTerm.Head.Equals(WellKnownSym.divide) &&
                  ySemiCompositeTerm.Head.Equals(WellKnownSym.divide) &&
                  xSemiCompositeTerm.Args.Length == 1  &&
                  ySemiCompositeTerm.Args.Length == 1 ))
            {
                shape = null;
                return false;                                                                           
            }

            xSemiTerm = xSemiCompositeTerm.Args[0];
            ySemiTerm = ySemiCompositeTerm.Args[0];

            if (!(ySemiTerm is CompositeExpr && xSemiTerm is CompositeExpr))
            {
                shape = null;
                return false;                              
            }

            xSemiCompositeTerm = xSemiTerm as CompositeExpr;
            ySemiCompositeTerm = ySemiTerm as CompositeExpr;

            if (!(xCordCompositeTerm.Head.Equals(WellKnownSym.power) &&
                  xSemiCompositeTerm.Head.Equals(WellKnownSym.power) &&
                  yCordCompositeTerm.Head.Equals(WellKnownSym.power) &&
                  ySemiCompositeTerm.Head.Equals(WellKnownSym.power) &&
                  xCordCompositeTerm.Args.Length == 2 &&
                  xSemiCompositeTerm.Args.Length == 2 &&
                  yCordCompositeTerm.Args.Length == 2 &&
                  ySemiCompositeTerm.Args.Length == 2))
            {
                shape = null;
                return false;                                              
            }

            var xCordSquareTerm = xCordCompositeTerm.Args[1];
            var xSemiSquareTerm = xSemiCompositeTerm.Args[1];
            var yCordSquareTerm = yCordCompositeTerm.Args[1];
            var ySemiSquareTerm = ySemiCompositeTerm.Args[1];

            if (!(xCordSquareTerm is IntegerNumber &&
                  xSemiSquareTerm is IntegerNumber &&
                  yCordSquareTerm is IntegerNumber &&
                  ySemiSquareTerm is IntegerNumber))
            {
                shape = null;
                return false;                 
            }

            var xCordSquareNumber = xCordSquareTerm as IntegerNumber;
            var xSemiSquareNumber = xSemiSquareTerm as IntegerNumber;
            var yCordSquareNumber = yCordSquareTerm as IntegerNumber;
            var ySemiSquareNumber = ySemiSquareTerm as IntegerNumber;

            if (!(xCordSquareNumber.Num == 2 &&
                  xSemiSquareNumber.Num == 2 &&
                  yCordSquareNumber.Num == 2 &&
                  ySemiSquareNumber.Num == 2))
            {
                shape = null;
                return false;
            }

            xCordTerm = xCordCompositeTerm.Args[0];
            xSemiTerm = xSemiCompositeTerm.Args[0];
            yCordTerm = yCordCompositeTerm.Args[0];
            ySemiTerm = ySemiCompositeTerm.Args[0];

            #region center Point coordinate

            if (!((xCordTerm is CompositeExpr) &&
                (yCordTerm is CompositeExpr)))
            {
                shape = null;
                return false;
            }

            xCordCompositeTerm = xCordTerm as CompositeExpr;
            yCordCompositeTerm = yCordTerm as CompositeExpr;

            if (!(xCordCompositeTerm.Head.Equals(WellKnownSym.plus) &&
                  yCordCompositeTerm.Head.Equals(WellKnownSym.plus) &&
                  xCordCompositeTerm.Args.Length == 2 &&
                  yCordCompositeTerm.Args.Length == 2 &&
                  xCordCompositeTerm.Args[0] is LetterSym &&
                  yCordCompositeTerm.Args[0] is LetterSym))
            {
                shape = null;
                return false;
            }

            var xTermSym = xCordCompositeTerm.Args[0] as LetterSym;
            var yTermSym = yCordCompositeTerm.Args[0] as LetterSym;

            if (!(xTermSym.Letter.ToString().Equals("x") || xTermSym.Letter.ToString().Equals("X")))
            {
                shape = null;
                return false;
            }

            if (!(yTermSym.Letter.ToString().Equals("y") || yTermSym.Letter.ToString().Equals("Y")))
            {
                shape = null;
                return false;
            }

            xCordTerm = xCordCompositeTerm.Args[1];
            yCordTerm = yCordCompositeTerm.Args[1];

            double xCor, yCor;

            if (xCordTerm is CompositeExpr)  // X - 1 e.g
            {
                var xCordComposite = xCordTerm as CompositeExpr;
                if (!(xCordComposite.Head.Equals(WellKnownSym.minus) &&
                     xCordComposite.Args.Length == 1))
                {
                    shape = null;
                    return false;
                }

                if (xCordComposite.Args[0] is IntegerNumber)
                {
                    var xCordInteger = xCordComposite.Args[0] as IntegerNumber;
                    xCor = double.Parse(xCordInteger.Num.ToString());
                }
                else if (xCordComposite.Args[0] is DoubleNumber)
                {
                    var xCordDouble = xCordComposite.Args[0] as DoubleNumber;
                    xCor = xCordDouble.Num;
                }
                else
                {
                    shape = null;
                    return false;
                }
            }
            else if (xCordTerm is IntegerNumber)
            {
                var xCordInteger = xCordTerm as IntegerNumber;
                xCor = -1 * double.Parse(xCordInteger.Num.ToString());
            }
            else if (xCordTerm is DoubleNumber)
            {
                var xCordDouble = xCordTerm as DoubleNumber;
                xCor = -1 * xCordDouble.Num;
            }
            else
            {
                shape = null;
                return false;
            }

            if (yCordTerm is CompositeExpr)  // X - 1 e.g
            {
                var yCordComposite = yCordTerm as CompositeExpr;
                if (!(yCordComposite.Head.Equals(WellKnownSym.minus) &&
                     yCordComposite.Args.Length == 1))
                {
                    shape = null;
                    return false;
                }

                if (yCordComposite.Args[0] is IntegerNumber)
                {
                    var yCordInteger = yCordComposite.Args[0] as IntegerNumber;
                    yCor = double.Parse(yCordInteger.Num.ToString());
                }
                else if (yCordComposite.Args[0] is DoubleNumber)
                {
                    var yCordDouble = yCordComposite.Args[0] as DoubleNumber;
                    yCor = yCordDouble.Num;
                }
                else
                {
                    shape = null;
                    return false;
                }
            }
            else if (yCordTerm is IntegerNumber)
            {
                var yCordInteger = yCordTerm as IntegerNumber;
                yCor = -1 * double.Parse(yCordInteger.Num.ToString());
            }
            else if (yCordTerm is DoubleNumber)
            {
                var yCordDouble = yCordTerm as DoubleNumber;
                yCor = -1 * yCordDouble.Num;
            }
            else
            {
                shape = null;
                return false;
            }
       
            #endregion

             var center  = new Point(xCor, yCor);

            double radiusAlongX, radiusAlongY;
            if (xSemiTerm is IntegerNumber)
            {
                var xSemiInteger = xSemiTerm as IntegerNumber;
                radiusAlongX = double.Parse(xSemiInteger.Num.ToString());
            }
            else if (xSemiTerm is DoubleNumber)
            {
                var radiusDouble = xSemiTerm as DoubleNumber;
                radiusAlongX = radiusDouble.Num;
            }
            else
            {
                shape = null;
                return false;
            }

            if (ySemiTerm is IntegerNumber)
            {
                var ySemiInteger = ySemiTerm as IntegerNumber;
                radiusAlongY = double.Parse(ySemiInteger.Num.ToString());
            }
            else if (ySemiTerm is DoubleNumber)
            {
                var radiusDouble = ySemiTerm as DoubleNumber;
                radiusAlongY = radiusDouble.Num;
            }
            else
            {
                shape = null;
                return false;
            }

            var ellipse = new Ellipse(center, radiusAlongX, radiusAlongY); 
            shape = new EllipseExpr(expr, ellipse);
            return true;
        }

        #endregion

        #endregion
    }

    public class AGTemplateExpressions
    {
        ////////////////////////// Point Template ///////////////////////////////////

        public static List<string> PointTemplates = new List<string>()
        {
            "A(1,2)",
            "(1,2)"
        };

        /////////////////////////// Line Template ///////////////////////////////////         

        public static string LineImplicitTemplate = "a*X + b*Y + c = 0";
        public static string LineExplicitTemplate = "Y = aX + b";

        public static readonly string[] LineParametricTemplate = new string[]
        {
            "X = x0 + a * T",
            "Y = y0 + b * T",
        };

        /////////////////////////// Circle Template ///////////////////////////////////   

        public const string CircleImplicitTemplate = "(X - a) ^ 2 + (Y - b) ^ 2 = r ^ 2";
        public static readonly string[] CircleParametricTemplate = new string[]
        {
           "X = r * cos(t)",
           "Y = r * sin(t)"
        };

        /////////////////////////// Ellipse Template ///////////////////////////////////

        public const string EllipseImplicitTemplate = "(X - h) ^ 2 / a ^ 2 + (Y - k) ^ 2 / b ^ 2 = 1";

        public static readonly string[] EllipseParametricTemplate = new string[]
        {
            "X = a * cos(t) + h",
            "Y = b * sin(t) + k"
        };
    }
}
