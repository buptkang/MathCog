using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLogic
{
    public partial class LogicSharp
    {
        #region Goals

        public static Func<object,object,Func<Dictionary<object,object>, bool>> 
            Goal_Equal()
        {
            Func<object, object, Dictionary<object, object>, bool> functor = Unifier.Unify;
            return Curry(functor);
        }

        public static bool Memberof(object x, List<object> coll)
        {
            return true;
        }

        #endregion

        #region Logical combination of goals

        public static object logic_Any(List<Func<object, object, object>> goals)
        {
            return null;
        }

        #endregion


        #region EarlyGoal Exception 

        public class EarlyGoalException : Exception
        {
            public EarlyGoalException(string msg) : base()
            {
                
            }            
        }

        #endregion
    }
}
