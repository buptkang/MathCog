using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGSemantic.KnowledgeBase;
using ExprSemantic.KnowledgeRelation;

namespace ExprSemantic.KnowledgeRelation
{
    public class RelationFactory
    {
        public static Relation CreateRelation(Shape shape1, Shape shape2)
        {
            if (shape1 is Point && shape2 is Point)
            {
                return CreateTwoPoints(shape1 as Point, shape2 as Point);
            }
            else if(shape1 is Line && shape2 is Line)
            {
                return CreateTwoLines(shape1 as Line, shape2 as Line);
            }
            else if (shape1 is Point && shape2 is Line)
            {
                return CreatePointLine(shape1 as Point, shape2 as Line);
            }
            else if (shape1 is Line && shape2 is Point)
            {
                return CreatePointLine(shape2 as Point, shape1 as Line);
            }
            else if (shape1 is Line && shape2 is Circle)
            {
                return CreateLineCircle(shape1 as Line, shape2 as Circle);
            }
            else if (shape1 is Circle && shape2 is Line)
            {
                return CreateLineCircle(shape2 as Line, shape1 as Circle);
            }

            return null;
        }

        private static TwoPoints CreateTwoPoints(Point pt1, Point pt2)
        {
            return new TwoPoints(pt1, pt2);
        }

        private static TwoLines CreateTwoLines(Line line1, Line line2)
        {
            return new Angle(line1, line2);
        }

        private static PointLine CreatePointLine(Point point, Line line)
        {
            return new PointLine(point, line);
        }

        private static LineAndCircle CreateLineCircle(Line line, Circle circle)
        {
            return new LineAndCircle(line,circle);   
        }
    }
}
