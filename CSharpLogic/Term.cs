using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CSharpLogic
{
    /// <summary>
    /// Substitution or term
    /// </summary>
    public partial class Term : DyLogicObject,
                IArithmeticLogic, IAlgebraLogic
    {
        #region Properties and Constructors

        public Func<Expression, Expression, BinaryExpression> Op { get; set; }
        public object Args { get; set; }

        public Term(Func<Expression, Expression, BinaryExpression> _op, object _args)
        {
            Op = _op;
            Args = _args;
        }

        #endregion

        public Term Reify(Dictionary<object, object> s)
        {
            var gArgs = LogicSharp.Reify(Args, s);
            return new Term(Op, gArgs);
        }

        #region Utility Functions

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

            var tuple = Args as Tuple<object, object>;
            if (tuple != null)
            {
                #region Tuple Format
                builder.Append('(');

                var lTerm = tuple.Item1;
                var rTerm = tuple.Item2;

                builder.Append(lTerm.ToString());

                if (Op.Method.Name.Equals("Add"))
                {
                    builder.Append('+');
                }
                else if (Op.Method.Name.Equals("Substract"))
                {
                    builder.Append('-');
                }
                else if (Op.Method.Name.Equals("Multiply"))
                {
                    builder.Append('*');
                }
                else if (Op.Method.Name.Equals("Divide"))
                {
                    builder.Append('/');
                }

                builder.Append(rTerm.ToString());
                builder.Append(')');
                #endregion
            }

            var lst = Args as List<object>;
            if (lst != null)
            {
                #region List Format



                #endregion
            }

            return builder.ToString();
        }

        public bool ContainsVar()
        {
            var lst = Args as List<object>;
            Debug.Assert(lst != null);
            foreach (var obj in lst)
            {
                var variable = obj as Var;
                if (variable != null) return true;

                var term1 = obj as Term;
                if (term1 != null && term1.ContainsVar()) return true;
            }
            return false;
        }

        public bool ContainsVar(Var variable)
        {
            var tuple1 = Args as Tuple<object>;
            if (tuple1 != null)
            {
                var term1 = tuple1.Item1 as Term;
                if (term1 != null) return term1.ContainsVar(variable);
                var variable1 = tuple1.Item1 as Var;
                if (variable1 != null) return variable1.Equals(variable);
                return false; //constant
            }

            var tuple2 = Args as Tuple<object, object>;
            if (tuple2 != null)
            {
                bool result;
                var term1 = tuple2.Item1 as Term;
                if (term1 != null)
                {
                    result = term1.ContainsVar(variable);
                    if (result) return true;
                }
                var variable1 = tuple2.Item1 as Var;
                if (variable1 != null)
                {
                    result = variable1.Equals(variable);
                    if (result) return true;
                }

                var term2 = tuple2.Item2 as Term;
                if (term2 != null)
                {
                    result = term2.ContainsVar(variable);
                    if (result) return true;
                }
                var variable2 = tuple2.Item2 as Var;
                if (variable2 != null)
                {
                    result = variable.Equals(variable2);
                    if (result) return true;
                }
                return false;
            }

            throw new Exception("Term.cs: Cannot reach here");
        }

        public Term Clone()
        {
            var term = (Term)this.MemberwiseClone();
            var newlst = new List<object>();

            var lst = Args as List<object>;
            Debug.Assert(lst != null);
            foreach (object obj in lst)
            {
                var variable = obj as Var;
                if (variable != null)
                {
                    newlst.Add(variable.Clone());
                    continue;
                }

                var localTerm = obj as Term;
                if (localTerm != null)
                {
                    newlst.Add(localTerm.Clone());
                    continue;
                }

                newlst.Add(obj);
            }
            term.Args = newlst;
            return term;
        }
        #endregion
    }
}
