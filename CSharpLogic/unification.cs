using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLogic
{
    public partial class LogicSharp
    {
        #region Reification

        private static object ReifyImpl(Object obj, Dictionary<object, object> dict)
        {
            return obj;
        }

        private static object ReifyImpl(Dictionary<object,object> dic, Dictionary<object, object> dict)
        {
            return dic.ToDictionary(pair => pair.Key, pair => Reify(pair.Value, dict));
        }

        private static object ReifyImpl(List<object> list, Dictionary<object, object> dict)
        {
            return ReifyImpl((IEnumerable<object>) list, dict);
        }

        private static object ReifyImpl(Tuple<object, object> tuple, Dictionary<object, object> dict)
        {
            return new Tuple<object, object>(Reify(tuple.Item1, dict), Reify(tuple.Item2, dict));
        }

        private static object ReifyImpl(IEnumerable<object> iter, Dictionary<object, object> dict)
        {
            return iter.Select(obj => Reify(obj, dict)).ToList();
        }

        public static object Reify(object e, Dictionary<object, object> s)
        {
            if (Var.IsVar(e))
            {
                var tempVar = (Var)e;
                return s.ContainsKey(tempVar) ? Reify(s[tempVar], s) : e;
            }

            var term = e as Term;
            if (term != null)
            {
                var gArgs = Reify(term.Args, s);
                return new Term(term.Op, gArgs);                
            }
            
            dynamic a = e;
            return ReifyImpl(a, s);
        }

        #endregion

        #region Unification

        private static bool UnifyImpl(Tuple<object, object> u, Tuple<object, object> v, Dictionary<object, object> s)
        {
            if (!Unify(u.Item1, v.Item1, s))
            {
                return false;
            }
            if (!Unify(u.Item2, v.Item2, s))
            {
                return false;
            }
            return true;
        }

        private static bool UnifyImpl(IEnumerable<object> u, IEnumerable<object> v, Dictionary<object, object> s)
        {
            var enumerable = u as IList<object> ?? u.ToList();
            var objects = v as IList<object> ?? v.ToList();

            if (enumerable.Count() != objects.Count()) return false;
            var pair = enumerable.Zip(objects, (first, second) 
                            => new Tuple<object, object>(first, second));
            return pair.All(item => Unify(item.Item1, item.Item2, s));
        }

        private static bool UnifyImpl(object u, object v, Dictionary<object, object> s)
        {
            return false;
        }

        private static bool UnifyImpl(Dictionary<object, object> u, 
            Dictionary<object, object> v,
            Dictionary<object, object> s)
        {
            if (v.Count != u.Count) return false;
            foreach (KeyValuePair<object, object> pair in u)
            {
                if (!v.ContainsKey(pair.Key))
                {
                    return false;
                }

                if (!Unify(pair.Value, v[pair.Key], s))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Unify(object u, object v, Dictionary<object, object> s)
        {
            if (s == null)
            {
                s = new Dictionary<object, object>();
            }

            object tempU = LogicSharp.transitive_get(u, s);
            object tempV = LogicSharp.transitive_get(v, s);

            if (LogicSharp.equal_test(tempU, tempV))
            {
                return true;
            }

            if (Var.IsVar(tempU))
            {
                LogicSharp.Assoc(s, tempU, tempV);
                return true;
            }

            if (Var.IsVar(tempV))
            {
                LogicSharp.Assoc(s, tempV, tempU);
                return true;
            }

            dynamic a = tempU;
            dynamic b = tempV;

            return UnifyImpl(a, b, s);
        }

        #endregion
    }
}
