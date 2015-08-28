/*******************************************************************************
 * Copyright (c) 2015 Bo Kang
 *   
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *  
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *******************************************************************************/

namespace MathCog
{
    using NUnit.Framework;

    [TestFixture]
    public class TestRealTime
    {
        [Test]
        public void Test0()
        {
            const string fact1 = "2x+y-1=0";
            Reasoner.Instance.Load(fact1);

            bool userInput;
            bool result = Reasoner.Instance.Unload(fact1, out userInput);
            Assert.True(result);
            Assert.False(userInput);

            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 0);

            //const string user1 = "y+2x-1=0";
            const string user1 = "y+3x-1=0";
            var obj = Reasoner.Instance.Load(user1, null, true);
            Assert.NotNull(obj);

            Assert.True(Reasoner.Instance.RelationGraph.Nodes.Count == 0);
            Assert.True(Reasoner.Instance.RelationGraph.UserNodes.Count == 1);

            result = Reasoner.Instance.Unload(user1, out userInput);
            Assert.True(result);
            Assert.True(userInput);
        }
    }
}
