using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Math.Grammar
{
    class program
    {
/*        static void Main(string[] args)
        {
            //Func<double, double> func = CompileFunction("sin(2 * x) + 1");
            Func<double, double> func = CompileFunction("x + 1 + 2*x");

            double y = func.Invoke(2.0);

            //func.Target

            Console.WriteLine(y);
            Console.Read();

            //string text = "Math.Sqrt(Math.Pow(x0-x1,2.0) + Math.Pow(y0-y1,2.0))";
        }

        public static Func<double, double> CompileFunction(string functionText)
        {
            ParseTree ast = ParserInstance.Parse(functionText);
            var builder = new ExpressionTreeBuilder();

            Expression<Func<double, double>> expression = builder.CreateFunction(ast.Root);

            //Expression<Func<double, double, double, double, double>> expression = builder.CreateFunction(ast.Root);

            Func<double, double> function = expression.Compile();

            return function;
        }

        static readonly Parser ParserInstance = new Parser(new ExpressionGrammar());*/
    }
}
