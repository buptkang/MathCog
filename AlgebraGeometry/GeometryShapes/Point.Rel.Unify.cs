using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public static class PointUnaryRelation
    {
        public static bool Unify(this PointSymbol shapeSymbol, object constraint, out object output)
        {
            output = shapeSymbol.Unify(constraint);
            return output != null;
        }

        public static object Unify(this PointSymbol ps, object constraint)
        {
            var refObj = constraint as string;
            Debug.Assert(refObj != null);
            switch (refObj)
            {
                case PointAcronym.X:
                case PointAcronym.X1:
                    return ps.InferXCoord(refObj);
                case PointAcronym.Y:
                case PointAcronym.Y1:
                    return ps.InferYCoord(refObj);
            }
            return null;
        }

        private static EqGoal InferXCoord(this PointSymbol inputPointSymbol, string label)
        {
            var point = inputPointSymbol.Shape as Point;
            Debug.Assert(point != null);
            var goal = new EqGoal(new Var(label), point.XCoordinate);
            return goal;
        }

        private static EqGoal InferYCoord(this PointSymbol inputPointSymbol, string label)
        {
            var point = inputPointSymbol.Shape as Point;
            Debug.Assert(point != null);
            var goal = new EqGoal(new Var(label), point.YCoordinate);
            return goal;
        }

    }
}
