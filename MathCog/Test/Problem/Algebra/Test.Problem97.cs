 /* Copyright (C) 2015 Bo Kang, Joseph J. LaViola Jr.
 * <p>
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 2 of the License, or any later version.
 * <p>
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
 * details.
 * <p>
 * You should have received a copy of the GNU General Public License along with
 * this program; if not, write to the Free Software Foundation, Inc., 51
 * Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 ******************************************************************************/

namespace MathCog
{
    using System;
    using CSharpLogic;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         * Simplify the expression x+1+x-4?
         * 
         */
        [Test]
        public void Test_Problem_97()
        {
            const string fact1 = "x+1+x-4=";
            var queryExpr = Reasoner.Instance.Load(fact1) as AGQueryExpr;
            Assert.NotNull(queryExpr);
            queryExpr.RetrieveRenderKnowledge();
            Assert.True(queryExpr.RenderKnowledge.Count == 1);

            var agEquationExpr = queryExpr.RenderKnowledge[0] as AGEquationExpr;
            Assert.True(agEquationExpr != null);
            agEquationExpr.IsSelected = true;

            agEquationExpr.GenerateSolvingTrace();
            Assert.True(agEquationExpr.AutoTrace != null);
            Assert.True(agEquationExpr.AutoTrace.Count == 1);

            var steps = agEquationExpr.AutoTrace[0].Item2 as List<TraceStepExpr>;
            Assert.NotNull(steps);
            Assert.True(steps.Count == 4);

            Reasoner.Instance.Reset();
        }

        [Test]
        public void Test_Problem_97_1()
        {
            const string fact1 = "x+x-3";
            object objOutput;
            var trace = Reasoner.Instance.RelationValidate(fact1, out objOutput) as List<Tuple<object, object>>;

            var query = objOutput as Query;
            Assert.NotNull(query);
            Assert.NotNull(trace);
            Assert.True(trace.Count == 1);
            var strategy = trace[0].Item1 as string;
            var tsLst = trace[0].Item2 as List<TraceStepExpr>;
            Assert.NotNull(strategy);
            Assert.NotNull(tsLst);
            var eqExpr = tsLst[1].TraceStep.Target as Term;
            Assert.NotNull(eqExpr);
           /* var term = eq.Rhs as Term;
            Assert.NotNull(term);
            Assert.True(term.ToString().Equals("((2*x)-3)"));*/

            Reasoner.Instance.Reset();
        }
    }
}