using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ExprSemantic.KnowledgeRelation;
using starPadSDK.MathExpr;
using AGSemantic.KnowledgeBase;

namespace ExprSemantic.KnowledgeUnification
{
    public static class BuiltInEngineExtensions
    {
        public static bool IsTwoLabel(this Expr expr, out List<string> letters)
        {
            letters = null;
            if (!(expr is CompositeExpr)) return false;
            var composite = expr as CompositeExpr;

            if (!((composite.Head.Equals(WellKnownSym.times)) && composite.Args.Length == 2)) return false;

            if (composite.Args[0] is LetterSym && composite.Args[1] is LetterSym)
            {
                letters = new List<string>();
                var first = composite.Args[0] as LetterSym;
                var second = composite.Args[1] as LetterSym;
                letters.Add(first.Letter.ToString());
                letters.Add(second.Letter.ToString());
                return true;
            }
            else
            {
                return false;                
            }
        }

        public static bool IsPointForm(this Expr expr, out PointExpr pointExpr)
        {
            pointExpr = null;
            if (!(expr is CompositeExpr)) return false;
            var composite = expr as CompositeExpr;

            if (!((composite.Head is LetterSym) && composite.Args.Length == 2)) return false;

            var label = composite.Head as LetterSym;

            if (composite.Args[0] is IntegerNumber && composite.Args[1] is IntegerNumber)
            {
                var expr1 = composite.Args[0] as IntegerNumber;
                var expr2 = composite.Args[1] as IntegerNumber;
                pointExpr = new PointExpr(new Point(label.Letter.ToString(), 
                                    expr1.Num.AsDouble(), expr2.Num.AsDouble()), expr);
                return true;
            }
            else if (composite.Args[0] is LetterSym || composite.Args[1] is LetterSym)
            {
                pointExpr = new PointExpr(new Point(0.0,1.0));
                return true;                
            }
            else
            {
                return false;
            }
        }

