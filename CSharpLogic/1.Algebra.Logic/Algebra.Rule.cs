using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public static class AlgebraRule
    {
        public enum AlgebraRuleType
        {
            Distributive,
            Associative,
            Commutative,
            Identity,
            Inverse
        }

        public static string Rule(AlgebraRuleType ruleType, 
                                object obj1, object obj2)
        {
            switch (ruleType)
            {
               case AlgebraRuleType.Distributive:
                    return string.Format("Apply distributive law between {0} and {1}", obj1, obj2);
               case AlgebraRuleType.Associative:
                    return string.Format("Apply associative law between {0} and {1}", obj1, obj2);
               case AlgebraRuleType.Commutative:
                    return string.Format("Apply commutative law between {0} and {1}", obj1, obj2);
               case AlgebraRuleType.Identity:
                    return string.Format("Apply identity law between {0} and {1}", obj1, obj2);
               case AlgebraRuleType.Inverse:
                    return string.Format("Apply inverse law between {0} and {1}", obj1, obj2);
               default:
                    break;
            }
            return null;
        }

    }
}
