using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ExprSemantic;

namespace AG.Interpreter
{
    public abstract class QueryTrace
    {
        public Trace ReasonTrace { get; set; }

        public QueryTrace(Trace trace)
        {
            ReasonTrace = trace;
        }
    }

    /// <summary>
    /// such as X=?, PS=?
    /// </summary>
    public class PropertyQueryTrace : QueryTrace
    {
        public object Property { get; set; }

        public object Answer { get; set; }

        public PropertyQueryTrace(Object prop,  Trace trace) :base(trace)
        {
            Property = prop;
            ExtractAnswer();
        }

        private void ExtractAnswer()
        {
            
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public class KnowledgeQueryTrace : QueryTrace
    {
        public KnowledgeQueryTrace(Trace trace) 
            : base(trace)
        {
            
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public class RelationQueryTrace : QueryTrace
    {
        public RelationQueryTrace(Trace trace)
            : base(trace)
        {
            
        }
    }
}