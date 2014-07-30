using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExprSemantic
{
    class AGTextParseException : Exception
    {
        public AGTextParseException()
        {

        }

        public AGTextParseException(string repr, Axiom entity)
        {
            Repr = repr;
            Entity = entity;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Entity.GetType().ToString())
                .Append("cannot parse the expression ")
                .Append(Repr);
            return builder.ToString();
        }

        public Axiom Entity { get; set; }
        public string Repr { get; set; }
    }
}
