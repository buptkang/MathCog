using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    public static class GraphExtension
    {
        #region Pattern Match String (Unification)

        /// <summary>
        /// This func takes charge of pattern match string with 
        /// existing nodes
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="label"></param>
        /// <param name="st"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool InferVariable(this RelationGraph graph,
            string label, ShapeType st, out object obj)
        {
            obj = null;
            Debug.Assert(label != null);

            if (label.ToCharArray().Length == 2)
            {
                string str1 = label.ToCharArray()[0].ToString();
                string str2 = label.ToCharArray()[1].ToString();

                object obj1 = graph.RetrieveObjectByLabel(str1);
                object obj2 = graph.RetrieveObjectByLabel(str2);

                if (obj1 == null || obj2 == null) return false;

                return RelationLogic.CreateRelation(obj1, obj2, st, out obj);
            }
            //TODO
            return false;
        }

        /// <summary>
        /// This func takes charge of pattern match string with 
        /// existing nodes
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="label"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool InferVariable(this RelationGraph graph, 
            string label, out object obj)
        {
            obj = null;
            Debug.Assert(label != null);

            if (label.ToCharArray().Length == 2)
            {
                string str1 = label.ToCharArray()[0].ToString();
                string str2 = label.ToCharArray()[1].ToString();

                object obj1 = graph.RetrieveObjectByLabel(str1);
                object obj2 = graph.RetrieveObjectByLabel(str2);

                if (obj1 == null || obj2 == null) return false;

                return RelationLogic.CreateRelation(obj1, obj2, out obj);            
            }
            //TODO
            return false; 
        }

        /// <summary>
        /// check EqGoal's variables exist in the graph's shape node or not 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="goal"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool InferVariable(this RelationGraph graph,
            Goal goal, out object obj)
        {
            obj = null;
            var eqGoal = goal as EqGoal;
            Debug.Assert(eqGoal != null);
            
            List<Var> vars = graph.RetrieveShapeInternalVars();
            if (vars.Count == 0) return false;

            foreach (Var tempVar in vars)
            {
                if (eqGoal.ContainsVar(tempVar))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool InferVariable(this RelationGraph graph,
             object inputObj, out object obj)
        {
            obj = null;
            var shape = inputObj as Shape;
            if (shape != null) return false;

            var goal = inputObj as EqGoal;
            if (goal != null) return graph.InferVariable(goal, out obj);

            return false;
        }

        #endregion
    }
}
