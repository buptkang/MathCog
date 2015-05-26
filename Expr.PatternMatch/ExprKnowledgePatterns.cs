using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgebraGeometry;
using CSharpLogic;
using starPadSDK.MathExpr;


namespace ExprSemantic
{
    public static partial class GeneralPatternExtensions
    {
        /// <summary>
        /// Pattern for label: such as "A", "c", "XT"
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool IsLabel(this starPadSDK.MathExpr.Expr expr,
            out string label)
        {
            label = null;
            if (expr is LetterSym)
            {
                var letter = expr as LetterSym;
                label = letter.Letter.ToString();
                return true;
            }
            else if (expr is WellKnownSym)
            {
                var comma = expr as WellKnownSym;
                if (comma.Equals(WellKnownSym.comma))
                {
                    return false;
                }
            }
            else if (expr is WordSym)
            {
                var word = expr as WordSym;
                if (word.Word.Equals("comma"))
                {
                    return false;
                }
                label = word.Word;
                return true;
            }           
            return false;
        }

        /// <summary>
        /// Pattern for Coordinate, such as "Y=3", "x=4.0"
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static bool IsTerm(this starPadSDK.MathExpr.Expr expr,
            out object coord)
        {
            coord = null;
            var compExpr = expr as CompositeExpr;
            if (compExpr != null &&
                compExpr.Head.Equals(WellKnownSym.equals) &&
                compExpr.Args.Count() == 2)
            {
                var expr1 = compExpr.Args[0] as starPadSDK.MathExpr.Expr;
                var expr2 = compExpr.Args[1] as starPadSDK.MathExpr.Expr;

                string label;
                object number;
                if (expr1.IsLabel(out label) && expr2.IsNumeric(out number))
                {
                    //coord = new KeyValuePair<object, object>(label, number);
                    //var goal = new EqGoal();
                    //coord = new AGPropertyExpr(expr, new EqGoal(new Var(label), number));
                    coord = new EqGoal(new Var(label), number);
                    return true;
                }
                else if (expr1.IsNumeric(out number) && expr2.IsLabel(out label))
                {
                    //coord = new KeyValuePair<object, object>(label,number);
                    //coord = new EqGoal(new Var(label), number);\
                    coord = new EqGoal(new Var(label), number);
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Pattern for Coordinate of the point, 
        /// such as "Y=3", "x=4.0", "x+1=2", -5.0
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="?"></param>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static bool IsCoordinateTerm(this starPadSDK.MathExpr.Expr expr,
            out object coord)
        {
            coord = null;
            if (expr.IsNumeric(out coord))
            {
                return true;
            }

            string ll;
            if (expr.IsLabel(out ll))
            {
                coord = ll;
                return true;
            }

            if (expr.IsTerm(out coord))
            {
                return true;
            }

            //TODO ""x+1=2""
            //ContainLabel

            return false;
        }

        public static bool IsNumeric(this starPadSDK.MathExpr.Expr expr,
            out object number)
        {
            number = null;

            var integerExpr = expr as IntegerNumber;
            if (integerExpr != null)
            {
                number = int.Parse(integerExpr.Num.ToString());
                return true;
            }

            var doubleExpr = expr as DoubleNumber;
            if (doubleExpr != null)
            {
                number = double.Parse(doubleExpr.Num.ToString());
                return true;
            }

            starPadSDK.MathExpr.Expr tempExpr;
            if (IsNegativeTerm(expr, out tempExpr))
            {
                if (tempExpr.IsNumeric(out number))
                {
                    int n;
                    if (Utils.IsInt(number, out n))
                    {
                        number = -1*n;
                        return true;
                    }

                    double d;
                    if (Utils.IsDouble(number, out d))
                    {
                        number = -1*d;
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsNegativeTerm(this starPadSDK.MathExpr.Expr expr, out starPadSDK.MathExpr.Expr outputExpr)
        {
            outputExpr = null;
            var compositeExpr = expr as CompositeExpr;
            if (compositeExpr != null && 
                compositeExpr.Head.Equals(WellKnownSym.minus))
            {
                outputExpr = compositeExpr.Args[0];
                return true;
            }

            return false;
        }

    }

    public static class PointPatternExtensions
    {
        /// <summary>
        /// A(3.0,-4.0), (-3.0, x), B(x+1=9, 9), C(x,y)
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool IsPoint(this starPadSDK.MathExpr.Expr expr, out object point)
        {
            point = null;
            string label;
            object xCord;
            object yCord;

            var composite = expr as CompositeExpr;
            if (composite == null) return false;

            var headExpr = composite.Head;
            bool hasLabel = headExpr.IsLabel(out label);

            if (!hasLabel)
            {
                var wordSymbol = composite.Head as WordSym;
                var wellKnownSymbol = composite.Head as WellKnownSym;  
                if (wordSymbol != null)
                {
                    if (!wordSymbol.Word.Equals("comma"))
                    {
                        return false;
                    }
                }
                else if (wellKnownSymbol != null)
                {
                    if (!wellKnownSymbol.ID.Equals(WKSID.comma))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            if (composite.Args.Length != 2) return false;

            var expr1 = composite.Args[0];
            var expr2 = composite.Args[1];

            object coord1 = null;
            object coord2 = null;

            bool isPointForm =
                expr1.IsCoordinateTerm(out coord1) &&
                expr2.IsCoordinateTerm(out coord2);
               
            if (isPointForm)
            {
                if (hasLabel)
                {
                    var temp = ExprKnowledgeFactory.CreatePointSymbol(label,coord1, coord2);
                    //point = new AGShapeExpr(expr, temp);
                    point =  temp;
                }
                else
                {
                    var temp = ExprKnowledgeFactory.CreatePointSymbol(coord1, coord2);
                    //point = new AGShapeExpr(expr, temp);
                    point = temp;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }


}
