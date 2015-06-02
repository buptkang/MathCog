using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public class RewriteRule
    {
        public static string MoveTerm(object moveObj, object source, object target)
        {
            return string.Format("Move {0} from {1} to {2}", moveObj.ToString(),
                source.ToString(), target.ToString());
        }

        public static string MergeSameVariable(object method, object variable)
        {
            return string.Format("{0} the same variable: {1}",
               method.ToString(), variable.ToString());
        }

        public static string ApplyCommutativeOnTerm(object obj1, object obj2)
        {
            return string.Format("Apply commutative rule between {0} and {1}",
              obj1.ToString(), obj2.ToString());
        }

        public static string ApplyDistributiveLaw(object obj1, object obj2)
        {
            return string.Format("Apply distributive rule between {0} and {1}",
              obj1.ToString(), obj2.ToString());            
        }
    }
}
