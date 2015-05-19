using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLogic
{
    /// <summary>
    /// Substitution or term
    /// </summary>
    public class Term
    {
        public Func<Expression, Expression, BinaryExpression> Op { get; set; }
        public object Args { get; set; }

        public Term(Func<Expression, Expression, BinaryExpression> _op, object _args)
        {
            Op = _op;
            Args = _args;
        }

        public override bool Equals(object obj)
        {
            if (obj is Term)
            {
                var term = obj as Term;
                return Op.Equals(term.Op) && Args.Equals(term.Args);
            }
            return false;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Op);
            builder.Append(LogicSharp.PrintTuple(Args));
            return builder.ToString();
        }

        public object Eval()
        {
            var tuple = Args as Tuple<object, object>;
            if (tuple != null)
            {
                if (!Var.ContainsVar(tuple.Item1) && !Var.ContainsVar(tuple.Item2))
                {
                    return LogicSharp.Calculate(Op, tuple.Item1, tuple.Item2);
                }                       
            }
            return this;
        }
    }

    public static class TermExtention
    {
        public static Term Reify(this Term term, Dictionary<object, object> s)
        {
            var gArgs = LogicSharp.Reify(term.Args, s);
            return new Term(term.Op, gArgs);
        }

        public static bool Unify(this Term term, Term otherTerm, Dictionary<object, object> s)
        {
            bool opUnifiable = LogicSharp.Unify(term.Op, otherTerm.Op, s);

            if (opUnifiable)
            {
                return LogicSharp.Unify(term.Args, otherTerm.Args, s);
            }
            return false;
        }
    }
}
