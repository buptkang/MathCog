using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace CSharpLogic
{
    public class Var
    {
        private static dynamic id = 1;

        private object _token;

        public object Token { get { return _token; } set { _token = value; } }

        public Var()
        {
            _token = id;
            id += 1;
        }

        public Var(object id)
        {
            _token = id;
        }

        public override string ToString()
        {
            return this._token.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is Var)
            {
                var mVar = obj as Var;
                return mVar._token.Equals(this._token);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Token.GetHashCode();
        }

        public static bool IsVar(Var obj)
        {
            return true;
        }

        public static bool IsVar(object obj)
        {
            if (obj is Var)
            {
                //dynamic a = obj;
                //IsVar(a);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsNotVar(object obj)
        {
            return !IsVar(obj);
        }

        public static bool ContainsVar(object obj)
        {
            if (obj is Var)
            {
                return true;
            }
            else if (obj is Tuple<object>)
            {
                var tuple = obj as Tuple<object>;
                return ContainsVar(tuple.Item1);
            }
            else if (obj is Tuple<object, object>)
            {
                var tuple = obj as Tuple<object, object>;
                return ContainsVar(tuple.Item1) || ContainsVar(tuple.Item2);
            }
            else if (obj is IEnumerable<object>)
            {
                var enumerator = obj as IEnumerable<object>;
                foreach (var tempObj in enumerator)
                {
                    if (ContainsVar(tempObj))
                        return true;
                }
                return false;
            }
            else if (obj is IDictionary<object, object>)
            {
                var dict = obj as IDictionary<object, object>;
                foreach (KeyValuePair<object, object> pair in dict)
                {
                    if (ContainsVar(pair.Key) || ContainsVar(pair.Value))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
