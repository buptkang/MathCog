using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CSharpLogic;
using System.Linq.Expressions;
using NUnit.Framework.Constraints;

namespace AlgebraExpression.Test
{
    [TestFixture]
    public class TestTermEvaluator
    {
        #region Line Term Rectification

        [Test]
        public void Test_Rectify_1()
        {
            //ax
            var variable = new Var("ax");
            object result = LineTermEvaluator.RectifyLineTerm(variable);
            Assert.NotNull(result);
            Assert.IsInstanceOf(typeof(Term), result);
            var term = result as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Multiply"));
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            var innerVar = tuple.Item1 as Var;
            Assert.NotNull(innerVar);
            Assert.True(innerVar.ToString().Equals("a"));
            var innerVar2 = tuple.Item2 as Var;
            Assert.NotNull(innerVar2);
            Assert.True(innerVar2.ToString().Equals("x"));

            //bY
            variable = new Var("bY");
            result = LineTermEvaluator.RectifyLineTerm(variable);
            Assert.NotNull(result);
            Assert.IsInstanceOf(typeof(Term), result);
            term = result as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Multiply"));
            tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            innerVar = tuple.Item1 as Var;
            Assert.NotNull(innerVar);
            Assert.True(innerVar.ToString().Equals("b"));
            innerVar2 = tuple.Item2 as Var;
            Assert.NotNull(innerVar2);
            Assert.True(innerVar2.ToString().Equals("Y"));

            //ab
            variable = new Var("ab");
            result = LineTermEvaluator.RectifyLineTerm(variable);
            Assert.True(result.Equals(variable));

            //1+bY
            variable = new Var("bY");
            term = new Term(Expression.Add, new Tuple<object, object>(1, variable));
            result = LineTermEvaluator.RectifyLineTerm(term);
            Assert.NotNull(result);
            Assert.IsInstanceOf(typeof(Term), result);
            term = result as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Add"));
            tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            var innerTerm = tuple.Item2 as Term;
            Assert.NotNull(innerTerm);
            tuple = innerTerm.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            innerVar = tuple.Item1 as Var;
            Assert.NotNull(innerVar);
            Assert.True(innerVar.ToString().Equals("b"));
            innerVar2 = tuple.Item2 as Var;
            Assert.NotNull(innerVar2);
            Assert.True(innerVar2.ToString().Equals("Y"));
        }

        [Test]
        public void Test_Rectify_2()
        {
            //x
            var variable = new Var("x");
            object result = LineTermEvaluator.RectifyLineTerm(variable);
            Assert.NotNull(result);
            Assert.IsInstanceOf(typeof(Term), result);
            var term = result as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Multiply"));
            var tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.ToString().Equals("1"));
            var innerVar2 = tuple.Item2 as Var;
            Assert.NotNull(innerVar2);
            Assert.True(innerVar2.ToString().Equals("x"));

            //2x
            variable = new Var("2x");
            result = LineTermEvaluator.RectifyLineTerm(variable);
            Assert.NotNull(result);
            Assert.IsInstanceOf(typeof(Term), result);
            term = result as Term;
            Assert.NotNull(term);
            Assert.True(term.Op.Method.Name.Equals("Multiply"));
            tuple = term.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.ToString().Equals("2"));
            innerVar2 = tuple.Item2 as Var;
            Assert.NotNull(innerVar2);
            Assert.True(innerVar2.ToString().Equals("x"));

