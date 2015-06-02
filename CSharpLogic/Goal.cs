using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLogic
{
    public abstract class Goal : DyLogicObject
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

       public static Goal GenerateGoal(object lhs, object rhs)
       {
           #region Term pre-evaluation

           var lTerm = lhs as Term;
           var lSteps = new List<TraceStep>();
           object lhsEval = null;
           if (lTerm != null)
           {
               lhsEval = lTerm.Eval();
               if (!lhsEval.Equals(lhs))
               {
                   lSteps.AddRange(lTerm.Traces);
               }
           }
           if (lhsEval == null)
           {
               lhsEval = lhs;
           }

           var rTerm = rhs as Term;
           var rSteps = new List<TraceStep>();
           object rhsEval = null;
           if (rTerm != null)
           {
               rhsEval = rTerm.Eval();
               if (!rhsEval.Equals(rhs))
               {
                   rSteps.AddRange(rTerm.Traces);
               }
           }
           if (rhsEval == null)
           {
               rhsEval = rhs;
           }

           #endregion

           #region lhs and rhs evaluation

           Goal goal = null;

           var llTerm = lhsEval as Term;
           var rrTerm = rhsEval as Term;
           if (llTerm != null && LogicSharp.IsNumeric(rhsEval))
           {
               var tuple = llTerm.Args as Tuple<object, object>;
               if(tuple == null) throw new Exception("Args cannot be null");
               if (llTerm.Op == Expression.Add)
               {
                   var addOp = new BinOp(Expression.Add, Expression.Subtract);
                   goal = addOp.GenerateGoal(tuple, rhsEval);    
               }
               else if (llTerm.Op == Expression.Subtract)
               {
                   var subOp = new BinOp(Expression.Subtract, Expression.Add);
                   goal = subOp.GenerateGoal(tuple, rhsEval);
               }
               else if (llTerm.Op == Expression.Multiply)
               {
                   var mulOp = new BinOp(Expression.Multiply, Expression.Divide);
                   goal = mulOp.GenerateGoal(tuple, rhsEval);
               }
               else if (llTerm.Op == Expression.Divide)
               {
                   var divOp = new BinOp(Expression.Divide, Expression.Multiply);
                   goal = divOp.GenerateGoal(tuple, rhsEval);
               }
           }
           else if (LogicSharp.IsNumeric(lhsEval) && rrTerm != null)
           {
               var tuple = rrTerm.Args as Tuple<object, object>;
               if (tuple == null) throw new Exception("Args cannot be null");
               if (rrTerm.Op == Expression.Add)
               {
                   var addOp = new BinOp(Expression.Add, Expression.Subtract);
                   goal = addOp.GenerateGoal(tuple, lhsEval);
               }
               else if (rrTerm.Op == Expression.Subtract)
               {
                   var subOp = new BinOp(Expression.Subtract, Expression.Add);
                   goal = subOp.GenerateGoal(tuple, lhsEval);
               }
               else if (rrTerm.Op == Expression.Multiply)
               {
                   var mulOp = new BinOp(Expression.Multiply, Expression.Divide);
                   goal = mulOp.GenerateGoal(tuple, lhsEval);
               }
               else if (rrTerm.Op == Expression.Divide)
               {
                   var divOp = new BinOp(Expression.Divide, Expression.Multiply);
                   goal = divOp.GenerateGoal(tuple, lhsEval);
               }
           }

           #endregion

           #region Goal construction

           if (goal != null)
           {
               goal.Traces = lSteps.Concat(rSteps)
                                   .Concat(goal.Traces)
                                   .ToList();
           }
           else
           {
               goal = new EqGoal(lhs, rhs);
               goal.Traces = lSteps.Concat(rSteps).ToList();
           }

           #endregion

           return goal;
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
                var obj = lTerm.Eval();
                if (!obj.Equals(Lhs))
                {
                    foreach (var step in lTerm.Traces)
                    {
                        Traces.Add(step); 
                    }
                    Lhs = obj;
                }
            }
            if (rTerm != null)
            {
                var obj = rTerm.Eval();
                if (!obj.Equals(Rhs))
                {
                    foreach (var step in rTerm.Traces)
                    {
                        Traces.Add(step);
                    }
                    Rhs = obj;
                }
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
