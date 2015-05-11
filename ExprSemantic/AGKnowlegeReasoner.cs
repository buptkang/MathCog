// /*******************************************************************************
//  * Analytical Geometry Semantic Parsing 
//  * <p>
//  * Copyright (C) 2014  Bo Kang, Hao Hu
//  * <p>
//  * This program is free software; you can redistribute it and/or modify it under
//  * the terms of the GNU General Public License as published by the Free Software
//  * Foundation; either version 2 of the License, or any later version.
//  * <p>
//  * This program is distributed in the hope that it will be useful, but WITHOUT
//  * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
//  * FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
//  * details.
//  * <p>
//  * You should have received a copy of the GNU General Public License along with
//  * this program; if not, write to the Free Software Foundation, Inc., 51
//  * Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
//  ******************************************************************************/

using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using ExprSemantic.KnowledgeBase;
using ExprSemantic.KnowledgeRelation;

namespace ExprSemantic
{
    using ExprSemantic.KnowledgeQueryEngine;
    using AGSemantic.KnowledgeBase;
    using ExprSemantic.KnowledgeUnification;
    using starPadSDK.MathExpr;
    using System;
    using System.Collections.Generic;

    public class AGKnowlegeReasoner
    {
        private static AGKnowlegeReasoner _instance;

        public static AGKnowlegeReasoner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AGKnowlegeReasoner();
                }
                return _instance;
            }
        }

        private AGKnowlegeReasoner()
        {
            Knowledge = new Dictionary<Expr, IKnowledgeExpr>();
            QueryProperties = new List<AGPropertyExpr>();
            CurrQueryMode = QueryMode.None;
            IsLastTrace = false;
        }







        public IDictionary<Expr, IKnowledgeExpr> Knowledge { get; set; }
        public KeyValuePair<Expr,IKnowledgeExpr> CurrKnowledge { get; set; }
        public List<AGPropertyExpr> QueryProperties { get; set; }
        public AGPropertyExpr CurrQuery { get; set; }

        public bool IsLastTrace { get; set; }

        public QueryMode CurrQueryMode { get; set; }

        public void Clear()
        {
            CurrQueryMode = QueryMode.None;
            Knowledge = new Dictionary<Expr, IKnowledgeExpr>();
            QueryProperties = new List<AGPropertyExpr>();
            IsLastTrace = false;
        }

        #region Query Mode

        public object QueryWhat(Expr rootExpr, Expr currExpr)
        {
            QueryMode mode = UpdateQuery(rootExpr, QueryMode.What);

            if (mode == QueryMode.WhatOnKnowledge)
            {
                return CurrKnowledge.Value;
            }
            else if (mode == QueryMode.WhatOnProperty)
            {
                return CurrQuery.PropertyAnswerExpr;
            }
            else
            {
                return null;
            }
        }

        public string QueryStrategy(Expr rootExpr, Expr currExpr)
        {
            QueryMode mode = UpdateQuery(rootExpr, QueryMode.Strategy);

            if (mode == QueryMode.StrategyOnKnowledge)
            {
                return CurrKnowledge.Value.RetrieveCurrStrategyHint(currExpr);
            }
            else if (mode == QueryMode.StrategyOnProperty)
            {                
                int index = CurrQuery.Tracers.FindIndex(x => x.Source.Equals(currExpr));

                if (index == -1)
                {
                    AGKnowledgeTracer firstTrace = CurrQuery.Tracers[0];
                    return firstTrace.StrategyHints;
                }
                else
                {
                    return CurrQuery.RetrieveCurrStrategyHint(currExpr);
                }
            }
            else
            {
                return "No Strategy";
            }
        }

        public string QueryWhy(Expr rootExpr, Expr currExpr)
        {
            QueryMode mode = UpdateQuery(rootExpr, QueryMode.Why);

            if (mode == QueryMode.WhyOnKnowledge)
            {
                return CurrKnowledge.Value.RetrieveCurrWhyHint(currExpr);
            }
            else if (mode == QueryMode.WhyOnProperty)
            {
                int index = CurrQuery.Tracers.FindIndex(x => x.Source.Equals(currExpr));

                if (index == -1)
                {
                    AGKnowledgeTracer firstTrace = CurrQuery.Tracers[0];                    
                    return firstTrace.WhyHints;
                }
                else
                {
                    return CurrQuery.RetrieveCurrWhyHint(currExpr);
                }
            }
            else
            {
                return "No Why Hint";
            }                                    
        }

        public Expr QueryHow(Expr rootExpr, Expr currExpr, out string appliedRule, out bool isLastTrace)
        {
            isLastTrace = false;
            QueryMode mode = UpdateQuery(rootExpr, QueryMode.How);

            if (mode == QueryMode.HowOnKnowledge)
            {
                return CurrKnowledge.Value.RetrieveHow(currExpr, out appliedRule, out isLastTrace);
            }
            else if (mode == QueryMode.HowOnProperty)
            {
                if (IsLastTrace)
                {
                    appliedRule = "Nothing";
                    return null;
                }

                int index = CurrQuery.Tracers.FindIndex(x => x.Source.Equals(currExpr));

                if (index == -1)
                {
                    AGKnowledgeTracer firstTrace = CurrQuery.Tracers[0];
                    appliedRule = firstTrace.AppliedRule;
                    return firstTrace.Target;
                }
                else
                {
                    Expr returnExpr =  CurrQuery.RetrieveHow(currExpr, out appliedRule, out isLastTrace);
                    IsLastTrace = isLastTrace;
                    return returnExpr;
                }                    
            }
            else
            {
                appliedRule = "Nothing";
                return null;
            }                        
        }

        public bool QueryVerify(Expr currExpr, out Expr preExpr)
        {
            preExpr = null;

            /*
            preExpr = null;
            Expr wrongExpr;
            if (FakeStepSatisfied(currExpr, out wrongExpr)) // correct
            {
                //find preExpr: fake now
                preExpr = CurrKnowledge.Value.RetrieveCurrExpr();
                return true;
            }
            else // wrong
            {
                return false;
            }*/

            Expr wrongExpr;
            if (FakeStepSatisfied(currExpr, out wrongExpr))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool FakeStepSatisfied2(Expr expr)
        {
            if (!(expr is CompositeExpr)) return false;
            var compositeExpr = expr as CompositeExpr;
            if (compositeExpr.Head != WellKnownSym.equals) return false;
            if (compositeExpr.Args.Count() != 2) return false;

            if (!(compositeExpr.Args[0] is LetterSym)) return false;
            var letter = compositeExpr.Args[0] as LetterSym;

            if (!(letter.Letter.ToString().Equals("Y") || letter.Letter.ToString().Equals("y"))) return false;

            if (!(compositeExpr.Args[1] is CompositeExpr)) return false;
            compositeExpr = compositeExpr.Args[1] as CompositeExpr;

            if (compositeExpr.Head != WellKnownSym.plus) return false;
            if (compositeExpr.Args.Count() != 2) return false;

            if (!(compositeExpr.Args[1] is IntegerNumber)) return false;
            var number = compositeExpr.Args[1] as IntegerNumber;
            if (!number.Num.ToString().Equals("3")) return false;

            if (!(compositeExpr.Args[0] is CompositeExpr)) return false;

            compositeExpr = compositeExpr.Args[0] as CompositeExpr;

            if (!compositeExpr.Head.Equals(WellKnownSym.times)) return false;
            if (compositeExpr.Args.Count() != 2) return false;

            if (!(compositeExpr.Args[1] is LetterSym)) return false;
            letter = compositeExpr.Args[1] as LetterSym;
            if (!(letter.Letter.ToString().Equals("X") || letter.Letter.ToString().Equals("x"))) return false;

            if (!(compositeExpr.Args[0] is IntegerNumber)) return false;
            number = compositeExpr.Args[0] as IntegerNumber;
            if (number.Num.ToString().Equals("2"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool FakeStepSatisfied(Expr expr, out Expr wrongExpr)
        {
            wrongExpr = null;

            if (!(expr is CompositeExpr)) return false;
            var compositeExpr = expr as CompositeExpr;
            if (compositeExpr.Head != WellKnownSym.equals) return false;
            if (compositeExpr.Args.Count() != 2) return false;
            if (!(compositeExpr.Args[1] is IntegerNumber)) return false;
            var zero = compositeExpr.Args[1] as IntegerNumber;
            if (!zero.Num.IsZero) return false;
            if (!(compositeExpr.Args[0] is CompositeExpr)) return false;
            compositeExpr = compositeExpr.Args[0] as CompositeExpr;

            if (compositeExpr.Head != WellKnownSym.plus) return false;
            if (compositeExpr.Args.Count() != 3) return false;

            //third term
            Expr tempExpr = compositeExpr.Args[2];
            if (!(tempExpr is CompositeExpr)) return false;
            var tempCompositeExpr = tempExpr as CompositeExpr;
            if (!tempCompositeExpr.Head.Equals(WellKnownSym.minus)) return false;
            if ((tempCompositeExpr.Args.Count() != 1)) return false;
            tempExpr = tempCompositeExpr.Args[0] as Expr;
            if (!(tempExpr is IntegerNumber)) return false;
            zero = tempExpr as IntegerNumber;
            if (!zero.Num.ToString().Equals("4")) return false;
            //second term
            tempExpr = compositeExpr.Args[1];
            if (!(tempExpr is LetterSym)) return false;
            var letter = tempExpr as LetterSym;
            if (letter.Letter != 'y') return false;
            //first term
            tempExpr = compositeExpr.Args[0];
            if (!(tempExpr is CompositeExpr)) return false;
            tempCompositeExpr = tempExpr as CompositeExpr;
            if (tempCompositeExpr.Head != WellKnownSym.times) return false;
            if (tempCompositeExpr.Args.Count() != 2) return false;
            tempExpr = tempCompositeExpr.Args[0] as Expr;
            if (!(tempExpr is IntegerNumber)) return false;
            zero = tempExpr as IntegerNumber;
            if (zero.Num.ToString().Equals("4"))
            {
                return true;
            }
            else
            {
                wrongExpr = tempExpr;
                return false;
            }
        }

        private QueryMode UpdateQuery(Expr expr, QueryMode inputMode)
        {
            if (CurrKnowledge.Key != null && expr.Equals(CurrKnowledge.Key))
            {
                return SelectKnowledgeMode(inputMode);                
            }

            CurrKnowledge = Knowledge.FirstOrDefault(x => x.Key.Equals(expr));
            if (!CurrKnowledge.Equals(default(KeyValuePair<Expr, IKnowledgeExpr>)))
            {
                return SelectKnowledgeMode(inputMode);
            }

            if (CurrQuery.PropertyExpr.Equals(expr))
            {
                return SelectPropertyMode(inputMode);
            }

            CurrQuery = QueryProperties.Find(x => x.PropertyExpr.Equals(expr));
            if (CurrQuery != null)
            {
                return SelectPropertyMode(inputMode);
            }

            return QueryMode.None;
        }

        private QueryMode SelectKnowledgeMode(QueryMode inputMode)
        {
            switch (inputMode)
            {
                case QueryMode.What:
                    return QueryMode.WhatOnKnowledge;
                case QueryMode.How:
                    return QueryMode.HowOnKnowledge;
                case QueryMode.Strategy:
                    return QueryMode.StrategyOnKnowledge;
                case QueryMode.Why:
                    return QueryMode.WhyOnKnowledge;
                default:
                    return QueryMode.None;
            }
        }

        private QueryMode SelectPropertyMode(QueryMode inputMode)
        {
            switch (inputMode)
            {
                case QueryMode.What:
                    return QueryMode.WhatOnProperty;
                case QueryMode.How:
                    return QueryMode.HowOnProperty;
                case QueryMode.Strategy:
                    return QueryMode.StrategyOnProperty;
                case QueryMode.Why:
                    return QueryMode.WhyOnProperty;
                default:
                    return QueryMode.None;
            }                                    
        }

        #endregion

        public void RemoveKnowledge(Expr expr)
        {
            KeyValuePair<Expr, IKnowledgeExpr> pair = default(KeyValuePair<Expr, IKnowledgeExpr>);
            foreach (KeyValuePair<Expr, IKnowledgeExpr> temp in Knowledge)
            {
                if (temp.Key.Equals(expr))
                {
                    pair = temp;
                    break;
                }
            }

            if (Knowledge.Keys.Contains(expr))
            {
                Knowledge.Remove(pair);
                if (pair.Equals(CurrKnowledge))
                {
                    CurrKnowledge = default(KeyValuePair<Expr, IKnowledgeExpr>);
                    if (Knowledge.Count > 0)
                    {
                        CurrKnowledge = Knowledge.Last();
                    }
                }
            }
        }

        public bool DoExpressionExist(Expr expr)
        {
            if (Knowledge.ContainsKey(expr) || QueryProperties.Exists(x => x.PropertyExpr.Equals(expr)))
            {
                return true;
            }
            else
            {
                return false;                
            }
        }

        /// <summary>
        /// Ad-hoc geometry input
        /// </summary>
        /// <param name="knowledgeExpr"></param>
        public void AddGeometryKnowledge(IKnowledgeExpr knowledgeExpr)
        {
            Knowledge.Add(knowledgeExpr.GeneralExpr, knowledgeExpr);
            CurrKnowledge = new KeyValuePair<Expr, IKnowledgeExpr>(knowledgeExpr.GeneralExpr, knowledgeExpr);
            IsLastTrace = false;
        }

        /// <summary>
        /// Ad-hoc geometry input
        /// </summary>
        /// <param name="propertyExpr"></param>
        public void AddGemetryProperty(AGPropertyExpr propertyExpr)
        {
            QueryProperties.Add(propertyExpr);
            CurrQuery = propertyExpr;
            CurrKnowledge = propertyExpr.Knowledge;
            IsLastTrace = false;
        }


        public bool IsAlgebraicKnowledgeEntity(Expr expr, out IKnowledgeExpr shape)
        {
            List<AGKnowledgeTracer> algebraTracer;
            return AGUnifier.Instance.VerifyShapeKnowledge(expr, out shape, out algebraTracer);
        }

        /// <summary>
        /// Algebra input
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public bool AddKnowledge(Expr expr)
        {
            List<AGKnowledgeTracer> algebraTracer;
            IKnowledgeExpr shape;
            bool success = AGUnifier.Instance.VerifyShapeKnowledge(expr, out shape, out algebraTracer);
            if (!success)
            {
                success = ComposeKnowledge(expr, out shape, out algebraTracer);
                if (!success)
                {
                    success = ComposeDistance(expr, out shape, out algebraTracer);               
                    if(!success)
                        return false;                
                }
            }
            shape.Tracers = algebraTracer;
            Knowledge.Add(expr, shape);
            CurrKnowledge = new KeyValuePair<Expr, IKnowledgeExpr>(expr, shape);
            IsLastTrace = false;
            return true;
        }

        public bool ComposeDistance(Expr expr, out IKnowledgeExpr shape, out List<AGKnowledgeTracer> tracer)
        {
            bool success = expr.IsDistanceForm(out shape);
            tracer = AGUnifier.Instance.RetrieveDistanceTracers2();
            return success;
        }

        public bool ComposeKnowledge(Expr expr, out IKnowledgeExpr shape, out List<AGKnowledgeTracer> tracer)
        {
            shape = null;
            tracer = null;

            List<string> letters;
            PointExpr pe1, pe2;

            if (expr.IsTwoLabel(out letters))
            {
                string lineLabel = letters[0] + letters[1];
                if (Knowledge.Count == 2)
                {
                    List<IKnowledgeExpr> lst = Knowledge.Values.ToList();
                    if (lst[0] is PointExpr && lst[1] is PointExpr)
                    {
                        pe1 = lst[0] as PointExpr;
                        pe2 = lst[1] as PointExpr;
                        shape = new LineExpr(new Line(lineLabel, (Point)pe1.AGShape, (Point)pe2.AGShape), expr);
                        tracer = AGUnifier.Instance.RetrieveLineTracers2(shape, expr);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }       
            }

            return false;
        }

        /// <summary>
        /// This method needs to handle exception
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public bool AddProperty(Expr expr, out bool isDirectQuery)
        {
            string propertyName;

            isDirectQuery = false;

            if (!expr.IsQueryFormWithQustionMark(out propertyName))
            {
                if (!(expr.IsQueryFormWithoutQuestionMark(out propertyName)))
                {
                    return false;
                }
            }
            else
            {
                isDirectQuery = true; //with ?
            }

            var property = new AGPropertyExpr(expr);           
            SearchKnowledge(propertyName, property);            
            //If no exception, add property to the list
            QueryProperties.Add(property);

            CurrQuery = property;
            CurrKnowledge = property.Knowledge;
            IsLastTrace = false;

            return true;
        }

        /// <summary>
        /// Search Algorithm based on AG Data Structure
        /// </summary>
        private void SearchKnowledge(string propertyName, AGPropertyExpr propertyExpr)
        {
            if (Knowledge.Count == 0)
            {
                throw new Exception("No Knowledge Input");
            }

            List<AGKnowledgeTracer> propertyTracer;
            Expr propertyAnswerExpr;
            bool isFound = false;
            foreach (KeyValuePair<Expr, IKnowledgeExpr> knowledge in Knowledge)
            {
                if (knowledge.Value.Transform(propertyName, out propertyAnswerExpr, out propertyTracer))
                {
                    propertyExpr.AddAnswer(knowledge, propertyAnswerExpr, propertyTracer);
                    isFound = true;
                }
            }

            if (!isFound)
            {
                throw new Exception("Cannot find property in any existing knowledge.");                
            }
        }
    }

    public enum QueryMode
    {
        What, How, Why, Strategy,
        WhatOnKnowledge, HowOnKnowledge, WhyOnKnowledge, StrategyOnKnowledge,
        WhatOnProperty,  HowOnProperty,  WhyOnProperty,  StrategyOnProperty,
        None
    }
}
