// /*******************************************************************************
//  * Analytical Geometry Semantic Parsing 
//  * <p>
//  * Copyright (C) 2014  Bo Kang, Hao Hu
//  * <p>
//  * This program is free software; you can redistribute it and/or modify it under
//  * the terms of the GNU General Public License as published by the Free Software
//  * Foundation; either version 2 of the License, or any later version.
//  * <p>
//  * This program is distributed in the hope that it will be useful, but WITHOUT
//  * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//  * FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
//  * details.
//  * <p>
//  * You should have received a copy of the GNU General Public License along with
//  * this program; if not, write to the Free Software Foundation, Inc., 51
//  * Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
//  ******************************************************************************/

namespace ExprSemantic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using starPadSDK.MathExpr;

    public abstract class TrsDataStructures
    {
        public Expr AstSource { get; protected set; }

        /// <summary>
        /// Applies the given substituion to this term, creating a new term. This method is used by the public
        /// version to apply a collection of substitutions.
        /// </summary>
        protected abstract TrsDataStructures ApplySubstitution(Substitution substitution);

        /// <summary>
        /// Applies a set of substitutions to this term, swapping variables. If no substitution applies to a term,
        /// the term is returned.
        /// </summary>
        /// <param name="substitutoins"></param>
        /// <returns></returns>
        public TrsDataStructures ApplySubstitutions(IEnumerable<Substitution> substitutoins)
        {
            TrsDataStructures retVal = this;
            foreach (var substitution in substitutoins) retVal = retVal.ApplySubstitution(substitution);
            return retVal;
        }

        public abstract override bool Equals(object other);

        public abstract override int GetHashCode();

        /// <summary>
        /// Create a copy of the current term.
        /// </summary>
        public abstract TrsDataStructures CreateCopy();

        /// <summary>
        /// Checks if this term contains the given variable. This is part of the "occurs check" which 
        /// forms part of the MGU calculation.
        /// </summary>
        public abstract bool ContainsVariable(TrsVariable testVariable);

        /// <summary>
        /// Converts this term into a form that is parsable by the term parser.
        /// </summary>
        /// <returns></returns>
        public abstract string ToSourceCode();
    }

    /// <summary>
    /// Common base class for variables, numbers and constants, all of which is regarded as atoms
    /// </summary>
    public abstract class TrsAtom : TrsDataStructures
    {
        public string Value { get; protected set; }

        public override bool Equals(object other)
        {
            var otherAtom = other as TrsAtom;
            return otherAtom != null
              && otherAtom.Value.Equals(this.Value)
              && otherAtom.GetType() == GetType(); // Caters for typeing between strings, numbers and constants (see child-classes)
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool ContainsVariable(TrsVariable testVariable)
        {
            return false;
        }

        protected override TrsDataStructures ApplySubstitution(Unification.Substitution substitution)
        {
            // Atoms do not contain variables.
            return this;
        }
    }

    public class TrsNumber : TrsAtom
    {
        public TrsNumber(string value, AstNumber source)
        {
            AstSource = source;
            Value = value;
        }

        public TrsNumber(string value)
          : this(value, null)
        {
        }

        public override string ToSourceCode()
        {
            return Value;
        }

        public override TrsTermBase CreateCopy()
        {
            return new TrsNumber(Value);
        }
    }

    public class TrsString : TrsAtom
    {
        public TrsString(string value, AstString source)
        {
            Value = value;
            AstSource = source;
        }

        public TrsString(string value) : this(value, null)
        {
        }

        public override string ToSourceCode()
        {
            return "\"" + Value + "\"";
        }

        public override TrsTermBase CreateCopy()
        {
            return new TrsString(Value);
        }
    }

    public class TrsConstant : TrsAtom
    {
        public TrsConstant(string value, AstConstant source)
        {
            Value = value;
            AstSource = source;
        }

        public TrsConstant(string value) : this(value, null) { }

        public override string ToSourceCode()
        {
            return Value;
        }

        public override TrsTermBase CreateCopy()
        {
            return new TrsConstant(Value);
        }
    }


}
