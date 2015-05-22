using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLogic
{
    public abstract class Goal
    {
        public abstract bool EarlySafe();

        public abstract bool Reify(Dictionary<object, object> substitutions);

        public abstract bool Unify(Dictionary<object, object> substitutions);

        //Evaluate Lhs and Rhs
        public abstract void Eval();
    }

    public class EqGoal : Goal
    {
       internal Func<Dictionary<object, object>, bool> Functor;

       public object Lhs { get; set; }
       public object Rhs { get; set; }

       public EqGoal(object lhs, object rhs)
       {
           Lhs = lhs;
           Rhs = rhs;
           var eq = LogicSharp.Equal();
           Functor = eq(Lhs, Rhs);
       }

       public override bool Reify(Dictionary<object, object> substitutions)
       {
           Lhs = LogicSharp.Reify(Lhs, substitutions);
           Rhs = LogicSharp.Reify(Rhs, substitutions);

           if (Var.ContainsVar(Lhs) || Var.ContainsVar(Rhs))
           {
               return true;
           }
           else
           {
               return Lhs.Equals(Rhs);               
           }
       }

       public override bool Unify(Dictionary<object, object> substitutions)
       {
          Eval();
          if (Lhs is Term || Rhs is Term)
          {
              return false;
          }
          return Functor(substitutions);
       }

        public override void Eval()
        {
            var lTerm = Lhs as Term;
            var rTerm = Rhs as Term;
            if (lTerm != null)
            {
                Lhs = lTerm.Eval();
            }
            if (rTerm != null)
            {
                Rhs = rTerm.Eval();
            }
            Functor = LogicSharp.Equal()(Lhs, Rhs);
        }

       public override bool EarlySafe()
       {
           return !(Var.ContainsVar(Lhs) && Var.ContainsVar(Rhs));
       }

        public override bool Equals(object obj)
        {
            var eqGoal = obj as EqGoal;
            if (eqGoal != null)
            {
                return Lhs.Equals(eqGoal.Lhs) && Rhs.Equals(eqGoal.Rhs);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Lhs.GetHashCode() ^ this.Rhs.GetHashCode();
        }
    }

    public static class EqGoalExtension
    {
        public static bool IsAssignment(this EqGoal goal)
        {
            //TODO
            if (Var.IsVar(goal.Lhs) && LogicSharp.IsNumeric(goal.Rhs))
            {
                return true;
            }
            else if(Var.IsVar(goal.Rhs) && LogicSharp.IsNumeric(goal.Lhs))
            {
                return true;
            }
            return false;
        }

        public static Dictionary<object, object> ToDict(this EqGoal goal)
        {
            object variable;
            object value;
            if (Var.IsVar(goal.Lhs))
            {
                variable = goal.Lhs;
                value = goal.Rhs;
            }
            else
            {
                variable = goal.Rhs;
                value = goal.Lhs;
            }

            var pair = new KeyValuePair<object, object>(variable, value);
            var substitute = new Dictionary<object, object>();
            substitute.Add(pair.Key, pair.Value);
            return substitute;
        }
    }
}