        public static bool IsDistanceForm(this Expr expr, out IKnowledgeExpr distanceExpr)
        {
            distanceExpr = null;

            if (!(expr is CompositeExpr)) return false;
            var composite = expr as CompositeExpr;
            if (!(composite.Head.Equals(WellKnownSym.equals) && composite.Args.Length == 2)) return false;

            Expr expr1 = composite.Args[0];
            Expr expr2 = composite.Args[1];

            if (!(expr2 is IntegerNumber)) return false;
            if (!(expr1 is CompositeExpr)) return false;
            
            composite = expr1 as CompositeExpr;

            if (!(composite.Head is LetterSym && composite.Args.Length == 2)) return false;

            expr1 = composite.Args[0];
            expr2 = composite.Args[1];

            if (expr1 is IntegerNumber || expr1 is LetterSym || expr2 is IntegerNumber || expr2 is LetterSym)
            {
                distanceExpr = new PointPointExpr(new TwoPoints(new Point(6.0,8.0), new Point(6.0,0.0)));
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsQueryFormWithoutQuestionMark(this Expr expr, out string property)
        {
            property = null;
            if (!(expr is CompositeExpr)) return false;
            var composite = expr as CompositeExpr;
            if (!(composite.Head.Equals(WellKnownSym.equals) && composite.Args.Length == 2)) return false;

            Expr expr1 = composite.Args[0];
            Expr expr2 = composite.Args[1];

            if (!(expr2 is ErrorExpr)) return false;

            if (IsWordTerm(expr1, out property))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsQueryFormWithQustionMark(this Expr expr, out string property)
        {
            property = null;
            if (!(expr is CompositeExpr)) return false;
            var composite = expr as CompositeExpr;
            if (!(composite.Head.Equals(WellKnownSym.equals) && composite.Args.Length == 2)) return false;

            Expr expr1 = composite.Args[0];
            Expr expr2 = composite.Args[1];

            if (!(expr2 is LetterSym)) return false;
            var questionMarkLetter = expr2 as LetterSym;
            if (questionMarkLetter.Letter.Equals('?'))
            {
                if (IsWordTerm(expr1, out property))
                {
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

        private static bool IsWordTerm(this Expr expr, out string word)
        {
            word = null;
            if (expr is LetterSym)
            {
                var letter = expr as LetterSym;
                word = letter.Letter.ToString();
                return true;
            }

            if (expr is CompositeExpr)
            {
                var compositeExpr = expr as CompositeExpr;
                if (compositeExpr.Head.Equals(WellKnownSym.times))
                {
                    var builder = new StringBuilder();
                    foreach (Expr temp in compositeExpr.Args)
                    {
                        DoubleNumber number;
                        if (temp is LetterSym)
                        {
                            var tempLetter = temp as LetterSym;
                            builder.Append(tempLetter.Letter);                            
                        }
                        else if (IsConstantTerm(temp, out number))
                        {
                            builder.Append(number.Num);
                        }
                        else
                        {
                            return false;
                        }
                    }

                    word = builder.ToString();
                    return true;
                }
            }

            return false;
        }

        public static bool IsYYTerm(this Expr expr, out DoubleNumber coeffExpr)
        {
            //Check negative
            Expr outputExpr;
            bool isNegative = false;

            if (expr.IsNegativeTerm(out outputExpr))
            {
                expr = outputExpr;
                isNegative = true;
            }

            if (!(expr is CompositeExpr))
            {
                coeffExpr = null;
                return false;
            }
            var compositeExpr = expr as CompositeExpr;
            //if (compositeExpr.Head.Equals(WellKnownSym.times) && compositeExpr.Args.Length == 2
            //    && compositeExpr.Args[1].IsPowerYForm())
            
            if (compositeExpr.Head.Equals(WellKnownSym.times) && compositeExpr.Args.Length == 2
                     && compositeExpr.Args[1].IsPowerYForm())
            {
                compositeExpr.Args[0].IsConstantTerm(out coeffExpr);
                coeffExpr = isNegative ? new DoubleNumber(coeffExpr.Num * -1) : coeffExpr;
                return true;
            }
            else if (compositeExpr.IsPowerYForm())
            {
                coeffExpr = isNegative ? new DoubleNumber(-1) : new DoubleNumber(1);
                return true;
            }

            coeffExpr = null;
            return false;            
        }

        public static bool IsXXTerm(this Expr expr, out DoubleNumber coeffExpr)
        {
            //Check negative
            Expr outputExpr;
            bool isNegative = false;

            if (expr.IsNegativeTerm(out outputExpr))
            {
                expr = outputExpr;
                isNegative = true;
            }

            if (!(expr is CompositeExpr))
            {
                coeffExpr = null;
                return false;
            }
            var compositeExpr = expr as CompositeExpr;
            //if (compositeExpr.Head.Equals(WellKnownSym.times) && compositeExpr.Args.Length == 2
            //    && compositeExpr.Args[1].IsPowerXForm())
            if(compositeExpr.Head.Equals(WellKnownSym.times) && compositeExpr.Args.Length == 2
                     && compositeExpr.Args[1].IsPowerXForm())
            {
                compositeExpr.Args[0].IsConstantTerm(out coeffExpr);
                coeffExpr = isNegative ? new DoubleNumber(coeffExpr.Num * -1) : coeffExpr;
                return true;
            }
            else if (compositeExpr.IsPowerXForm())
            {
                coeffExpr = isNegative ? new DoubleNumber(-1) : new DoubleNumber(1);
                return true;
            }

            coeffExpr = null;
            return false;
        }

        private static bool IsPowerYForm(this Expr expr)
        {
            if (!(expr is CompositeExpr))
            {
                return false;
            }

            var compositeExpr = expr as CompositeExpr;
            if (compositeExpr.Head.Equals(WellKnownSym.power) && compositeExpr.Args.Length == 2)
            {
                Expr expr1 = compositeExpr.Args[0];
                Expr expr2 = compositeExpr.Args[1];

                DoubleNumber dn1, dn2;
                if (expr1.IsYTerm(out dn1) && expr2.IsConstantTerm(out dn2))
                {
                    if (dn2.Num.Equals(2.0))
                    {
                        return true;
                    }
                }
            }        
            return false;
        }

        private static bool IsPowerXForm(this Expr expr)
        {
            if (!(expr is CompositeExpr))
            {
                return false;
            }

            var compositeExpr = expr as CompositeExpr;
            if (compositeExpr.Head.Equals(WellKnownSym.power) && compositeExpr.Args.Length == 2)
            {
                Expr expr1 = compositeExpr.Args[0];
                Expr expr2 = compositeExpr.Args[1];

                DoubleNumber dn1, dn2;
                if (expr1.IsXTerm(out dn1) && expr2.IsConstantTerm(out dn2))
                {
                    if (dn2.Num.Equals(2.0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IsYTerm(this Expr expr, out DoubleNumber coeffExpr)
        {
            //Check negative
            Expr outputExpr;
            bool isNegative = false;

            if (expr.IsNegativeTerm(out outputExpr))
            {
                expr = outputExpr;
                isNegative = true;
            }

            if (expr is LetterSym)
            {
                var letter = expr as LetterSym;
                if (letter.Letter.Equals('Y') || letter.Letter.Equals('y'))
                {
                    coeffExpr = isNegative ? new DoubleNumber(-1.0) : new DoubleNumber(1.0);
                    return true;
                }
            }

            if (expr is WordSym)
            {
                var wordExpr = expr as WordSym;
                if (wordExpr.Word.Last().Equals('y') || wordExpr.Word.Last().Equals('Y'))
                {
                    try
                    {
                        int number = int.Parse(wordExpr.Word.Substring(0, wordExpr.Word.Length - 1));
                        coeffExpr = isNegative ? new DoubleNumber(-1 * number) : new DoubleNumber(number);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (expr is CompositeExpr)
            {
                var compositeExpr = expr as CompositeExpr;
                if (compositeExpr.Head.Equals(WellKnownSym.times) && compositeExpr.Args.Length == 2)
                {
                    Expr coeff = compositeExpr.Args[0];
                    Expr letterY = compositeExpr.Args[1];
                    if (letterY is LetterSym)
                    {
                        var letter = letterY as LetterSym;
                        if (letter.Letter.Equals('Y') || letter.Letter.Equals('y'))
                        {
                            coeff.IsConstantTerm(out coeffExpr);
                            coeffExpr = isNegative ? new DoubleNumber(coeffExpr.Num * -1.0) : coeffExpr;
                            return true;
                        }
                    }
                }
            }

            coeffExpr = null;
            return false;
        }

        public static bool ExtractXTerm(this Expr expr, out Expr coeffXExpr)
        {
            //Check negative
            Expr outputExpr;
            bool isNegative = false;

            if (expr.IsNegativeTerm(out outputExpr))
            {
                expr = outputExpr;
                isNegative = true;
            }

            if (expr is LetterSym)
            {
                var letter = expr as LetterSym;
                if (letter.Letter.Equals('X') || letter.Letter.Equals('x'))
                {
                    coeffXExpr = expr;
                    return true;
                }
            }

            if (expr is WordSym)
            {
                var wordExpr = expr as WordSym;
                if (wordExpr.Word.Last().Equals('x') || wordExpr.Word.Last().Equals('X'))
                {
                    try
                    {
                        int number = int.Parse(wordExpr.Word.Substring(0, wordExpr.Word.Length - 1));
                        coeffXExpr = expr;
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            if (expr is CompositeExpr)
            {
                var compositeExpr = expr as CompositeExpr;
                if (compositeExpr.Head.Equals(WellKnownSym.times) && compositeExpr.Args.Length == 2)
                {
                    Expr coeff = compositeExpr.Args[0];
                    Expr letterX = compositeExpr.Args[1];
                    if (letterX is LetterSym)
                    {
                        var letter = letterX as LetterSym;
                        if (letter.Letter.Equals('X') || letter.Letter.Equals('x'))
                        {
                            coeffXExpr = compositeExpr.Args[0];
                            return true;
                        }
                    }
                }
            }

            coeffXExpr = null;
            return false;
        }

        public static bool IsXTerm(this Expr expr, out DoubleNumber coeffExpr)
        {
            //Check negative
            Expr outputExpr;
            bool isNegative = false;

            if (expr.IsNegativeTerm(out outputExpr))
            {
                expr = outputExpr;
                isNegative = true;
            }

            if (expr is LetterSym)
            {
                var letter = expr as LetterSym;
                if (letter.Letter.Equals('X') || letter.Letter.Equals('x'))
                {
                    coeffExpr = isNegative ? new DoubleNumber(-1.0) : new DoubleNumber(1.0);
                    return true;
                }
            }

            if (expr is WordSym)
            {
                var wordExpr = expr as WordSym;
                if (wordExpr.Word.Last().Equals('x') || wordExpr.Word.Last().Equals('X'))
                {
                    try
                    {
                        int number = int.Parse(wordExpr.Word.Substring(0, wordExpr.Word.Length - 1));
                        coeffExpr = isNegative ? new DoubleNumber(-1 * number) : new DoubleNumber(number);
                    }
                    catch (Exception)
                    {
                    }                   
                }
            }

            if (expr is CompositeExpr)
            {
                var compositeExpr = expr as CompositeExpr;
                if (compositeExpr.Head.Equals(WellKnownSym.times) && compositeExpr.Args.Length == 2)
                {
                    Expr coeff = compositeExpr.Args[0];
                    Expr letterX = compositeExpr.Args[1];
                    if (letterX is LetterSym)
                    {
                        var letter = letterX as LetterSym;
                        if (letter.Letter.Equals('X') || letter.Letter.Equals('x'))
                        {
                            coeff.IsConstantTerm(out coeffExpr);
                            coeffExpr = isNegative ? new DoubleNumber(coeffExpr.Num*-1.0) : coeffExpr;
                            return true;
                        }
                    }
                }
            }

            coeffExpr = null;
            return false;
        }

        public static bool IsConstantTerm(this Expr expr, out DoubleNumber coeffExpr)
        {
            Expr outputExpr;
            bool isNegative = false;

            if (expr.IsNegativeTerm(out outputExpr))
            {
                expr = outputExpr;
                isNegative = true;                
            }

            if (expr is IntegerNumber)
            {
                var integerNumber = expr as IntegerNumber;
                coeffExpr = isNegative ? new DoubleNumber(integerNumber.Num.AsDouble() * -1) : new DoubleNumber(integerNumber.Num.AsDouble());
                return true;
            }

            if (expr is DoubleNumber)
            {
                var doubleNumber = expr as DoubleNumber;
                coeffExpr = isNegative ? new DoubleNumber(doubleNumber.Num * -1) : new DoubleNumber(doubleNumber.Num);
                return true;
            }

            coeffExpr = null;
            return false;                            
        }

        public static bool IsNegativeTerm(this Expr expr, out Expr outputExpr)
        {
            outputExpr = null;
            if (!(expr is CompositeExpr))
            {
                return false;
            }

            var compositeExpr = expr as CompositeExpr;
            if (compositeExpr.Head.Equals(WellKnownSym.minus))
            {
                outputExpr = compositeExpr.Args[0];
                return true;
            }

            return false;
        }

        public static bool IsAGLabel(this Expr expr, out string label)
        {
            label = null;
            if (expr is LetterSym)
            {
                var letter = expr as LetterSym;
                label = letter.Letter.ToString();
                return true;
            }else if (expr is WordSym)
            {
                var word = expr as WordSym;
                label = word.Word;
                return true;
            }
            return false;
        }
        

    }
}
