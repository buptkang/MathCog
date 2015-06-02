using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpLogic;

namespace AlgebraGeometry
{
    /// <summary>
    /// AGLogic Derive from LogicSharp to make inference
    /// </summary>
    public class AGLogic
    {
        public static object Infer(Var variable, Goal goal)
        {
            var eqGoal = goal as EqGoal;
            if (eqGoal != null)
            {
                return Infer(variable, eqGoal);
            }
            return null;
        }

        private static object Infer(Var variable, EqGoal eqGoal)
        {
            return LogicSharp.Run(variable, eqGoal);
        }

        public static object Infer(Var variable, Shape obj)
        {
            var point = obj as Point;
            if (point != null)
            {
                return Infer(variable, point);
            }

            return null;
            //TODO, line, circle
        }

        private static object Infer(Var variable, Point pt)
        {
            if (pt.Properties.ContainsKey(variable))
            {
                return pt.Properties[variable];
            }
            else
            {
                if (variable.Token.ToString().Equals("x") ||
                    variable.Token.ToString().Equals("X"))
                {
                    return pt.XCoordinate;
                }
                else if (variable.Token.ToString().Equals("Y") ||
                         variable.Token.ToString().Equals("y"))
                {
                    return pt.YCoordinate;
                }
                else
                {
                    return null;
                }
            }               
        }
    }
}
