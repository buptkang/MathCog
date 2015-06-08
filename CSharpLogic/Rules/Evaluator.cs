using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic.Rules
{
    /// <summary>
    /// The laws of Algebra
    /// </summary>
    public static class TermEvaluator
    {
        /// <summary>
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
                    if(innerTuple == null) throw new Exception("cannot be null");

                    var internalTerm = new Term(arg1Term.Op, new Tuple<object, object>(innerTuple.Item2, tuple.Item2));
                    gTerm = new Term(term.Op, new Tuple<object,object>(innerTuple.Item1, internalTerm));
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

        }

        /// <summary>
        /// x + y == y + x, xy == yx
        /// </summary>
        /// <param name="term"></param>
        /// <param name="gTerm"></param>
        /// <returns></returns>
        public static bool CommutativeLaw(this Term term, out Term gTerm)
        {
            gTerm = null;
            var tuple = term.Args as Tuple<object, object>;
            if (tuple == null) throw new Exception("cannot be null");
            gTerm = new Term(term.Op, new Tuple<object,object>(tuple.Item2, tuple.Item1));
            return true;
        }

        /// <summary>
        /// x(y+z) == xy+xz
        /// </summary>
        /// <param name="term"></param>
        /// <param name="gTerm"></param>
        /// <returns></returns>
        public static bool DistributeLaw(this Term term, out Term gTerm)
        {
            gTerm = null;
            var tuple = term.Args as Tuple<object, object>;
            if(tuple == null) throw new Exception("cannot be null");
            var lterm = tuple.Item1 as Term;
            var rTerm = tuple.Item2 as Term;

            if (term.Op.Method.Name.Equals("Multiply"))
            {
                if(lterm != null && rTerm != null) throw new Exception("Not support currently");

                if (lterm == null && rTerm == null) return false;

                if (rTerm != null)
                {
                    var innerTuple = rTerm.Args as Tuple<object, object>;
                    if(innerTuple == null) throw new Exception("cannot be null");
                    var gArg1 = new Term(term.Op, new Tuple<object, object>(tuple.Item1, innerTuple.Item1));

                    //TODO Recursive
                    //gArg1.DistributeLaw(out gArg1Term);                    
                    var gArg2 = new Term(term.Op, new Tuple<object, object>(tuple.Item1, innerTuple.Item2));
                    gTerm = new Term(rTerm.Op, new Tuple<object,object>(gArg1, gArg2));
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
        }
    }

    /// <summary>
    /// The laws of Equality
    /// </summary>
    public static class GoalEvaluator
    {
        /// <summary>
        /// if x = y, then y = x
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="gGoal"></param>
        /// <returns></returns>
        public static bool SymmetricProperty(this Goal goal, out Goal gGoal)
        {
            gGoal = null;
            return false;
        }

        /// <summary>
        /// if x = y and y = z, then x = z
        /// if x = y, then x + a = y + a
        /// if x = y, then ax = ay
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="gGoal"></param>
        /// <returns></returns>
        public static bool TransitiveProperty(this Goal goal, out Goal gGoal)
        {
            gGoal = null;
            return false;
        }
    }
}