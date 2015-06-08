using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace AG.Interpreter.Test
{
    [TestFixture]
    public class QueryTraceAction
    {
        [Test]
        public void UndoRedo()
        {
            //
        }

        public void test_reify_object_static()
        {
            //static approach
/*
            var foo = new DyLogicObject();
            foo.Properties.Add(new Var("y"), 1);
            var variable = new Var('x');
            var dict = new Dictionary<object, object>();
            dict.Add(variable, 4);
            var result = foo.Reify(dict) as DyLogicObject;
            Assert.NotNull(result);
            Assert.True(result.Properties.Count == 1);
            result.Properties.Add(variable, null);
            var result2 = result.Reify(dict) as DyLogicObject;
            Assert.NotNull(result2);
            Assert.True(result2.Properties.Count == 2);
            Assert.True(result2.Properties[variable].Equals(4));
 */
            /*
                        //negative case
                        foo = new DyLogicObject();
                        foo.Properties.Add(new Var("y"), 1);
                        foo.Properties.Add(new Var("x"), 2);
                        variable = new Var('x');
                        dict = new Dictionary<object, object>();
                        dict.Add(variable, 4);
                        result = foo.Reify(dict);
                        Assert.True(result.Equals(foo));
             */
        }

/*
        [Test]
        public void test_reify_object_action()
        {
            var foo = new DyLogicObject();
            foo.Properties.Add(new Var("y"), 1);
            foo.Properties.Add(new Var('x'), null);
            var variable = new Var('x');
            var dict = new Dictionary<object, object>();
            dict.Add(variable, 4);

            var action = new ReifyLogicObjectAction(foo, dict);
            var am = new ActionManager();
            am.RecordAction(action);
            Assert.NotNull(action.CurrObj);
            Assert.True(action.CurrObj.Properties[variable].Equals(4));
            am.Undo();
            Assert.Null(action.CurrObj.Properties[variable]);
            am.Redo();
            Assert.True(action.CurrObj.Properties[variable].Equals(4));
        }
 */
        /*
                [Test]
                public void test_reify_objects_action()
                {
                    var foo = new DyLogicObject();
                    foo.Properties.Add(new Var("y"), 1);
                    foo.Properties.Add(new Var('x'), null);

                    var foo1 = new DyLogicObject();
                    foo1.Properties.Add(new Var("y"), 2);
                    foo1.Properties.Add(new Var('x'), null);
                    var variable = new Var('x');
                    var dict = new Dictionary<object, object>();
                    dict.Add(variable, 4);

                }
         */
    }
}
