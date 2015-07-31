using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    /// <summary>
    /// Type checking of relation objects and their corresponding types
    /// </summary>
    public static partial class RelationLogic
    {
        #region Single Constraint (label or ShapeType)

        /// <summary>
        /// The output of relation creation can be Shape 
        /// or List<type>.
        /// </summary>
        /// <param name="obj1">Shape or Goal</param>
        /// <param name="obj2">Shape or Goal</param>
        /// <param name="st"></param>
        /// <param name="constraint"></param>
        /// <param name="output">Shape or Goal</param>
        /// <returns></returns>
        private static bool ConstraintCheck(GraphNode obj1,
            GraphNode obj2, object constraint, out object output)
        {
            Debug.Assert(constraint != null);

            output = null;
            var shape1 = obj1 as ShapeNode;
            var shape2 = obj2 as ShapeNode;
            var goal1 = obj1 as GoalNode;
            var goal2 = obj2 as GoalNode;
            var query1 = obj1 as QueryNode;
            var query2 = obj2 as QueryNode;

            //Combinatoric Pattern Match
            if (shape1 != null && shape2 != null) return ConstraintCheck(shape1, shape2, constraint, out output);
            if (goal1 != null && goal2 != null) return ConstraintCheck(goal1, goal2, constraint, out output);
            if (shape1 != null && goal2 != null) return ConstraintCheck(shape1, goal2, constraint, out output);
            if (goal1 != null && shape2 != null) return ConstraintCheck(shape2, goal1, constraint, out output);

            //TODO other relations
            return false;
        }

        private static bool ConstraintCheck(GoalNode goalNode1, GoalNode goalNode2,
                    object constraint, out object output)
        {
            Debug.Assert(constraint != null);
            var st = constraint as ShapeType?;
            var goal1 = goalNode1.Goal as EqGoal;
            var goal2 = goalNode2.Goal as EqGoal;
            Debug.Assert(goal1 != null);
            Debug.Assert(goal2 != null);
            output = null;
            if (st != null)
            {
                switch (st.Value)
                {
                    case ShapeType.Line:
                        output = LineRelation.Unify(goal1, goal2);
                        break;
                }
                return output != null;                
            }
            var label = constraint as string;
            if (label != null)
            {
                //TODO
                if (LineAcronym.EqualGeneralFormLabels(label))
                {
                    output = LineRelation.Unify(goal1, goal2);
                    var ls = output as LineSymbol;
                    if (ls != null)
                    {
                        ls.OutputType = LineType.GeneralForm;
                        return true;
                    }
                    return false;
                }
                if (LineAcronym.EqualSlopeInterceptFormLabels(label))
                {
                    output = LineRelation.Unify(goal1, goal2);
                    var ls = output as LineSymbol;
                    if (ls != null)
                    {
                        ls.OutputType = LineType.SlopeIntercept;
                        return true;
                    }
                    return false;                    
                }
            }
            return false;
        }

        private static bool ConstraintCheck(ShapeNode shapeNode, GoalNode goalNode,
                    object constraint, out object output)
        {
            output = null;
            //TODO
            return false;
        }

        private static bool ConstraintCheck(ShapeNode shapeNode1, ShapeNode shapeNode2,
                    object constraint, out object output)
        {
            output = null;
            var shape1 = shapeNode1.ShapeSymbol;
            var shape2 = shapeNode2.ShapeSymbol;
            Debug.Assert(shape1 != null);
            Debug.Assert(shape2 != null);
            var pt1 = shape1 as PointSymbol;
            var pt2 = shape2 as PointSymbol;
            if (pt1 != null && pt2 != null) return ConstraintCheck(pt1, pt2, constraint, out output);
            return false;
        }

        #endregion

        #region Two Constraint Solving (label and ShapeType)

        public static bool ConstraintCheck(GraphNode obj1,
            GraphNode obj2, object constraint1, object constraint2, out object output)
        {
            if (constraint1 == null) return ConstraintCheck(obj1, obj2, constraint2, out output);
            if (constraint2 == null) return ConstraintCheck(obj1, obj2, constraint1, out output);
            var label = constraint1 as string;
            if (DefaultLabels.EqualDefaultLabel(label)) return ConstraintCheck(obj1, obj2, constraint2, out output);
            var shapeType = constraint2 as ShapeType?; 
            Debug.Assert(label != null);
            Debug.Assert(shapeType != null);

            output = null;
            var shape1 = obj1 as ShapeNode;
            var shape2 = obj2 as ShapeNode;
            var goal1 = obj1 as GoalNode;
            var goal2 = obj2 as GoalNode;
            var query1 = obj1 as QueryNode;
            var query2 = obj2 as QueryNode;

            //Combinatoric Pattern Match
            if (shape1 != null && shape2 != null) return ConstraintCheck(shape1, shape2, label, shapeType.Value, out output);
            if (goal1 != null && goal2 != null) return ConstraintCheck(goal1, goal2, label, shapeType.Value, out output);
            if (shape1 != null && goal2 != null) return ConstraintCheck(shape1, goal2, label, shapeType.Value, out output);
            if (goal1 != null && shape2 != null) return ConstraintCheck(shape2, goal1, label, shapeType.Value, out output);

            //TODO other relations
            return false;
        }

        private static bool ConstraintCheck(ShapeNode shapeNode1, ShapeNode shapeNode2,
            string constraint1, ShapeType constraint2, out object output)
        {
            output = null;
            var shape1 = shapeNode1.ShapeSymbol;
            var shape2 = shapeNode2.ShapeSymbol;
            Debug.Assert(shape1 != null);
            Debug.Assert(shape2 != null);
            var pt1 = shape1 as PointSymbol;
            var pt2 = shape2 as PointSymbol;
            if (pt1 != null && pt2 != null) return ConstraintCheck(pt1, pt2, constraint1, constraint2, out output);
            return false;
        }

        private static bool ConstraintCheck(ShapeNode shapeNode, GoalNode goalNode,
            string constraint1, ShapeType constraint2 , out object output)
        {
            output = null;
            //TODO
            return false;
        }

        private static bool ConstraintCheck(GoalNode goalNode1, GoalNode goalNode2,
            string constraint1, ShapeType constraint2 , out object output)
        {
            output = null;
            return false;
        }

        #endregion
    }
}
