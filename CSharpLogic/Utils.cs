using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpLogic
{
    public partial class LogicSharp
    {


        public static object transitive_get(object key, Dictionary<object, object> d)
        {
            if (d == null || d.Count == 0) return key;
            while (d.ContainsKey(key))
            {
                key = d[key];
            }
            return (object)key;
        }

        public static object deep_transitive_get(object key, Dictionary<object, object> d)
        {
            object key1 = transitive_get(key, d);
            if (key1 is Tuple<object>)
            {
                var mKey = key1 as Tuple<object>;
                return Tuple.Create(deep_transitive_get(mKey.Item1, d) as object);
            }
            else if (key1 is Tuple<object, object>)
            {
                var mKey = key1 as Tuple<object, object>;
                return Tuple.Create(deep_transitive_get(mKey.Item1, d),
                             deep_transitive_get(mKey.Item2, d)
                            );
            }
            else if (key1 is Tuple<object, object, object>)
            {
                var mKey = key1 as Tuple<object, object, object>;
                return Tuple.Create(deep_transitive_get(mKey.Item1, d),
                             deep_transitive_get(mKey.Item2, d),
                             deep_transitive_get(mKey.Item3, d)
                            );
            }
            return key1;
        }

        public static bool equal_test(object obj1, object obj2)
        {
            if (obj1 is List<object> && obj2 is List<object>)
            {
                var lst1 = obj1 as List<object>;
                var lst2 = obj2 as List<object>;
                return lst1.SequenceEqual(lst2);
            }
            else if (obj1 is Tuple<object> && obj2 is Tuple<object>)
            {
                var tuple1 = obj1 as Tuple<object>;
                var tuple2 = obj2 as Tuple<object>;
                return tuple1.Equals(tuple2);
            }
            else if (obj1 is Dictionary<object, object>
                && obj2 is Dictionary<object, object>)
            {
                var dict1 = obj1 as Dictionary<object, object>;
                var dict2 = obj2 as Dictionary<object, object>;

                if (dict1.Count != dict2.Count) return false;
                if (dict1.Keys.Except(dict2.Keys).Any()) return false;
                if (dict2.Keys.Except(dict1.Keys).Any()) return false;
                return dict1.All(pair => equal_test(pair.Value, dict2[pair.Key]));
            }
            else
            {
                return obj1.Equals(obj2);
            }
        }

        /*
         *      >>> assoc({'x': 1}, 'x', 2)
                {'x': 2}
                >>> assoc({'x': 1}, 'y', 3)   
                {'x': 1, 'y': 3}
        */
        public static void Assoc(Dictionary<object, object> dict, object key, object value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }


        public static Func<T1, T2, Func<T3, TResult>> Curry<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function)
        {
            return (a, b) => c => function(a, b, c);
        }
    }
}
