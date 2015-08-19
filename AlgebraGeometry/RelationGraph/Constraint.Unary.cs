using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSharpLogic;
using NUnit.Framework.Constraints;

namespace AlgebraGeometry
{
    /// <summary>
    /// Type checking of relation objects and their corresponding types
    /// </summary>
    public static partial class RelationLogic
    {
        public static bool ConstraintCheck(GraphNode obj1, object constraint1, object constraint2, out object output)
        {
            if (constraint1 == null) return ConstraintCheck(obj1, constraint2, out output);
            var label = constraint1 as string;
            if (DefaultLabels.EqualDefaultLabel(label)) return ConstraintCheck(obj1, constraint2, out output);

            if (constraint2 == null) return ConstraintCheck(obj1, constraint1, out output);
          
            var shapeType = constraint2 as ShapeType?;
            Debug.Assert(label != null);
            Debug.Assert(shapeType != null);

            output = null;
            var shape1 = obj1 as ShapeNode;
            var goal1 = obj1 as GoalNode;
            var query1 = obj1 as QueryNode;

            //Combinatoric Pattern Match
            if (shape1 != null) return ConstraintCheck(shape1, constraint1, constraint2, out output);
            if (goal1 != null)  return ConstraintCheck(goal1, constraint1, constraint2, out output);
            if (query1 != null) return ConstraintCheck(query1, constraint1, constraint2, out output);
            //TODO other relations
            return false;
        }

        private static bool ConstraintCheck(ShapeNode sn, object constraint1, object constraint2, out object output)
        {
            output = null;
            return false;
        }

        private static bool ConstraintCheck(GoalNode gn, object constraint1, object constraint2, out object output)
        {
            output = null;
            return false;
        }

        private static bool ConstraintCheck(QueryNode gn, object constraint1, object constraint2, out object output)
        {
            output = null;
            return false;
        }

        private static bool ConstraintCheck(GraphNode gn, object constraint, out object output)
        {
            var shapeNode = gn as ShapeNode;
            var goalNode  = gn as GoalNode;
            var queryNode = gn as QueryNode;
            if (shapeNode != null) return ConstraintCheck(shapeNode, constraint, out output);
            if (goalNode != null) return ConstraintCheck(goalNode, constraint, out output);
            if (queryNode != null) return ConstraintCheck(queryNode, constraint, out output);

            throw new Exception("Graph.Unify.UnaryConstraint.cs: Cannot reach here");
        }

        /// <summary>
        /// e.g given  y = 2x + 1, ask: m = ?
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="constraint"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static bool ConstraintCheck(ShapeNode sn, object constraint, out object output)
        {
            output = null;
            var label = constraint as string;
            if (label != null)
            {
                return sn.ShapeSymbol.UnifyProperty(label, out output); 
            }
            return false;
        }

        private static bool ConstraintCheck(GoalNode gn, object constraint, out object output)
        {
            output = null;
            var label = constraint as string;
            var eqGoal = gn.Goal as EqGoal;
            Debug.Assert(eqGoal != null);
            if (label != null && gn.Satisfy(label))
            {
                output = new EqGoal(new Var(label), eqGoal.Rhs);
                return true;
            }
            var variable = constraint as Var;
            if (variable != null && gn.Satisfy(variable))
            {
                output = new EqGoal(variable, eqGoal.Rhs);
                return true;
            }
            return false;
        }

        private static bool ConstraintCheck(QueryNode gn, object constraint, out object output)
        {
            output = null;
            var st = constraint as ShapeType?;
            if (st != null)
            {
                //TODO
                throw new Exception("TODO");
            }
            var label = constraint as string;
            if (label != null)
            {
                var nodes = gn.InternalNodes;
                //TODO
                foreach (var node in nodes)
                {
                    var goalNode = node as GoalNode;
                    if (goalNode != null)
                    {
                        bool result = ConstraintCheck(goalNode, label, out output);
                        if (result) goalNode.Related = true;
                        return result;
                    }
                    var shapeNode = node as ShapeNode;
                    if (shapeNode != null)
                    {
                        bool result = ConstraintCheck(shapeNode, label, out output);
                        if (result) shapeNode.Related = true;
                        return result;
                    }
                }
            }
            return false;
        }
    }
}
