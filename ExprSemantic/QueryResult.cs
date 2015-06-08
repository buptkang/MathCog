using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CSharpLogic;
using starPadSDK.MathExpr;
using AlgebraGeometry.Expr;

namespace ExprSemantic
{
    public abstract class QueryResult
    {
        //TODO Strategy
        public List<TraceStepExpr> Trace { get; set; }
        public string Instruction { get; set; } // Instruction Design
        public bool QuerySuccess { get; set; } // true or false of querying

        protected QueryResult()
        { }
    }

    /// <summary>
    /// such as X=?, PS=?
    /// </summary>
    public class PropertyQueryResult : QueryResult
    {
        public Var Property { get; set; }
        public Expr Answer { get; set; }

        public PropertyQueryResult(Var property)
        {
            Property = property;
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public class KnowledgeQueryTrace 
    {
      
    }

    /// <summary>
    /// TODO
    /// </summary>
    public class RelationQueryTrace 
    {
        
    }
}