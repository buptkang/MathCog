using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return "~" + this._token.ToString();
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
                dynamic a = obj;
                IsVar(a);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
