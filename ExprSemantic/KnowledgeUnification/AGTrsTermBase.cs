using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using starPadSDK.MathExpr;

namespace ExprSemantic.KnowledgeUnification
{
    public abstract class AGTrsTermBase
    {
        public Expr AstSource { get; protected set; }

        /// <summary>
        /// Applies the given substituion to this term, creating a new term. This method is used by the public
        /// version to apply a collection of substitutions.
        /// </summary>
        protected abstract AGTrsTermBase ApplySubstitution(AGSubstitution substitution);

        /// <summary>
        /// Applies a set of substitutions to this term, swapping variables. If no substitution applies to a term,
        /// the term is returned.
        /// </summary>
        /// <param name="substitutoins"></param>
        /// <returns></returns>
        public AGTrsTermBase ApplySubstitutions(IEnumerable<AGSubstitution> substitutoins)
        {
            AGTrsTermBase retVal = this;
            foreach (var substitution in substitutoins) retVal = retVal.ApplySubstitution(substitution);
            return retVal;
        }

        public abstract override bool Equals(object other);

        public abstract override int GetHashCode();

        /// <summary>
        /// Create a copy of the current term.
        /// </summary>
        public abstract AGTrsTermBase CreateCopy();

        /// <summary>
        /// Checks if this term contains the given variable. This is part of the "occurs check" which 
        /// forms part of the MGU calculation.
        /// </summary>
        public abstract bool ContainsVariable(AGTrsVariable testVariable);

        /// <summary>
        /// Converts this term into a form that is parsable by the term parser.
        /// </summary>
        /// <returns></returns>
        public abstract string ToSourceCode();
    }
}
