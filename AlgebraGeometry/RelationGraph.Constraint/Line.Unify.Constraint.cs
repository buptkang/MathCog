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
    public static partial class RelationLogic
    {
        private static bool ConstraintCheck(PointSymbol pt1, PointSymbol pt2,
            object constraint, out object output)
        {
            output = null;
            Debug.Assert(constraint != null);
            var shapeType = constraint as ShapeType?;
            if (shapeType != null)
            {
                #region ShapeType Constraint Solving
                if (shapeType == ShapeType.Line)
                {
                    output = LineRelation.Unify(pt1, pt2);
                    if (output != null) return true;
                    output = LineGenerationRule.IdentityPoints;
                    return false;
                }

                if (shapeType == ShapeType.LineSegment)
                {
                    output = LineSegRelation.Unify(pt1, pt2);
                    if (output != null) return true;
                    output = LineSegmentGenerationRule.IdentityPoints;
                    return false;
                }

                return false;
                #endregion
            }
            var label = constraint as string;
            if (label != null)
            {
                #region Label Constraint Solving

                //Case 2
                if (LineAcronym.EqualGeneralFormLabels(label))
                {
                    output = LineRelation.Unify(pt1, pt2);
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
                    output = LineRelation.Unify(pt1, pt2);
                    var ls = output as LineSymbol;
                    if (ls != null)
                    {
                        ls.OutputType = LineType.SlopeIntercept;
                        return true;
                    }
                    return false;
                }

                //Case 1
                char[] charr = label.ToCharArray();
                if (charr.Length != 2) return false;
                string str1 = label.ToCharArray()[0].ToString(CultureInfo.InvariantCulture);
                string str2 = label.ToCharArray()[1].ToString(CultureInfo.InvariantCulture);
                string label1 = pt1.Shape.Label;
                string label2 = pt2.Shape.Label;
                if (label1 == null || label2 == null) return false;
                bool condition1 = label1.Equals(str1) && label2.Equals(str2);
                bool condition2 = label1.Equals(str2) && label2.Equals(str1);
                if (condition1 || condition2)
                {
                    var supportTypes = new List<ShapeType> { ShapeType.Line, ShapeType.LineSegment };
                    output = supportTypes;
                    return false;
                }

                #endregion
            }
            return false;
        }

        private static bool ConstraintCheck(PointSymbol pt1, PointSymbol pt2,
            string constraint1, ShapeType constraint2, out object output)
        {
            output = null;
            Debug.Assert(constraint1 != null);

            var label = constraint1 as string;
            char[] charr = label.ToCharArray();
            if (charr.Length != 2) return false;
            string str1 = label.ToCharArray()[0].ToString(CultureInfo.InvariantCulture);
            string str2 = label.ToCharArray()[1].ToString(CultureInfo.InvariantCulture);
            string label1 = pt1.Shape.Label;
            string label2 = pt2.Shape.Label;

            bool condition1 = label1.Equals(str1) && label2.Equals(str2);
            bool condition2 = label1.Equals(str2) && label2.Equals(str1);
            if (condition1 || condition2)
            {
                if (constraint2 == ShapeType.Line)
                {
                    output = LineRelation.Unify(pt1, pt2);
                    if (output != null) return true;
                    output = LineGenerationRule.IdentityPoints;
                    return false;
                }

                if (constraint2 == ShapeType.LineSegment)
                {
                    output = LineSegRelation.Unify(pt1, pt2);
                    if (output != null) return true;
                    output = LineSegmentGenerationRule.IdentityPoints;
                    return false;
                }
            }
            return false; 
        }
    
    }
}
