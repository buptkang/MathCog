using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Math.Grammar
{
    [Language("Expression", "1.0", "Dynamic geometry expression evaluator")]
    public class ExpressionGrammar : Irony.Parsing.Grammar
    {
        public ExpressionGrammar()
        {
            this.GrammarComments = @"Arithmetical expressions for dynamic geometry.";

            // 1. Terminals
            var number = new NumberLiteral("number");
            var identifier = new IdentifierTerminal("identifier");

            // 2. Non-terminals
            var Expr = new NonTerminal("Expr");
            var Term = new NonTerminal("Term");
            var BinExpr = new NonTerminal("BinExpr");
            var ParExpr = new NonTerminal("ParExpr");
            var UnExpr = new NonTerminal("UnExpr");
            var UnOp = new NonTerminal("UnOp");
            var BinOp = new NonTerminal("BinOp", "operator");
            var PostFixExpr = new NonTerminal("PostFixExpr");
            var PostFixOp = new NonTerminal("PostFixOp");
            var AssignmentStmt = new NonTerminal("AssignmentStmt");
            var AssignmentOp = new NonTerminal("AssignmentOp");
            var PropertyAccess = new NonTerminal("PropertyAccess");
            var FunctionCall = new NonTerminal("FunctionCall");

            // 3. BNF rules
            Expr.Rule = Term | UnExpr | FunctionCall | PropertyAccess | BinExpr;
            Term.Rule = number | ParExpr | identifier;
            ParExpr.Rule = "(" + Expr + ")";
            UnExpr.Rule = UnOp + Term;
            UnOp.Rule = ToTerm("+") | "-" | "++" | "--";
            BinExpr.Rule = Expr + BinOp + Expr;
            BinOp.Rule = ToTerm("+") | "-" | "*" | "/" | "^";
            PropertyAccess.Rule = identifier + "." + identifier;
            FunctionCall.Rule = identifier + ParExpr;
            this.Root = Expr;

            // 4. Operators precedence
            RegisterOperators(1, "+", "-");
            RegisterOperators(2, "*", "/");
            RegisterOperators(3, Associativity.Right, "^");

            MarkPunctuation("(", ")", ".");
            MarkTransient(Term, Expr, BinOp, UnOp, AssignmentOp, ParExpr);
        }
    }
}
