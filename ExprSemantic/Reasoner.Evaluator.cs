using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using AlgebraGeometry;
using CSharpLogic;
using NUnit.Framework;

namespace ExprSemantic
{
    public partial class Reasoner
    {
        /// <summary>
        /// The purpose of EvalPropogate is to reify all caching facts.
        /// </summary>
        /// <param name="obj"></param>
        private void EvalPropogate(object obj)
        {
            if (_cache.Count == 0) return;

            bool result = false;
            var variable = obj as string;
            if (variable != null)
            {
                object dict;
                EvalVariable(variable, out dict);
                return;
            }

            var shapeSym = obj as ShapeSymbol;
            if (shapeSym != null)
            {
                EvalShape(shapeSym);
                return; 
            }

            var term = obj as EqGoal;
            if (term != null)
            {
                EvalTerm(term);
                return;
            }
        }

        private void UndoEvalPropogate(object obj)
        {
            if (_cache.Count == 0) return;

            var shapeSym = obj as ShapeSymbol;
            if (shapeSym != null)
            {
                return;
            }

            var term = obj as EqGoal;
            if (term != null)
            {
                UnEvalTerm(term);
                return;
            }
        }

        #region eval type checking

        /// <summary>
        /// input:  x=
        /// output: x = 3
        /// </summary>
        private bool EvalVariable(string token, out object keyValues)
        {
            keyValues = null;
            var variable = new Var(token);

            List<ShapeSymbol> lst = GetShapeFacts();
            if (lst.Count == 0)
            {
                return false;
            }
            else
            {
                //TODO constraint solving
                var dict = new Dictionary<string, object>();
                foreach (ShapeSymbol d in lst)
                {
                    object result = LogicSharp.Run(variable, d.Shape);
                    if (result != null)
                    {
                        dict.Add(token, result);
                    }
                }
                keyValues = dict;
                return dict.Count != 0;
            }
        }
        
        /// <summary>
        /// Check AlgebraGeometry.Test.Test.Point (Test2,Test3)
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        private bool EvalShape(ShapeSymbol shapeSym)
        {
            if (shapeSym.Shape.Concrete)
            {
                return false;
            }

            List<EqGoal> lst = GetTermFacts();
            if (lst.Count == 0)
            {
                return false;
            }
            else
            {
                var pt = shapeSym.Shape as Point;
                if (pt != null)
                {
                    pt.Reify(lst);
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, shapeSym);
                }                
                return true;
            }
        }

        private bool UnEvalTerm(EqGoal term)
        {
            List<ShapeSymbol> lst = GetShapeFacts();
            if (lst.Count == 0)
            {
                return false;
            }
            else
            {
                //TODO constraint solving
                foreach (ShapeSymbol ss in lst)
                {
                    var shapeSym = ss as ShapeSymbol;
                    Assert.NotNull(shapeSym);

                    var pt = shapeSym.Shape as Point;
                    if (pt != null)
                    {
                        pt.UnReify(term);
                        if (KnowledgeUpdated != null)
                            KnowledgeUpdated(this, shapeSym);
                    }
                    return true;
                }
            }
            return false;
        }
    
        private bool EvalTerm(EqGoal term)
        {
            List<ShapeSymbol> lst = GetShapeFacts();
            if (lst.Count == 0)
            {
                return false;
            }
            else
            {
                //TODO constraint solving
                foreach (ShapeSymbol ss in lst)
                {
                    var shapeSym = ss as ShapeSymbol;
                    Assert.NotNull(shapeSym);
                    if (shapeSym.Shape.Concrete)
                    {
                        continue;
                    }
                    else
                    {
                        var pt = shapeSym.Shape as Point;
                        if (pt != null)
                        {
                            pt.Reify(term);
                            if(KnowledgeUpdated != null)
                                KnowledgeUpdated(this, shapeSym);
                        }                
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region cache utilities

        private List<EqGoal> GetTermFacts()
        {
            var lst = new List<EqGoal>();
            foreach (KeyValuePair<string, object> pair in _cache)
            {
                var temp = pair.Value as EqGoal;
                if (temp != null)
                {
                    lst.Add(temp);
                }
            }
            return lst;     
        }

        private List<ShapeSymbol> GetShapeFacts()
        {
            var lst = new List<ShapeSymbol>();
            foreach (KeyValuePair<string, object> pair in _cache)
            {
                var temp = pair.Value as ShapeSymbol;
                if (temp != null)
                {
                    lst.Add(temp);
                }
            }
            return lst;
        }

        private object SearchFact(string key)
        {
            foreach (KeyValuePair<string, object> pair in _cache)
            {
                if (key.Equals(pair.Key))
                    return pair;
            }
            return null;
        }

        #endregion

        #region Test Purpose

        public List<ShapeSymbol> TestGetShapeFacts()
        {
            return GetShapeFacts();
        }

        #endregion
    }
}
