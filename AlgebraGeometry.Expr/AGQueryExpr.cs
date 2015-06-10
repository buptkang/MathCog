using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgebraGeometry.Expr
{
    /// <summary>
    /// Data structure to convert QueryResult to
    /// </summary>
    public class AGQueryExpr : IKnowledge
    {
        //TODO Strategy

        public string Instruction { get; set; } // Instruction Design
        public bool QuerySuccess { get; set; } // true or false of querying

        public AGQueryExpr(starPadSDK.MathExpr.Expr expr) : base(expr)
        {
           
        }
    }
}
