using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public partial class Query : Equation
    {
        //Cached symbols for non-concrete objects
        public HashSet<object> CachedEntities { get; set; }
        private HashSet<KeyValuePair<object, object>> CachedObjects;
    }
}
