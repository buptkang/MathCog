using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExprSemantic
{
    public class Substitution
    {
        /// <summary>
        /// Variable to be substituted.
        /// </summary>
        public TrsVariable Variable { get; set; }

        /// <summary>
        /// Term to substitute the variable with. This term must be a clone of an existing term, 
        /// because the we do not want to overwrite the working set of terms.
        /// </summary>
        public TrsDataStructures SubstitutionTerm { get; set; }

        public string ToSourceCode()
        {
            return Variable.ToSourceCode() + " => " + SubstitutionTerm.ToSourceCode();
        }

        public override bool Equals(object obj)
        {
            Substitution other = obj as Substitution;
            return other != null
              && Variable.Equals(other.Variable)
              && SubstitutionTerm.Equals(other.SubstitutionTerm);
        }

        public override int GetHashCode()
        {
            return Variable.GetHashCode() ^ ~SubstitutionTerm.GetHashCode();
        }
    }
}
