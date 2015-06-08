using System;
using System.Diagnostics;
using System.Linq.Expressions;
using AlgebraGeometry;
using CSharpLogic;
using starPadSDK.MathExpr;

namespace ExprGenerator
{
    public static class ExprG
    {
        public static Expr Generate(object obj)
        {
            var ss = obj as ShapeSymbol;
            if (ss != null) return Generate(ss);

            var eqGoal = obj as EqGoal;
            if (eqGoal != null) return Generate(eqGoal);

            var term = obj as Term;
            if (term != null) return Generate(term);

            var variable = obj as Var;
            if (variable != null)
            {
                return new WordSym(variable.Token.ToString());
            }

            int integer;
            if (LogicSharp.IsInt(obj, out integer))
            {
                return new IntegerNumber(integer.ToString());
            }

            double dNumber;
            if (LogicSharp.IsDouble(obj, out dNumber))
            {
                return new DoubleNumber(dNumber);
            }

            return null;
        }

        public static Expr Generate(ShapeSymbol ss)
        {
            if (ss is PointSymbol)
            {
                var ps = ss as PointSymbol;
                return ps.ToExpr();
            }
            else
            {
                return null;
            }
        }

        public static Expr Generate(EqGoal goal)
        {
            var head = WellKnownSym.equals;
            var lhs = Generate(goal.Lhs);
            var rhs = Generate(goal.Rhs);
            Debug.Assert(lhs!=null && rhs != null);
            return new CompositeExpr(head, new Expr[] { lhs, rhs });
        }

        public static Expr Generate(Term term)
        {
            if (term.Op.Method.Name.Equals("Add"))
            {
                var head = WellKnownSym.plus;
                var tuple = term.Args as Tuple<object, object>;
                Debug.Assert(tuple!=null);
                var arg1 = Generate(tuple.Item1);
                var arg2 = Generate(tuple.Item2);
                return new CompositeExpr(head, new Expr[] {arg1, arg2});
            }
            else if (term.Op.Method.Name.Equals("Subtract"))
            {
                var head = WellKnownSym.minus;
                var tuple = term.Args as Tuple<object, object>;
                Debug.Assert(tuple != null);
                var arg1 = Generate(tuple.Item1);
                var arg2 = Generate(tuple.Item2);
                return new CompositeExpr(head, new Expr[] { arg1, arg2 });
            }
            else if (term.Op.Method.Name.Equals("Multiply"))
            {
                var head = WellKnownSym.times;
                var tuple = term.Args as Tuple<object, object>;
                Debug.Assert(tuple != null);
                var arg1 = Generate(tuple.Item1);
                var arg2 = Generate(tuple.Item2);
                return new CompositeExpr(head, new Expr[] { arg1, arg2 });
            }
            //TODO
            return null;
        }
    }

    public static class PointExpExtension
    {
        public static Expr ToExpr(this PointSymbol ps)
        {
            var xExpr = ToCoord(ps.SymXCoordinate);
            var yExpr = ToCoord(ps.SymYCoordinate);
            if (ps.Shape.Label == null)
            {
                var comp = new CompositeExpr(new WordSym("comma"), new Expr[] { xExpr, yExpr});
                var cc = new CompositeExpr(new WordSym(""), new Expr[] { comp });
                return cc;                
            }
            else
            {
                var comp = new CompositeExpr(new WordSym(ps.Shape.Label), new Expr[] {xExpr, yExpr});
                return comp;
            }
            //return Text.Convert(form);
        }

        public static Expr ToCoord(string coord)
        {
            int number;
            bool result = LogicSharp.IsInt(coord, out number);
            if(result) return new IntegerNumber(coord);

            double dNumber;
            result = LogicSharp.IsDouble(coord, out dNumber);
            if(result) return new DoubleNumber(dNumber);

            return new WordSym(coord);            
        }
    }
}
