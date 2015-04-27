using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using starPadSDK.MathExpr;

namespace ExprSemantic.KnowledgeUnification
{
    public class AGTrsTerm : AGTrsTermBase
    {
        public string Name { get; private set; }

        public List<AGTrsTermBase> Arguments { get; private set; }

        public AGTrsTerm(string name, IEnumerable<AGTrsTermBase> arguments, Expr sourceTerm) 
        {
          if (arguments == null || arguments.Count() == 0) 
            throw new ArgumentException("Expected at least one argument for term definition, otherwise use atom.");
          Name = name;
          Arguments = arguments.ToList();
          AstSource = sourceTerm;
        }

        public AGTrsTerm(string name, IEnumerable<AGTrsTermBase> arguments) 
          : this (name, arguments, null)
        {

        }

        public override bool Equals(object other)
        {
          var otherTerm = other as AGTrsTerm;
          if (otherTerm == null) return false;
          return Name.Equals(otherTerm.Name)
            && Arguments.Count == otherTerm.Arguments.Count
            && Enumerable.Range(0, Arguments.Count).Where(i => Arguments[i].Equals(otherTerm.Arguments[i])).Count() == otherTerm.Arguments.Count;
        }

        public override int GetHashCode()
        {
          int currCode = Name.GetHashCode();
          foreach (var arg in Arguments)
          {
            currCode = currCode ^ ~arg.GetHashCode();
          }
          return currCode;
        }

        public override bool ContainsVariable(AGTrsVariable testVariable)
        {
          return Arguments.Any(arg => arg.ContainsVariable(testVariable));
        }

        public override string ToSourceCode()
        {
          StringBuilder result = new StringBuilder();
          result.Append(Name);
          result.Append("(");
          // A term will always have at least one argument, otherwise it will not parse. "t()" is not a valid term, and should be written "t"
          result.Append(Arguments.First().ToSourceCode());
          foreach (var arg in Arguments.Skip(1))
          {
            result.Append(",");
            result.Append(arg.ToSourceCode());
          }
          result.Append(")");
          return result.ToString();
        }

        protected override AGTrsTermBase ApplySubstitution(AGSubstitution substitution)
        {
          return new AGTrsTerm(Name, Arguments.Select(arg => arg.ApplySubstitutions(new [] { substitution })));
        }

        public override AGTrsTermBase CreateCopy()
        {
          return new AGTrsTerm(Name, Arguments.Select(arg => arg.CreateCopy()));
        }

    }
}
