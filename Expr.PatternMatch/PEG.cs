using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using AlgebraGeometry;
using CSharpLogic;
using ExprSemantic;
using starPadSDK.MathExpr;

namespace ExprPatternMatch
{
    public static partial class GeneralPatternExtensions
    {
        /// <summary>
        /// Pattern for label: such as "A", "c", "XT","c12","c_1"
        /// False: 2A, 12mm,2m1,
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static bool IsLabel(this starPadSDK.MathExpr.Expr expr,
            out object label)
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
                if (word.Word.Equals("comma") || word.Word.Equals(""))
                {
                    return false;
                }
                label = word.Word;
                return true;
            }
            else if (expr is CompositeExpr) // merge labels
            {
                var composite = expr as CompositeExpr;
                if (composite.Head.Equals(WellKnownSym.times))
                {
                    var builder = new StringBuilder();
                    foreach (Expr tempExpr in composite.Args)
                    {
                        object tempObj;
                        bool result = tempExpr.IsLabel(out tempObj);
                        if (!result) return false;
                        builder.Append(tempObj);
                    }
                    label = builder.ToString();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Pattern for wordTerm: such as 2A, 2XY, -3mm, 
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsWordTerm(this Expr expr, out object obj)
        {
            obj = null;
            if (expr is LetterSym)
            {
                var letter = expr as LetterSym;
                obj = letter.Letter.ToString(CultureInfo.InvariantCulture);
                return true;
            }

            Expr outputExpr;
            if (expr.IsNegativeTerm(out outputExpr))
            {
                expr = outputExpr;
                bool innerTerm = expr.IsWordTerm(out obj);
                if (innerTerm)
                {
                    var str = obj as string;
                    Debug.Assert(str != null);
                    obj = "-"+ str;
                    return true;
                }
                return false;
            }

            if (expr is WellKnownSym)
            {
                var comma = expr as WellKnownSym;
                if (comma.Equals(WellKnownSym.comma))
                {
                    return false;
                }
            }

            if (expr is WordSym)
            {
                var word = expr as WordSym;
                if (word.Word.Equals("comma") || word.Word.Equals(""))
                {
                    return false;
                }
                obj = word.Word;
                return true;
            }
            
            if (expr is CompositeExpr) // merge labels
            {
                var composite = expr as CompositeExpr;
                if (composite.Head.Equals(WellKnownSym.times))
                {
                    var builder = new StringBuilder();
                    foreach (Expr tempExpr in composite.Args)
                    {
                        object tempObj;
                        bool result = tempExpr.IsLabel(out tempObj);
                        if (!result) return false;
                        builder.Append(tempObj);
                    }
                    obj = builder.ToString();
                    return true;
                }
            }

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

            Expr tempExpr;
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

        public static bool IsNegativeTerm(this Expr expr
                                          , out Expr outputExpr)
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

    public static class QueryPatternExtensions
    {
        public static bool IsQuery(this starPadSDK.MathExpr.Expr expr, out object property)
        {
            property = null;
            if (!(expr is CompositeExpr)) return false;
            var composite = expr as CompositeExpr;
            if (!composite.Head.Equals(WellKnownSym.equals)) return false;

            if (composite.Args.Length == 1)
            {
                Expr expr1 = composite.Args[0];

                object obj;
                bool result = expr1.IsLabel(out obj);
                if (result)
                {
                    //property = new KeyValuePair<string, object>("Label", new Var(obj));
                    property = new Query(new Var(obj));
                    return true;
                }
                result = expr1.IsExpression(out obj);
                if (result)
                {
                    //TODO
                    //property = new KeyValuePair<string, object>("Term", obj);
                    property = new Equation(obj, null);
                    return true;
                }
                return false;                
            }
            else if (composite.Args.Length == 2)
            {
                Expr expr1 = composite.Args[0];
                Expr expr2 = composite.Args[1];

                if(expr2 is ErrorExpr)
                {                    
                    object obj;
                    bool result = expr1.IsLabel(out obj);
                    if (result)
                    {
                        //property = new KeyValuePair<string, object>("Label", new Var(obj));
                        property = new Query(new Var(obj));
                        return true;
                    }

                    result = expr1.IsExpression(out obj);
                    if (result)
                    {
                        //property = new KeyValuePair<string, object>("Term", obj);
                        property = new Equation(obj, null);
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }

    public static partial class ExpressionPatternExtensions
    {
        private static bool IsExpressionTerm(this Expr expr, out object obj)
        {
            bool result = expr.IsWordTerm(out obj);
            if (result)
            {
                var str = obj as string;
                Debug.Assert(str != null);
                obj = TransformString(str);
                return true;
            }
            return false;
        }

        public static bool IsExpression(this Expr expr, out object obj)
        {
            obj = null;
            if (expr.IsNumeric(out obj)) return true;
            if (expr.IsExpressionTerm(out obj)) return true;

            var root = expr as CompositeExpr;
            if (root == null) return false;

            if (root.Head.Equals(WellKnownSym.plus))
            {
                #region Addition Terms
                var argCount = root.Args.Count();
                var lst = new List<object>();
                for (int i = 0; i < argCount; i++)
                {
                    object tempObj;
                    if (root.Args[i].IsExpression(out tempObj))
                    {
                        lst.Add(tempObj);
                    }
                    else
                    {
                        return false;
                    }
                }
                obj = new Term(Expression.Add, lst);
                return true;
                #endregion
            }            
            else if (root.Head.Equals(WellKnownSym.times))
            {
                #region Multiply Term
                int argCount = root.Args.Count();
                if (argCount == 2)
                {
                    object obj1;
                    bool arg1Expr = root.Args[0].IsExpression(out obj1);
                    object obj2;
                    bool arg2Expr = root.Args[1].IsExpression(out obj2);

                    if (arg1Expr && arg2Expr)
                    {
                        obj = new Term(Expression.Multiply, new List<object>(){obj1, obj2});
                        return true;
                    }
                    return false;
                }
                else if (argCount > 2)
                {
                    object obj1;
                    bool arg1Expr = root.Args[0].IsExpression(out obj1);
                    object obj2;
                    bool arg2Expr = root.Args[1].IsExpression(out obj2);

                    if (arg1Expr && arg2Expr)
                    {
                        obj = new Term(Expression.Multiply, new List<object>(){obj1, obj2});
                        object tempObj;
                        for (int i = 2; i < argCount; i++)
                        {
                            if (root.Args[i].IsExpression(out tempObj))
                            {
                                obj = new Term(Expression.Multiply, new List<object>(){obj, tempObj});
                            }
                        }
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
                #endregion
            }
            return false;
        }
    }

    public static class EquationPatternExtensions
    {
        public static bool IsEquation(this Expr expr, out object obj)
        {
            obj = null;
            var compExpr = expr as CompositeExpr;
            if (compExpr == null) return false;

            if (compExpr.Head.Equals(WellKnownSym.equals) &&
                compExpr.Args.Count() == 2)
            {
                Expr lhsExpr = compExpr.Args[0];
                Expr rhsExpr = compExpr.Args[1];

                object obj1, obj2;

                if (lhsExpr.IsExpression(out obj1)
                    && rhsExpr.IsExpression(out obj2))
                {
                    obj = new Equation(obj1, obj2);
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
    }

    public static class PointPatternExtensions
    {
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

                object label;
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

            object ll;
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

        /// <summary>
        /// A(3.0,-4.0), (-3.0, x), B(x+1=9, 9), C(x,y)
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool IsPoint(this starPadSDK.MathExpr.Expr expr, out object point)
        {
            point = null;
            object label;

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
                    if (wordSymbol.Word.Equals(""))
                    {
                        return IsPoint(composite.Args[0], out point);
                    }

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
                    var temp = CreatePointSymbol((string)label,coord1, coord2);
                    //point = new AGShapeExpr(expr, temp);
                    point =  temp;
                }
                else
                {
                    var temp = CreatePointSymbol(coord1, coord2);
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

        public static PointSymbol CreatePointSymbol(object coord1, object coord2)
        {
            object setCoord1 = null;
            object setCoord2 = null;

            if (coord1 is string)
            {
                coord1 = new Var(coord1);
            }
            else if (coord1 is KeyValuePair<object, object>)
            {
                var dict = (KeyValuePair<object, object>)coord1;
                coord1 = new Var(dict.Key);
                setCoord1 = dict.Value;
            }

            if (coord2 is string)
            {
                coord2 = new Var(coord2);
            }
            else if (coord2 is KeyValuePair<object, object>)
            {
                var dict = (KeyValuePair<object, object>)coord2;
                coord2 = new Var(dict.Key);
                setCoord2 = dict.Value;
            }

            var pt = new Point(coord1, coord2);
            if (setCoord1 != null)
            {
                pt.AddXCoord(setCoord1);
            }

            if (setCoord2 != null)
            {
                pt.AddYCoord(setCoord2);
            }
            return new PointSymbol(pt);
        }

        public static PointSymbol CreatePointSymbol(string label, object coord1, object coord2)
        {
            PointSymbol ps = CreatePointSymbol(coord1, coord2);
            if (ps != null)
            {
                ps.Shape.Label = label;
            }
            return ps;
        }   
    }

    public static class LinePatternExtensions
    {
        public static bool IsLineRel(this starPadSDK.MathExpr.Expr expr, out LineSymbol ls)
        {
            ls = null;
            var compExpr = expr as CompositeExpr;
            if (compExpr == null) return false;

            if (compExpr.Head.Equals(WellKnownSym.times) &&
                compExpr.Args.Count() == 2)
            {
                var underExpr = compExpr.Args[1] as CompositeExpr;
                if (underExpr == null) return false;
                if (underExpr.Head.Equals(WellKnownSym.divide)
                    && underExpr.Args.Count() == 1)
                {
                    var expr1 = underExpr.Args[0] as CompositeExpr;
                    if (expr1 == null) return false;

                    if (expr1.Head.Equals(WellKnownSym.times) &&
                        expr1.Args.Count() == 2)
                    {
                        object obj1, obj2;
                        bool result1 = expr1.Args[0].IsLabel(out obj1);
                        bool result2 = expr1.Args[1].IsLabel(out obj2);

                        if (result1 && result2)
                        {
                            var str1 = obj1 as string;
                            var str2 = obj2 as string;
                            Debug.Assert(str1 != null);
                            Debug.Assert(str2 != null);
                            string label = str1 + str2;
                            var line = new Line(label);
                            ls = new LineSymbol(line);
                            return true;
                        }
                        return false;                        
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            return false;
        }
    }

    public static class EllipsePatternExtensions
    {
        #region Ellipse Matching
/*

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
                  xSemiCompositeTerm.Args.Length == 1 &&
                  ySemiCompositeTerm.Args.Length == 1))
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

            var center = new Point(xCor, yCor);

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
*/

        #endregion
    }

    public static class NotInUse
    {
        private static bool IsPowerYForm(this starPadSDK.MathExpr.Expr expr)
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
               // if (expr1.IsYTerm(out dn1) && expr2.IsConstantTerm(out dn2))
               // {
               //     if (dn2.Num.Equals(2.0))
               //     {
                        return true;
               //     }
               // }
            }
            return false;
        }

        private static bool IsPowerXForm(this starPadSDK.MathExpr.Expr expr)
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
                //if (expr1.IsXTerm(out dn1) && expr2.IsConstantTerm(out dn2))
                //{
                //    if (dn2.Num.Equals(2.0))
                //    {
                        return true;
                //    }
                //}
            }
            return false;
        }

        public static bool IsXXTerm(this starPadSDK.MathExpr.Expr expr, out DoubleNumber coeffExpr)
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
            if (compositeExpr.Head.Equals(WellKnownSym.times) && compositeExpr.Args.Length == 2
                     && compositeExpr.Args[1].IsPowerXForm())
            {
                //TODO
                //compositeExpr.Args[0].IsConstantTerm(out coeffExpr);
                //coeffExpr = isNegative ? new DoubleNumber(coeffExpr.Num * -1) : coeffExpr;
                coeffExpr = null;
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

        public static bool IsYYTerm(this starPadSDK.MathExpr.Expr expr, out DoubleNumber coeffExpr)
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
                //compositeExpr.Args[0].IsConstantTerm(out coeffExpr);
                //coeffExpr = isNegative ? new DoubleNumber(coeffExpr.Num * -1) : coeffExpr;
                coeffExpr = null;
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

        public static bool IsTwoLabel(this starPadSDK.MathExpr.Expr expr, out List<string> letters)
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

        //public static bool IsDistanceForm(this Expr expr, out IKnowledgeExpr distanceExpr)
        //{
        //    distanceExpr = null;

        //    if (!(expr is CompositeExpr)) return false;
        //    var composite = expr as CompositeExpr;
        //    if (!(composite.Head.Equals(WellKnownSym.equals) && composite.Args.Length == 2)) return false;

        //    Expr expr1 = composite.Args[0];
        //    Expr expr2 = composite.Args[1];

        //    if (!(expr2 is IntegerNumber)) return false;
        //    if (!(expr1 is CompositeExpr)) return false;

        //    composite = expr1 as CompositeExpr;

        //    if (!(composite.Head is LetterSym && composite.Args.Length == 2)) return false;

        //    expr1 = composite.Args[0];
        //    expr2 = composite.Args[1];

        //    if (expr1 is IntegerNumber || expr1 is LetterSym || expr2 is IntegerNumber || expr2 is LetterSym)
        //    {
        //        distanceExpr = new PointPointExpr(new TwoPoints(new Point(6.0,8.0), new Point(6.0,0.0)));
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
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