            //1+2x
            variable = new Var("2x");
            term = new Term(Expression.Add, new Tuple<object,object>(1,variable));
            result = LineTermEvaluator.RectifyLineTerm(term);
            Assert.NotNull(result);
            Assert.IsInstanceOf(typeof(Term), result);
            var term1 = result as Term;
            Assert.NotNull(term1);
            tuple = term1.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.Equals(1));
            var innerTerm = tuple.Item2 as Term;
            Assert.NotNull(innerTerm);
            tuple = innerTerm.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.Equals(2.0));
            Assert.True(tuple.Item2.Equals(new Var('x')));

            //2*x+1
            var variable1 = new Var("x");
            var term11 = new Term(Expression.Multiply, new Tuple<object, object>(2, variable1));
            term = new Term(Expression.Add, new Tuple<object, object>(term11, 1));
            result = LineTermEvaluator.RectifyLineTerm(term);
            Assert.NotNull(result);
            Assert.IsInstanceOf(typeof(Term), result);
            term1 = result as Term;
            Assert.NotNull(term1);
            tuple = term1.Args as Tuple<object, object>;
            Assert.NotNull(tuple);            
            Assert.True(tuple.Item2.Equals(1));
            innerTerm = tuple.Item1 as Term;
            Assert.NotNull(innerTerm);
            tuple = innerTerm.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            Assert.True(tuple.Item1.Equals(2));
            Assert.True(tuple.Item2.Equals(new Var('x')));
        }

        [Test]
        public void Test_Rectify_3()
        {
            //-ax
            var variable = new Var("ax");
            var term = new Term(Expression.Multiply, new Tuple<object, object>(-1, variable));
            //TODO
        }

        [Test]
        public void Test_REctify_4()
        {
            //2x+b
            var variable  = new Var("2x");
            var variableb = new Var("b");
            var term = new Term(Expression.Add, new Tuple<object, object>(variable, variableb));

            object result = LineTermEvaluator.RectifyLineTerm(term);
            Assert.NotNull(result);
            Assert.IsInstanceOf(typeof(Term), result);
            var term1 = result as Term;

            Assert.NotNull(term1);
            Assert.True(term1.Op.Method.Name.Equals("Add"));
            var tuple = term1.Args as Tuple<object, object>;
            Assert.NotNull(tuple);
            var bb = tuple.Item2 as string;
            Assert.NotNull(bb);
            Assert.True(bb.Equals("b"));

            var innerTerm = tuple.Item1 as Term;
            Assert.NotNull(innerTerm);
            var tuple2 = innerTerm.Args as Tuple<object, object>;
            Assert.NotNull(tuple2);
            Assert.True(tuple2.Item1.Equals(2.0));
            var xVar = tuple2.Item2 as Var;
            Assert.NotNull(xVar); 
            Assert.True(xVar.ToString().Equals("x"));
        }

        [Test]
        public void Test_Rectify_5()
        {
            //a*x
            var a = new Var('a');
            var x = new Var('x');
            var term = new Term(Expression.Multiply, new Tuple<object, object>(a, x));
            
            //object result = LineTermEvaluator.RectifyLineTerm(term);
        }


        #endregion

        #region Term Flattern and Treeify(UnFlattern)

        [Test]
        public void Test_Term_Flattern()
        {
            //1*2
            var term = new Term(Expression.Multiply, new Tuple<object, object>(1, 2));
            Term gTerm = term.Flattern();
            Assert.True(gTerm.Equals(term));

            //1*2+2
            term  = new Term(Expression.Multiply, new Tuple<object, object>(1, 2));
            var term2 = new Term(Expression.Add, new Tuple<object, object>(term,2)); 
            gTerm = term2.Flattern();
            Assert.IsInstanceOf(typeof(List<object>), gTerm.Args);
            var lst = gTerm.Args as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 2);

            //1*2+2+3
            term  = new Term(Expression.Multiply, new Tuple<object, object>(1, 2));
            term2 = new Term(Expression.Add, new Tuple<object, object>(term, 2));
            var term3 = new Term(Expression.Add, new Tuple<object, object>(term2, 3));
            gTerm = term3.Flattern();
            Assert.IsInstanceOf(typeof(List<object>), gTerm.Args);
            lst = gTerm.Args as List<object>;
            Assert.NotNull(lst);
            Assert.True(lst.Count == 3);
            var innerTerm = lst[0] as Term;
            Assert.NotNull(innerTerm);
            Assert.True(lst[1].Equals(2));
            Assert.True(lst[2].Equals(3));

        }

        [Test]
        public void Test_Term_Treeify()
        {
            var term1 = new Term(Expression.Multiply, new Tuple<object,object>(1,2));
            var term2 = new Term(Expression.Multiply, new Tuple<object,object>(1,3));
            var term3 = new Term(Expression.Multiply, new Tuple<object,object>(1,4));

            var term = new Term(Expression.Add, new List<object>() { term1, term2, term3 });
            var result = term.Treeify();
            Assert.NotNull(result);
        }

        #endregion

        #region TODO

        [Test]
        public void Test_basic()
        {
            // 1+x -> x + 1
            var x = new Var('x');
            var term = new Term(Expression.Add, new Tuple<object, object>(1, x));
            object result = term.Eval();
            Assert.True(result.Equals(term));
            Assert.True(term.Traces.Count == 1);

            //x - x -> 0
            term = new Term(Expression.Subtract, new Tuple<object, object>(x, x));
            result = term.Eval();
            Assert.NotNull(result);
            Assert.True(result.Equals(0));
            Assert.True(term.Traces.Count == 1);

            //x + x -> 2*x
            term = new Term(Expression.Add, new Tuple<object, object>(x, x));
            result = term.Eval();
            Assert.NotNull(result);
            Assert.IsInstanceOf(typeof(Term), result);
            var gTerm = result as Term;
            Assert.NotNull(gTerm);
            Assert.True(gTerm.Op.Method.Name.Equals("Multiply"));
            var gTuple = gTerm.Args as Tuple<object, object>;
            Assert.NotNull(gTuple);
            Assert.True(gTuple.Item1.Equals(2));
            Assert.True(gTuple.Item2.Equals(x));
            Assert.True(term.Traces.Count == 1);
            Assert.True(gTerm.Traces.Count == 1);
        }

        [Test]
        public void Test_Eval_Recursive()
        {
            //x+x -> 2*x, y*y -> y^2 

            //(x+1)-3

            //x+x-y

            //x+y-x

            //x + (x-3)

            //2x + x
            //add(mul(2,x),x) 

            //distributive law
            //identity law


        }

        #endregion 
    }
}