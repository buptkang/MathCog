using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            return EvalAlgebra();
        }

        /// <summary>
        /// Arithmetic Evaluation
        /// </summary>
        /// <returns></returns>
        public object EvalArithmetic()
        {
            return this.Arithmetic(this);
        }

        /// <summary>
        /// Algebra Evaluation, it embed Arithmetic Evaluation
        /// </summary>
        /// <returns>The latest derived term</returns>
        public object EvalAlgebra()
        {
            return this.AlgebraLaws(this);
        }

        #region Evaluation Algorithm 

        /// <summary>
        /// BFS
        /// </summary>
        /// <param name="rootTerm"></param>
        /// <returns></returns>
        public object AlgebraLaws(Term rootTerm)
        {
            bool hasChange;
            object localObj;
            object localTerm0 = this;
            do
            {
                hasChange = false;
                var localTerm1 = localTerm0.ApplyCommutative(rootTerm);
                if (!localTerm1.Equals(localTerm0))
                {
                    hasChange = true;
                    localTerm0 = localTerm1;
                }
                var localTerm2 = localTerm1.ApplyIdentity(rootTerm);
                if (!localTerm2.Equals(localTerm1))
                {
                    hasChange = true;
                    localTerm0 = localTerm2;
                }
                var localTerm3 = localTerm2.ApplyZero(rootTerm);
                if (!localTerm3.Equals(localTerm2))
                {
                    hasChange = true;
                    localTerm0 = localTerm3;
                }
                var localTerm4 = localTerm3.ApplyDistributive(rootTerm);
                if (!localTerm4.Equals(localTerm3))
                {
                    hasChange = true;
                    localTerm0 = localTerm4;
                }
                var localTerm5 = localTerm4.ApplyAssociative(rootTerm);
                if (!localTerm5.Equals(localTerm4))
                {
                    hasChange = true;
                    localTerm0 = localTerm5;
                }
                localObj = localTerm5.Arithmetic(rootTerm);
                if (!localObj.Equals(localTerm5))
                {
                    hasChange = true;
                    localTerm0 = localObj;
                }
            } while (hasChange);

            return localObj;
        }

        /// <summary>
        /// DFS: Depth First Search
        /// </summary>
        /// <param name="term"></param>
        /// <param name="rootTerm">Trace generation purpose</param>
        /// <returns></returns>
        public Term DepthFirstSearch(Term rootTerm)
        {
            var lst = Args as List<object>;
            Debug.Assert(lst != null);
            Term localTerm = this;
            int index = 0;
            while (index < lst.Count)
            {
                var tempTerm = lst[index] as Term;
                if (tempTerm != null)
                {
                    object gTerm = tempTerm.AlgebraLaws(rootTerm);
                    if (!gTerm.Equals(tempTerm))
                    {
                        var cloneTerm = localTerm.Clone();
                        var cloneLst = cloneTerm.Args as List<object>;
                        Debug.Assert(cloneLst != null);
                        cloneLst[index] = gTerm;
                        lst = cloneLst;
                        localTerm = cloneTerm;
                    }
                }
                index++;
            }
            return localTerm;
        }

        public void Beautify()
        {
            var lst = Args as List<object>;
            Debug.Assert(lst != null);

            for (int i = 0; i < lst.Count; i++)
            {
                var localTerm = lst[i] as Term;
                if (localTerm != null)
                {
                    localTerm.Beautify();
                }
                //remove identity and minus
                if (Op.Method.Name.Equals("Multiply"))
                {
                    if (lst[i].Equals(1))
                    {
                        lst.RemoveAt(0);
                    }
                }
            }
        }

        #endregion

    }    
}