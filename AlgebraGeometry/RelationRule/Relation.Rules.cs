using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgebraGeometry
{
    public static class RelationRule
    {
        public static List<ShapeType> Exist(Type type1, Type type2)
        {
            var supportTypes = new List<ShapeType>();
            if (type1.Name.Equals("Point") && type2.Name.Equals("Point"))
            {
                supportTypes.Add(ShapeType.Line);
                supportTypes.Add(ShapeType.LineSegment);
                return supportTypes;
            }

            return null;
        }
    }
}
