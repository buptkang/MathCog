using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using starPadSDK.MathExpr;

namespace ExprSemantic.KnowledgeUnification
{
    public abstract class AGTrsAtom : AGTrsTermBase
    {
        public string Value { get; protected set; }

        public override bool Equals(object other)
        {
            var otherAtom = other as AGTrsAtom;
            return otherAtom != null
              && otherAtom.Value.Equals(this.Value)
              && otherAtom.GetType().Equals(this.GetType()); 
            // Caters for typeing between strings, numbers and constants (see child-classes)
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool ContainsVariable(AGTrsVariable testVariable)
        {
            return false;
        }

        protected override AGTrsTermBase ApplySubstitution(AGSubstitution substitution)
        {
            // Atoms do not contain variables.
            return this;
        }
    }

    public class AGTrsNumber : AGTrsAtom
    {
        public AGTrsNumber(string value, Number source)
        {
            AstSource = source;
            Value = value;
        }

        public AGTrsNumber(string value)
            : this(value, null)
        {
        }

        public override string ToSourceCode()
        {
            return Value;
        }

        public override AGTrsTermBase CreateCopy()
        {
            return new AGTrsNumber(Value);
        }
    }

    public class AGTrsString : AGTrsAtom
    {
        public AGTrsString(string value, Sym source)
        {
            Value = value;
            AstSource = source;
        }

        public AGTrsString(string value)
            : this(value, null)
        {
        }

        public override string ToSourceCode()
        {
            return "\"" + Value + "\"";
        }

        public override AGTrsTermBase CreateCopy()
        {
            return new AGTrsString(Value);
        }
    }

    public class AGTrsConstant : AGTrsAtom
    {
        public AGTrsConstant(string value, Sym source)
        {
            Value = value;
            AstSource = source;
        }

        public AGTrsConstant(string value) : this(value, null) { }

        public override string ToSourceCode()
        {
            return Value;
        }

        public override AGTrsTermBase CreateCopy()
        {
            return new AGTrsConstant(Value);
        }
    }
}