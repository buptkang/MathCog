using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using starPadSDK.MathExpr;

namespace ExprSemantic.KnowledgeUnification
{
    public class AGTrsVariable : AGTrsTermBase
    {
        public string Name { get; private set; }

        public AGTrsVariable(string name, LetterSym astSource)
        {
          Name = name;
          AstSource = astSource;
        }

        public AGTrsVariable(string name) : this(name, null) { }

        public override bool Equals(object other)
        {
          var otherVar = other as AGTrsVariable;
          return otherVar != null && otherVar.Name.Equals(this.Name);
        }

        public override int GetHashCode()
        {      
          // Use negation to distinguish bestween variables and atoms in terms of hash codes. 
          // Better hash functions are possible but not explored here.
          return ~Name.GetHashCode();
        }

        public override bool ContainsVariable(AGTrsVariable testVariable)
        {
           return testVariable.Equals(this);
        }

        public override string ToSourceCode()
        {
          return ":" + Name;
        }

        protected override AGTrsTermBase ApplySubstitution(AGSubstitution substitution)
        {
          if (substitution.Variable.Equals(this)) return substitution.SubstitutionTerm;
          else return this;
        }

        public override AGTrsTermBase CreateCopy()
        {
          return new AGTrsVariable(Name);
        }
    }
}
