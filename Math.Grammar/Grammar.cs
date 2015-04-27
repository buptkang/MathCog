using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace Reason
{
    public class MathGrammar : Grammar
    {
        public MathGrammar()
            : base(false)
        {
            this.GrammarComments = @"Analytical Geometry Grammar";

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //1. Terminals
            var identifier = new IdentifierTerminal("identifier");
            var number = new NumberLiteral("number", NumberOptions.AllowSign);
            var lineStandardForm = new RegexBasedTerminal("lineStandardForm", @"\b([-+]?(\d*[.])?\d+)?(?i)x[-+]((\d*[.])?\d+)?(?i)y[-+]((\d*[.])?\d+)?=0\b");
            var circleStandardForm = new RegexBasedTerminal("circleStandardForm", @"\b([-+]?(\d*[.])?\d+)?((?i))^2 \b");

            var XCoordinatePrefix = new RegexBasedTerminal("XCoordinatePrefix", @"\b(?i)x=\b");
            var YCoordinatePrefix = new RegexBasedTerminal("YCoordinatePrefix", @"\b(?i)y=\b");

            var LineSlopePrefix = new RegexBasedTerminal("LineSlopePrefix", @"\b(?i)s=\b");
            var LineInterceptPrefix = new RegexBasedTerminal("LineInterceptPrefix", @"\b(?i)i=\b");
            var CircleCenterPrefix = new RegexBasedTerminal("CircleCenterPrefix", @"\b(?i)c\b");
            var CircleRadiusPrefix = new RegexBasedTerminal("CircleRadiusPrefix", @"\b(?i)r\b");
            var LengthPrefix = new RegexBasedTerminal("LengthPrefix", @"\b(?i)distance\b");
            var AnglePrefix = new RegexBasedTerminal("AnglePrefix", @"[a]");

            var questionMark = new RegexBasedTerminal("Query", @"\\?");


            var PointPrefix = new RegexBasedTerminal("LengthPrefix", @"\bPOINT:\b");

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //2. Non Terminals
            var Expr = new NonTerminal("Expr");

            var LineCircle = new NonTerminal("LineCircle");
            var LineCircleExpr = new NonTerminal("LineCircleExpr");

            var Circle = new NonTerminal("Circle");
            var CircleExpr = new NonTerminal("CircleExpr");
            var Radius = new NonTerminal("Radius");
            var Center = new NonTerminal("Center");

            var Length = new NonTerminal("Length");

            var TwoLine = new NonTerminal("TwoLine");
            var TwoLineExpr = new NonTerminal("TwoLineExpr");
            var Angle = new NonTerminal("Angle");

            var PointLine = new NonTerminal("PointLine");
            var PointLineExpr = new NonTerminal("PointLineExpr");

            var Line = new NonTerminal("Line");
            var LineExpr = new NonTerminal("LineExpr");
            var LinePointExpr = new NonTerminal("LinePointExpr");
            var Slope = new NonTerminal("Slope");
            var Intercept = new NonTerminal("Intercept");

            var Point = new NonTerminal("Point");
            var PointExpr = new NonTerminal("PointExpr");
            var PointValueExpr = new NonTerminal("PointValueExpr");

            var Coordinate = new NonTerminal("CoordinateExpr");
            var CoordinateExpr = new NonTerminal("CoordinateXExpr");

            var IsOp = new NonTerminal("IsOp");
            var ConnectOp = new NonTerminal("ConnectOp");

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //3. BNF Rules
            Expr.Rule = Coordinate | Expr + ToTerm(",") + Coordinate
                                      | Point | Expr + ToTerm(",") + Point; //
            //| Length | Expr + ToTerm(",") + Length; //| Length | Line | TwoLine | Angle | Circle | LineCircle;

            LineCircle.Rule = identifier + IsOp + LineCircleExpr | identifier + LineCircleExpr | LineCircleExpr;
            LineCircleExpr.Rule = Line + ConnectOp + Circle | Circle + ConnectOp + Line;

            Circle.Rule = identifier + IsOp + CircleExpr | identifier + CircleExpr | CircleExpr;
            CircleExpr.Rule = circleStandardForm | Center + ConnectOp + Point
                                    | Center + ConnectOp + Radius;

            Center.Rule = CircleCenterPrefix + IsOp + PointExpr | CircleCenterPrefix + PointExpr;
            Radius.Rule = CircleRadiusPrefix + IsOp + number | Length;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            PointLine.Rule = identifier + IsOp + PointLineExpr | identifier + PointLineExpr | PointLineExpr;
            PointLineExpr.Rule = Point + ConnectOp + Line | Line + ConnectOp + Point;

            Angle.Rule = TwoLine;
            TwoLine.Rule = identifier + IsOp + TwoLineExpr | identifier + TwoLineExpr | TwoLineExpr;
            TwoLineExpr.Rule = Line + ConnectOp + Line;

            Line.Rule = identifier + IsOp + LineExpr | identifier + LineExpr | LineExpr;
            LineExpr.Rule = lineStandardForm | LinePointExpr + ConnectOp + LinePointExpr | LineExpr + ConnectOp + Point;
            LinePointExpr.Rule = Point | Slope | Intercept;
            Slope.Rule = LineSlopePrefix + number;
            Intercept.Rule = LineInterceptPrefix + number;

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Length.Rule = LengthPrefix + "(" + Point + ConnectOp + Point + ")"
                                        | LengthPrefix + "=" + number
                                        | LengthPrefix + "(" + identifier + ConnectOp + identifier + ")";

            Point.Rule = identifier + IsOp + PointExpr | identifier + PointExpr | PointExpr;
            IsOp.Rule = ToTerm("=") | ":";

            PointExpr.Rule = "(" + Coordinate + ConnectOp + Coordinate + ")";
            ConnectOp.Rule = ToTerm(",") | ":";

            Coordinate.Rule = identifier + IsOp + CoordinateExpr | CoordinateExpr;

            CoordinateExpr.Rule = XCoordinatePrefix + number | XCoordinatePrefix + identifier
                                    | YCoordinatePrefix + number | YCoordinatePrefix + identifier;

            IsOp.Rule = ToTerm("=") | ":";

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            Root = Expr;

            MarkPunctuation("=", "(", ",", ")", ":");
            MarkTransient(IsOp, ConnectOp);

            //LanguageFlags = LanguageFlags.CreateAst;
        }

    }
}
