using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CSharpLogic
{
    public interface IAlgebraLogic : IEval
    {
        /// <summary>
        // 1.1 Distributive law a(b+c) <=> ab+ac
        // 1.2 Associative  law 
        // 1.3 Commutative  law
        // 1.4 Identity     law x + 0 => x, x*1 =>1
        // 1.5 Inverse      law -a + a = 0
        /// </summary>
        /// <returns>value of term</returns>
        Term EvalAlgebra();
    }

    public static class AlgebraEvalExtension
    {
        /// <summary>
        /// Commutative Law 
        /// Algorithm: Bubble Sort
        /// </summary>
        /// <param name="term"></param>
        public static Term Commutative(this Term term)
        {
            bool madeChanges; 
            Term localTerm = term;
            do
            {
                var list = localTerm.Args as List<object>;
                if (list == null) throw new Exception("Cannot be null");
                int itemCount = list.Count;
                madeChanges = false;
                itemCount--;
                for (var i = 0; i < itemCount; i++)
                {
                    list = localTerm.Args as List<object>;
                    Debug.Assert(list != null);
                    if (SatisfyCommutativeCondition(list[i], list[i + 1]))
                    {
                        var cloneTerm = localTerm.Clone();
                        var cloneLst = cloneTerm.Args as List<object>;
                        Debug.Assert(cloneLst != null);
                        object tempObj = cloneLst[i];
                        cloneLst[i] = cloneLst[i + 1];
                        cloneLst[i + 1] = tempObj;
                        madeChanges = true;

                        //generate trace rule
                        string rule = AlgebraRule.Rule(
                            AlgebraRule.AlgebraRuleType.Commutative,
                            list[i], list[i + 1]);

                        var ts = new TraceStep(localTerm, cloneTerm, rule);
                        term.Traces.Add(ts);
                        localTerm = cloneTerm;
                    }
                }
            } while (madeChanges);
            return localTerm;
        }

        //commutative law
        private static bool SatisfyCommutativeCondition(object obj1, object obj2)
        {
            if (LogicSharp.IsNumeric(obj1))
            {
                var variable = obj2 as Var;
                if (variable != null) return true;

                var term = obj2 as Term;
                if (term != null && term.ContainsVar()) return true;
            }

            var term1 = obj1 as Term;
            if (term1 != null && !term1.ContainsVar())
            {
                var variable = obj2 as Var;
                if (variable != null) return true;

                var term = obj2 as Term;
                if (term != null && term.ContainsVar()) return true;
            }

            return false;
        }

        /// <summary>
        /// Associative Law
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public static Term Associative(this Term term)
        {
            //TODO
            return term;
        }

        public static Term Distribute(this Term term)
        {
            //TODO
            return term;
        }

       /* /// <summary>
        /// x+(y+z) == (x+y)+zm, x(yz) == (xy)z
        /// </summary>
        /// <param name="term"></param>
        /// <param name="gTerm"></param>
        /// <returns></returns>
        public static bool AssociativeLaw(this Term term, out Term gTerm)
        {
            gTerm = null;
            var tuple = term.Args as Tuple<object, object>;
            if (tuple == null) throw new Exception("cannot be null");
            var arg1Term = tuple.Item1 as Term;
            if (arg1Term != null)
            {
                if (term.Op.Equals(arg1Term.Op))
                {
                    var innerTuple = arg1Term.Args as Tuple<object, object>;
                    if (innerTuple == null) throw new Exception("cannot be null");

                    var internalTerm = new Term(arg1Term.Op, new Tuple<object, object>(innerTuple.Item2, tuple.Item2));
                    gTerm = new Term(term.Op, new Tuple<object, object>(innerTuple.Item1, internalTerm));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }*/


        /*/// <summary>
        /// x(y+z) == xy+xz
        /// </summary>
        /// <param name="term"></param>
        /// <param name="gTerm"></param>
        /// <returns></returns>
        public static bool DistributeLaw(this Term term, out Term gTerm)
        {
            gTerm = null;
            var tuple = term.Args as Tuple<object, object>;
            if (tuple == null) throw new Exception("cannot be null");
            var lterm = tuple.Item1 as Term;
            var rTerm = tuple.Item2 as Term;

            if (term.Op.Method.Name.Equals("Multiply"))
            {
                if (lterm != null && rTerm != null) throw new Exception("Not support currently");

                if (lterm == null && rTerm == null) return false;

                if (rTerm != null)
                {
                    var innerTuple = rTerm.Args as Tuple<object, object>;
                    if (innerTuple == null) throw new Exception("cannot be null");
                    var gArg1 = new Term(term.Op, new Tuple<object, object>(tuple.Item1, innerTuple.Item1));

                    //TODO Recursive
                    //gArg1.DistributeLaw(out gArg1Term);                    
                    var gArg2 = new Term(term.Op, new Tuple<object, object>(tuple.Item1, innerTuple.Item2));
                    gTerm = new Term(rTerm.Op, new Tuple<object, object>(gArg1, gArg2));
                    return true;
                }
                else // lTerm != null
                {
                    var innerTuple = lterm.Args as Tuple<object, object>;
                    if (innerTuple == null) throw new Exception("cannot be null");
                    var gArg1 = new Term(term.Op, new Tuple<object, object>(tuple.Item1, innerTuple.Item1));

                    //TODO Recursive
                    //gArg1.DistributeLaw(out gArg1Term);                    
                    var gArg2 = new Term(term.Op, new Tuple<object, object>(tuple.Item1, innerTuple.Item2));
                    gTerm = new Term(lterm.Op, new Tuple<object, object>(gArg1, gArg2));
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// true positive: x+0 = x, x-0 = x, y*1= y, y/1=y
        /// false negative: 0+x-> apply commutative law first 
        /// </summary>
        /// <param name="term"></param>
        /// <param name="gTerm"></param>
        /// <returns></returns>
        public static bool IdentityLaw(this Term term, out object gTerm)
        {
            gTerm = null;
            var tuple = term.Args as Tuple<object, object>;
            if (tuple == null) throw new Exception("cannot be null");
            if (term.Op.Method.Name.Equals("Add") ||
                term.Op.Method.Name.Equals("Substract"))
            {
                if (tuple.Item2.Equals(0))
                {
                    gTerm = tuple.Item1;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (term.Op.Method.Name.Equals("Multiply") ||
                term.Op.Method.Name.Equals("Divide"))
            {
                if (tuple.Item2.Equals(1))
                {
                    gTerm = tuple.Item1;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }*/


    }
}