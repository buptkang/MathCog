using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CSharpLogic
{
    public class TraceStep
    {
        public object Source { get; set; }

        public object Target { get; set; }

        public object Rule { get; set; }

        public TraceStep(object source, object target, object rule)
        {
            Source = source;
            Target = target;
            Rule = rule;
        }
    }
}
