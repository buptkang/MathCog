using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using AlgebraExpression;
using AlgebraGeometry;
using CSharpLogic;
using starPadSDK.MathExpr;
using System.Text.RegularExpressions;


namespace ExprSemantic
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
            if (expr is LetterSym) return false;

            //Check negative
            Expr outputExpr;
            bool isNegative = false;

            if (expr.IsNegativeTerm(out outputExpr))
            {
                expr = outputExpr;
                isNegative = true;
            }

            var wordExpr = expr as WordSym;
            if (wordExpr == null) return false;

            string[] strs = Regex.Split(wordExpr.Word, "(?<=\\D)(?=\\d)|(?<=\\d)(?=\\D)");

            if (strs.Length == 1) //xx, mm, axy, xy
            {
                return false;
            }
            else if (strs.Length == 2) // 2xy, 39x, 
            {
                var headStr = strs[0];
                var tailStr = strs[1];

                if (LogicSharp.IsNumeric(headStr))
                {
                    double dNum;
                    LogicSharp.IsDouble(headStr, out dNum);
                    int iNum;
                    bool result = LogicSharp.IsInt(headStr, out iNum);
                    if (result)
                    {
                        if (isNegative)
                        {
                            iNum = -1*iNum;
                        }
                        obj = new Term(Expression.Multiply, new Tuple<object, object>(iNum, new Var(tailStr)));
                    }
                    else
                    {
                        if (isNegative)
                        {
                            dNum = -1 * dNum;
                        }
                        obj = new Term(Expression.Multiply, new Tuple<object, object>(dNum, new Var(tailStr)));
                    }
                    return true;
                }
                else
                {
                    return false;
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

    public static class QueryPatternExtensions
    {
        public static bool IsQuery(this Expr expr, out object property)
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
                    property = new KeyValuePair<string, object>("Label", new Var(obj));
                    return true;
                }

                result = expr1.IsExpression(out obj);
                if (result)
                {
                    property = new KeyValuePair<string, object>("Term", obj);
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
                        property = new KeyValuePair<string, object>("Label", new Var(obj));
                        return true;
                    }

                    result = expr1.IsExpression(out obj);
                    if (result)
                    {
                        property = new KeyValuePair<string, object>("Term", obj);
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

    public static class ExpressionPatternExtensions
    {
        public static bool IsExpression(this Expr expr, out object obj)
        {
            obj = null;

            if (expr.IsNumeric(out obj)) return true;
            if (expr.IsLabel(out obj))
            {
                object iObj;
                bool isTerm = expr.IsWordTerm(out iObj);
                if (isTerm)
                {
                    obj = iObj;
                    return true;
                }
                obj = new Var((string)obj);
                return true;
            }

            var root = expr as CompositeExpr;
            if (root == null) return false;

            if (root.Head.Equals(WellKnownSym.plus))
            {
                #region Add Term
                int argCount = root.Args.Count();
                if (argCount == 2)
                {
                    object obj1;
                    bool arg1Expr = root.Args[0].IsExpression(out obj1);
                    object obj2;
                    bool arg2Expr = root.Args[1].IsExpression(out obj2);

                    if (arg1Expr && arg2Expr)
                    {
                        obj = new Term(Expression.Add, new Tuple<object, object>(obj1, obj2));
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
                        obj = new Term(Expression.Add, new Tuple<object, object>(obj1, obj2));
                        object tempObj;
                        for (int i = 2; i < argCount; i++)
                        {
                            if (root.Args[i].IsExpression(out tempObj))
                            {
                                obj = new Term(Expression.Add, new Tuple<object, object>(obj, tempObj));
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
            else if (root.Head.Equals(WellKnownSym.minus))
            {
                Debug.Assert(root.Args.Length == 1);
                object obj2;
                bool result = root.Args[0].IsLabel(out obj2);
                if (result)
                {
                    var label = obj2 as string;
                    string newLabel = "-" + label;
                    obj = new Var(newLabel);
                    return true;
                }
                else
                {
                    throw new Exception("TODO");
                }
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
                        obj = new Term(Expression.Multiply, new Tuple<object, object>(obj1, obj2));
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
                        obj = new Term(Expression.Multiply, new Tuple<object, object>(obj1, obj2));
                        object tempObj;
                        for (int i = 2; i < argCount; i++)
                        {
                            if (root.Args[i].IsExpression(out tempObj))
                            {
                                obj = new Term(Expression.Multiply, new Tuple<object, object>(obj, tempObj));
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

    public static class GoalPatternExtensions
    {
        /// <summary>
        /// Pattern for Coordinate, such as "Y=3", "x=4.0"
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static bool IsTerm(this Expr expr,
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
        /// X + 1 = 2+3
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsGoal(this Expr expr, out object obj)
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
                    obj = new EqGoal(obj1, obj2);
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
                    var temp = PointEvaluator.CreatePointSymbol((string)label,coord1, coord2);
                    //point = new AGShapeExpr(expr, temp);
                    point =  temp;
                }
                else
                {
                    var temp = PointEvaluator.CreatePointSymbol(coord1, coord2);
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

    public static class LinePatternExtensions
    {
        public static bool IsLine(this Expr expr, out LineSymbol ls)
        {
            ls = null;
            var compExpr = expr as CompositeExpr;
            if (compExpr == null) return false;

            if (compExpr.Head.Equals(WellKnownSym.equals) &&
                compExpr.Args.Count() == 2)
            {
                Expr lhsExpr = compExpr.Args[0];
                Expr rhsExpr = compExpr.Args[1];
                object obj1, obj2;

                bool result1 = lhsExpr.IsExpression(out obj1);
                bool result2 = rhsExpr.IsExpression(out obj2);

                if (result1 && result2)
                {
                    Line line;
                    bool result = LineEvaluator.Unify(obj1, obj2, out line);
                    if (result)
                    {
                        ls = new LineSymbol(line);
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
            else
            {
                return false;
            }
        }

        public static bool IsLineRel(this Expr expr, out LineSymbol ls)
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
        public static bool IsXTerm(this Expr expr, out object coeffExpr)
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
                            //TODO
                            //coeff.IsConstantTerm(out coeffExpr);
                            //coeffExpr = isNegative ? new DoubleNumber(coeffExpr.Num*-1.0) : coeffExpr;
                            coeffExpr = null;
                            return true;
                        }
                    }
                }
            }

            coeffExpr = null;
            return false;
        }

        public static bool IsYTerm(this Expr expr, out object coeffExpr)
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
                            //TODO
                            //coeff.IsConstantTerm(out coeffExpr);
                            //coeffExpr = isNegative ? new DoubleNumber(coeffExpr.Num * -1.0) : coeffExpr;
                            coeffExpr = null;
                            return true;
                        }
                    }
                }
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