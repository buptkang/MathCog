using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AlgebraGeometry;

namespace ExprSemantic
{
    public static class RelationGraphExtension
    {
        public static bool FindNodes(this RelationGraph graph, char[] charArr, out object relationObj)
        {
            relationObj = null;
            if (charArr.Length == 2)
            {
                var tuple2 = new Tuple<string, string>
                    (charArr[0].ToString(CultureInfo.InvariantCulture), 
                     charArr[1].ToString(CultureInfo.InvariantCulture));
                return graph.FindBinaryNodes(tuple2, out relationObj);
            }
            else
            {
                //TODO
                return false;
            }
        }

        private static bool FindBinaryNodes(this RelationGraph graph, 
            Tuple<string, string> tuple, out object relationObj)
        {
            relationObj = null;
            Shape shape1 = graph.FindShapeNodeByLabel(tuple.Item1);
            Shape shape2 = graph.FindShapeNodeByLabel(tuple.Item2);
            if (shape1 != null && shape2 != null)
            {
                
            }
            else
            {
                return false;
            }
        }

        private static Shape FindShapeNodeByLabel(this RelationGraph graph, string label)
        {
            foreach (Shape shape in graph.RetrieveShapes())
            {
                if (label.Equals(shape.Label))
                {
                    return shape;
                }
            }
            return null;
        }
    }
}