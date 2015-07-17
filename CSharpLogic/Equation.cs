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
        public bool IsGenerated { get; set; }

        public Equation(string label, object lhs, 
                        object rhs, bool generated = false)
        {
            Label = label;
            Lhs = lhs;
            Rhs = rhs;
            IsGenerated = generated;
        }

        public Equation(object lhs, object rhs) 
            : this(null, lhs, rhs, false)
        {}

        public Equation(object lhs)
            : this(null, lhs, null, false)
        {}

        public Equation(object lhs, object rhs, bool generated)
            : this(null, lhs, rhs, generated)
        {}

        #endregion

        #region Utility Functions

        public bool ContainsVar()
        {
            var lhsVar = Lhs as Var;
            var rhsVar = Rhs as Var;

            if (lhsVar != null || rhsVar != null) return true;

            var lhsTerm = Lhs as Term;
            var rhsTerm = Rhs as Term;

            if (lhsTerm != null && lhsTerm.ContainsVar()) return true;
            if (rhsTerm != null && rhsTerm.ContainsVar()) return true;
            return false;
        }

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

        public Equation Clone()
        {
            var equation = (Equation)this.MemberwiseClone();

            var lhs = Lhs as Term;
            if (lhs != null)
            {
                equation.Lhs = lhs.Clone();
            }

            if (IsExpression)
            {
                var rhs = Rhs as Term;
                if (rhs != null)
                {
                    equation.Rhs = rhs.Clone();
                }
            }

            return equation;
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
            return string.Format("{0}={1}", Lhs.ToString(), Rhs.ToString());
        }

        #endregion
    }
}
