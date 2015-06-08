using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public class AlgebraRule
    {
        public static string CommutativeLaw(Term oldTerm)
        {
            var tuple = oldTerm.Args as Tuple<object, object>;
            if(tuple == null) throw new Exception("Cannot be null");
            return string.Format("Apply commutative law between {0} and {1}"
                , tuple.Item1, tuple.Item2);
        }

        public static string AssociativeLaw()
        {
            return null;
        }

        public static string DistributiveLaw()
        {
/*            return string.Format("Apply distributive rule between {0} and {1}",
                obj1.ToString(), obj2.ToString());
 */
            return null;
        }

        public static string IdentityLaw(Term term)
        {
            var tuple = term.Args as Tuple<object, object>;
            if (tuple == null) throw new Exception("Cannot be null");
            return string.Format("Apply identity law: on term {0}", term.ToString());
        }
    }
}
