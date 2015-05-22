using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiLabs.Undo;
using NUnit.Framework;

namespace CSharpLogic.Undo.Test
{
    [TestFixture]
    public class TestAction
    {
        [Test]
        public void Test1()
        {
           var d = new DyLogicCachingObject();

           var variable = new Var('x');
           var goal = new EqGoal(variable, 4);             
           d.Reify(goal);
           Assert.True(d.Properties.Count == 1);
           d.UnReify(goal);
           Assert.True(d.Properties.Count == 0);
        }

        public void Test2()
        {
            var d = new DyLogicCachingObject();

            var variable = new Var('x');
            var goal = new EqGoal(variable, 4);
            d.Reify(goal);
            Assert.True(d.Properties.Count == 1);
            //d.Undo();
            //Assert.True(d.Properties.Count == 0);
        }

        [Test]
        public void Test3()
        {
            var d = new DyLogicCachingObject();

            var variable = new Var('x');
            var goal = new EqGoal(variable, 4);
            d.Reify(goal);
            Assert.True(d.Properties.Count == 1);
            var goal2 = new EqGoal(variable, 5);
            d.Reify(goal2);
            Assert.True(d.Properties.Count == 1);
            Assert.True(d.Properties.ContainsKey(variable));
            Assert.True(5.Equals(d.Properties[variable]));
        }

        [Test]
        public void Test4()
        {
            var d = new DyLogicCachingObject();

            var variable = new Var('x');
            var goal = new EqGoal(variable, 4);

            
            var goal2 = new EqGoal(variable, 5);

            var lst = new List<EqGoal>();
            lst.Add(goal);
            lst.Add(goal2);

            d.Reify(lst);
            Assert.True(d.Properties.Count == 1);
            Assert.True(d.Properties.ContainsKey(variable));
            Assert.True(5.Equals(d.Properties[variable]));
        }

    }
}
