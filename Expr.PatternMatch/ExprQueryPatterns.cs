using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using starPadSDK.MathExpr;

namespace ExprSemantic
{
    public static partial class GeneralQueryExtensions
    {
        public static bool IsQueryFormWithoutQuestionMark(this Expr expr, out string property)
        {
            property = null;
            if (!(expr is CompositeExpr)) return false;
            var composite = expr as CompositeExpr;
            if (!(composite.Head.Equals(WellKnownSym.equals) && composite.Args.Length == 2)) return false;

            Expr expr1 = composite.Args[0];
            Expr expr2 = composite.Args[1];

            if (!(expr2 is ErrorExpr)) return false;

            if (expr1.IsWordTerm(out property))
            {
                return true;                
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Not in use
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="property"></param>
        /// <returns></returns>
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
                /*
                if (IsWordTerm(expr1, out property))
                {
                    return true;
                }
                else
                {
                    return false;
                }*/
            }
            else
            {
                return false;
            }
            return false;
        }



    }
}
