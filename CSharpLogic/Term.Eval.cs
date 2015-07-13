using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace CSharpLogic
{
    /// <summary>
    /// Substitution or term
    /// </summary>
    public partial class Term : DyLogicObject
        , IArithmeticLogic, IAlgebraLogic
    {
        /// <summary>
        /// Evaluation Pipeline: 
        /// 1. Algebra Eval 
        /// 2. Arithmetic Eval
        /// 
        ///  "2+2",  "2+3-1", "2+2*2", "x+(1+2)", "x + y + 3"
        ///  "x^2+x+2+1" 
        ///  "(x+1)+2" or "1+x+2", "x+x"
        /// </summary>
        /// <returns></returns>
        public object Eval()
        {
            var lst = Args as List<object>;
            if (lst == null) throw new Exception("Cannot be null");

            //Evaluation Loop Algorithm:
            //1: Outer Loop Algebra Eval
            //2: Inner Loop Arithmetic Eval
            Term term1 = EvalAlgebra(); 
            object obj = term1.EvalArithmetic();
            //transform term1 trace onto term
            if(!term1.Equals(this))
                TraceUtils.MoveTraces(term1, this);
            return obj;
        }

        /// <summary>
        /// Arithmetic Evaluation
        /// </summary>
        /// <returns></returns>
        public object EvalArithmetic()
        {
            return this.Calc();
        }

        /// <summary>
        /// Algebra Evaluation, it embed Arithmetic Evaluation
        /// </summary>
        /// <returns>The latest derived term</returns>
        public Term EvalAlgebra()
        {
            if (Op == Expression.Add)
            {
                //Associative Law
                Term term = this.Associative();
                //Commutative Law
                term.Commutative();
                TraceUtils.MoveTraces(term, this);
                //Identity Law
                //Inverse Law
                return term;
            }
            else if (Op == Expression.Subtract)
            {
                return this;
            }
            else if (Op == Expression.Multiply)
            {
                //Distributive law
                Term term = this.Distribute();
                //identity laws
                return term;
            }
            throw new Exception("Term.Eval.cs: cannot reach here");
        }

        
    }    
}