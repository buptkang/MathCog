using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public partial class Equation : DyLogicObject, IEquationLogic
    {
        #region Properties and Constructors 

        public string Label { get; set; }
        public object Lhs { get; set; }
        public object Rhs { get; set; }
        public bool IsExpression { get { return Rhs == null; } }

        #endregion

        #region Utility Functions

        public bool ContainsVar(Var variable)
        {
            var lhsVar = Lhs as Var;
            bool result;
            if (lhsVar != null)
            {
                result = lhsVar.Equals(variable);
                if (result) return true;
            }

            var rhsVar = Rhs as Var;
            if (rhsVar != null)
            {
                result = Rhs.Equals(variable);
                if (result) return true;
            }
            return false;
        }

        #endregion

        #region Object Override functions

        public override bool Equals(object obj)
        {
            var eq = obj as Equation;
            if (eq != null)
            {
                return Lhs.Equals(eq.Lhs) && Rhs.Equals(eq.Rhs);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Lhs.GetHashCode() ^ this.Rhs.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}={1}", Lhs, Rhs);
        }

        #endregion
    }
}
