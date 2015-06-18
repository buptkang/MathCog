using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public static class LineRelation
    {
        /// <summary>
        /// construct a line through two points
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static Line Unify(Point pt1, Point pt2)
        {
            //point identify check
            if (pt1.Equals(pt2)) return null;

            //Line build process
            if (pt1.Concrete && pt2.Concrete)
            {
                var line = LineGenerationRule.GenerateLine(pt1, pt2);
                return line;
            }
            else
            {
                //lazy evaluation    
                //Constraint solving on Graph
                var line = new Line(null); //ghost line
                return line;
            }
        }

        /// <summary>
        /// construct a line through a point and a goal,
        /// e.g A(1,2) ^ S = 2=> Conjunctive Norm Form
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static Line Unify(Point pt, EqGoal goal)
        {            
            throw new Exception("TODO");
        }

        public static Line Unify(EqGoal goal, Point pt)
        {
            return Unify(pt, goal);
        }

        /// <summary>
        /// construct a line through two goals
        /// e.g  m=2, k=3 => conjunctive norm form
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static Line Unify(EqGoal goal1, EqGoal goal2)
        {
            throw new Exception("TODO");            
        }
    }

    public static class LineSegRelation
    {
    }
}
