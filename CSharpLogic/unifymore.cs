using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLogic
{
    public partial class Unifier
    {
        public static object Reify_Object(DyLogicObject logicObj, Dictionary<Var, object> s)
        {
            var obj = Reify(logicObj.Properties, s) as Dictionary<object, object>;
            if (LogicSharp.equal_test(obj, logicObj.Properties))
            {
                return logicObj;
            }
            else
            {
                //Initialize a new dynamic object
                var newObj = new DyLogicObject();
                if (obj != null)
                {
                    foreach (var pair in obj)
                    {
                        newObj.Properties.Add(pair.Key, pair.Value);
                    }                    
                }
                return newObj;
            }
        }

        public static object Unify_Object(DyLogicObject dy1, DyLogicObject dy2, Dictionary<object, object> s)
        {
            return Unify(dy1.Properties, dy2.Properties, s);
        }
    }

    public class DyLogicObject : DynamicObject
    {
        public readonly Dictionary<object, object> Properties 
            = new Dictionary<object, object>();

        public int Count
        {
            get { return Properties.Count; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            return Properties.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase 
            // so that property names become case-insensitive.
            Properties[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary, 
            // so this method always returns true. 
            return true;
        }
    }
}
