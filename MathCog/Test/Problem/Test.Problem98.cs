/*******************************************************************************
 * Math Interactive Tutoring System
 * <p>
 * Copyright (C) 2015 Bo Kang, Joseph J. LaViola Jr.
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
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public partial class TestProblems
    {
        /*
         * Simplify the expression 1+2-3?
         */
        [Test]
        public void Test_Problem98()
        {
            const string fact1 = "1+2-3=";
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
            Assert.True(steps.Count == 2);
        }
    }
}
