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
        public static bool VerifyRelation(Shape relationObj, object nonRelObj)
        {
            //Debug.Assert(relationObj.RelationStatus);
            var line = relationObj as Line;
            if (line != null) return VerifyRelation(line, nonRelObj);        
            var lineSegment = relationObj as LineSegment;
            if (lineSegment != null) return VerifyRelation(lineSegment, nonRelObj);
            return false;
        }

        private static bool VerifyRelation(LineSegment lineSeg, object dependent)
        {
            var point = dependent as Point;
            if (point == null) return false;

            //Label Verification between two shapes
            //Label Verification between two shapes
            string relLabel = lineSeg.Label;
            string nonRelLabel = point.Label;

            if (nonRelLabel == null) return false;
            Debug.Assert(relLabel != null);
            bool result= relLabel.Contains(nonRelLabel);
            if (result)
            {
                if (relLabel.Substring(0, 1).Equals(nonRelLabel))
                {
                    lineSeg.Pt1 = point;
                }
                if (relLabel.Substring(1, 1).Equals(nonRelLabel))
                {
                    lineSeg.Pt2 = point;
                }
            }
            return result;
        }

        private static bool VerifyRelation(Line line, object dependent)
        {
            var point = dependent as Point;
            if (point != null)
            {
                //Label Verification between two shapes
                string relLabel = line.Label;
                string nonRelLabel = point.Label;

                if (nonRelLabel == null) return false;
                if (relLabel == null) return false;
                return relLabel.Contains(nonRelLabel);                
            }

            var variable = dependent as Var; //EqGoal
            if (variable != null)
            {
                return LineAcronym.Properties
                    .Any(str => variable.ToString().Equals(str));
            }
            return false;
        }
    }
}
