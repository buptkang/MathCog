using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    /// <summary>
    /// Type checking of relation objects and their corresponding types
    /// </summary>
    public static partial class RelationLogic
    {
        #region Non-Deterministic Relation Type Checking

        /// <summary>
        /// The output of relation creation can be Shape 
        /// or List<type>.
        /// </summary>
        /// <param name="obj1">Shape or Goal</param>
        /// <param name="obj2">Shape or Goal</param>
        /// <param name="output">Shape or Goal</param>
        /// <returns></returns>
        public static bool CreateRelation(object obj1, 
            object obj2, out object output)
        {
            output = null;
            var shape1 = obj1 as Shape;
            var shape2 = obj2 as Shape;
            var goal1 = obj1 as EqGoal;
            var goal2 = obj2 as EqGoal;
            if (shape1 != null && shape2 != null)
            {
                return CreateRelation(shape1, shape2, out output);
            }

            if (goal1 != null && goal2 != null)
            {
                return CreateRelation(goal1, goal2, out output);
            }

            if (shape1 != null && goal2 != null)
            {
                return CreateRelation(shape1, goal2, out output);
            }

            if (goal1 != null && shape2 != null)
            {
                return CreateRelation(shape2, goal1, out output);
            }

            return false;
        }

        private static bool CreateRelation(EqGoal goal1, EqGoal goal2,
                    out object output)
        {
            output = null;
            return false;
        }

        private static bool CreateRelation(Shape shape, EqGoal goal,
                    out object output)
        {
            output = null;
            return false;
        }

        private static bool CreateRelation(Shape shape1,
            Shape shape2, out object output)
        {
            output = null;
            var types = RelationRule.Exist(shape1.GetType(), shape2.GetType());
            if (types == null) return false;

            if (types.Count == 1)
            {
                ShapeType st = types[0];
                return CreateRelation(shape1, shape2, st, out output);
            }
            else
            {
                output = types;
                return false;
            }
        }

        #endregion

        #region Deterministic Relation Type Checking

        public static bool CreateRelation(object obj1,
            object obj2, ShapeType shapeType, out object output)
        {
            output = null;

            var shape1 = obj1 as Shape;
            var shape2 = obj2 as Shape;

            if (shape1 != null && shape2 != null)
            {
                return CreateRelation(shape1, shape2, shapeType, out output);
            }

            return false;
        }

        private static bool CreateRelation(Shape shape1,
            Shape shape2, ShapeType shapeType, out object output)
        {
            output = null;

            var pt1 = shape1 as Point;
            var pt2 = shape2 as Point;

            if (pt1 != null && pt2 != null)
            {
                return CreateRelation(pt1, pt2, shapeType, out output);
            }

            return false;
        }

        private static bool CreateRelation(Point pt1,
            Point pt2, ShapeType shapeType, out object output)
        {
            output = null;
            if (shapeType == ShapeType.Line)
            {
                output = LineRelation.Unify(pt1, pt2);
                if (output != null) return true;
                output = LineGenerationRule.IdentityPoints;
                return false;
            }
            else if (shapeType == ShapeType.LineSegment)
            {
                throw new Exception("TODO");
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
