using System;
using System.Collections.Generic;

using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using AlgebraGeometry;
using AlgebraGeometry.Expr;
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
/*
            var variable = obj as string;
            if (variable != null)
            {
                object dict;
                EvalVariable(variable, out dict);
                return;
            }
*/
            var shapeSym = obj as AGShapeExpr;
            if (shapeSym != null)
            {
                EvalShape(shapeSym);
                return; 
            }

            var term = obj as AGPropertyExpr;
            if (term != null)
            {
                EvalTerm(term);
                return;
            }
        }

        private void UndoEvalPropogate(object obj)
        {
            if (_cache.Count == 0) return;

            var shapeSym = obj as AGShapeExpr;
            if (shapeSym != null)
            {
                return;
            }

            var term = obj as AGPropertyExpr;
            if (term != null)
            {
                UnEvalTerm(term);
                return;
            }
        }

        #region eval type checking
/*
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
*/ 
        /// <summary>
        /// AlgebraGeometry.Test.Test.Point (Test2,Test3)
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        private bool EvalShape(AGShapeExpr shapeExpr)
        {
            ShapeSymbol shapeSym = shapeExpr.ShapeSymbol;
            if (shapeSym.Shape.Concrete)
            {
                return false;
            }

            List<AGPropertyExpr> lst = GetTermFacts();
            if (lst.Count == 0)
            {
                return false;
            }
            else
            {
                var pt = shapeSym.Shape as Point;
                if (pt != null)
                {
                    foreach (AGPropertyExpr term in lst)
                    {
                        pt.Reify(term.Goal);
                    }
                    #region Interaction
                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, shapeExpr);
                    #endregion
                }                
                return true;
            }
        }

        private bool UnEvalTerm(AGPropertyExpr prop)
        {
            EqGoal term = prop.Goal;
            List<AGShapeExpr> lst = GetShapeFacts();
            if (lst.Count == 0)
            {
                return false;
            }
            else
            {
                //TODO constraint solving
                foreach (AGShapeExpr ss in lst)
                {
                    var shapeExpr = ss as AGShapeExpr;
                    var pt = shapeExpr.ShapeSymbol.Shape as Point;
                    if (pt != null)
                    {
                        pt.UnReify(term);
                        #region Interaction
                        if (KnowledgeUpdated != null)
                            KnowledgeUpdated(this, ss);
                        #endregion
                    }
                    return true;
                }
            }
            return false;
        }
    
        private bool EvalTerm(AGPropertyExpr prop)
        {
            EqGoal term = prop.Goal;
            List<AGShapeExpr> lst = GetShapeFacts();
            if (lst.Count == 0)
            {
                return false;
            }
            else
            {
                //TODO constraint solving
                foreach (AGShapeExpr ss in lst)
                {
                    var shapeExpr = ss as AGShapeExpr;                   
                    if (shapeExpr.ShapeSymbol.Shape.Concrete)
                    {
                        continue;
                    }
                    else
                    {
                        var pt = shapeExpr.ShapeSymbol.Shape as Point;
                        if (pt != null)
                        {
                            pt.Reify(term);
                            #region Interaction
                            if (KnowledgeUpdated != null)
                                KnowledgeUpdated(this, shapeExpr);
                            #endregion
                        }                
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region cache utilities

        private List<AGPropertyExpr> GetTermFacts()
        {
            var lst = new List<AGPropertyExpr>();
            foreach (KeyValuePair<object, object> pair in _cache)
            {
                var temp = pair.Value as AGPropertyExpr;
                if (temp != null)
                {
                    lst.Add(temp);
                }
            }
            return lst;     
        }

        private List<AGShapeExpr> GetShapeFacts()
        {
            var lst = new List<AGShapeExpr>();
            foreach (KeyValuePair<object, object> pair in _cache)
            {
                var temp = pair.Value as AGShapeExpr;
                if (temp != null)
                {
                    lst.Add(temp);
                }
            }
            return lst;
        }

        private object SearchFact(object key)
        {
            foreach (KeyValuePair<object, object> pair in _cache)
            {
                if (key.Equals(pair.Key))
                    return pair;
            }
            return null;
        }

        #endregion

        #region Test Purpose

        public List<AGShapeExpr> TestGetShapeFacts()
        {
            return GetShapeFacts();
        }

        #endregion
    }
}
