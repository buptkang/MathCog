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

    }
}
