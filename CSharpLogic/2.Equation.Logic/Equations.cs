using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public interface IEquationLogic : IEval
    {
        Equation EvalEquation();
    }


    public static class EquationEvalExtension
    {
        /// <summary>
        /// if x = y, then y = x
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="gGoal"></param>
        /// <returns></returns>
        public static bool SymmetricProperty(this Goal goal, out Goal gGoal)
        {
            gGoal = null;
            return false;
        }

        /// <summary>
        /// if x = y and y = z, then x = z
        /// if x = y, then x + a = y + a
        /// if x = y, then ax = ay
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="gGoal"></param>
        /// <returns></returns>
        public static bool TransitiveProperty(this Goal goal, out Goal gGoal)
        {
            gGoal = null;
            return false;
        }

                    //Assert cannot be (Lhs is constant, Rhs contains variable)  

            /*
                else if(Var.IsVar(x) && Var.IsVar(y))
                {
                    var term1 = new Term(RevOp, new Tuple<object, object>(z, y));
                    return new EqGoal(x, term1);
                }
                else if(Var.IsVar(y) && Var.IsVar(z))
                {
                    var term1 = new Term(Op, new Tuple<object, object>(x, y));
                    return new EqGoal(term1, z);
                }
                else if(Var.IsVar(x) && Var.IsVar(z))
                {
                    //var oper = new Operator(RevOp.Method.Name);
                    var term1 = new Term(Op, new Tuple<object, object>(x, y));
                    return new EqGoal(term1, z);
                }
            */
          

            //Apply TransitiveProperty rule for the equality
            //if x = y, then x + a = y + a
            //if x = y, then ax = ay
    }
}
