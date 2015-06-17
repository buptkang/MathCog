using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
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

        #region multiple candidates filtering search algo

        private IKnowledge SearchBySemantic(List<object> objs)
        {
            if (_cache.Count == 0)
            {
                foreach (object obj in objs)
                {
                    var ishape = obj as AGShapeExpr;
                    if (ishape != null) return ishape;
                }
                throw new Exception("Cannot reach here");
            }

           
            foreach (object obj in objs)
            {
                var iproperty = obj as AGPropertyExpr;
                if (iproperty != null) return iproperty;
            }

            //TODO
            throw new Exception("Cannot reach here");

            //Rule: If cached Shape contains the var, then it must be the goal.            
            foreach (KeyValuePair<object, object> pair in _cache)
            {
                var ishape = pair.Value as AGShapeExpr;
                if (ishape != null)
                {
                    //the current shape contains the var, 
                    //which current obj has.
                                        
                }
            }

        }

        [Obsolete]
        private void ExtractVariables(object obj)
        {
            var shapeSym = obj as AGShapeExpr;
            if (shapeSym != null)
            {
                var shape = shapeSym.ShapeSymbol.Shape;

                if (shape.Label != null)
                {
                    var labelVar = new Var(shape.Label);
                    if (!_globalVarCache.Contains(labelVar))
                    {
                        _globalVarCache.Add(labelVar);
                    }
                }

                var point = shape as Point;
                if (point != null)
                {
                    if (point.XCoordinate is Var && !_globalVarCache.Contains(point.XCoordinate))
                    {
                        _globalVarCache.Add((Var)point.XCoordinate);
                    }

                    if (point.YCoordinate is Var && !_globalVarCache.Contains(point.YCoordinate))
                    {
                        _globalVarCache.Add((Var)point.YCoordinate);
                    }
                }

                var line = shape as Line;
                if (line != null)
                {
                    if (line.A is Var && !_globalVarCache.Contains(line.A))
                    {
                        _globalVarCache.Add((Var)line.A);                        
                    }

                    if (line.B is Var && !_globalVarCache.Contains(line.B))
                    {
                        _globalVarCache.Add((Var)line.B);
                    }

                    if (line.C is Var && !_globalVarCache.Contains(line.C))
                    {
                        _globalVarCache.Add((Var)line.C);
                    }
                }
                return;
            }

            var term = obj as AGPropertyExpr;
            if (term != null)
            {
                Dictionary<object, object> dict = term.Goal.ToDict();
                foreach (KeyValuePair<object, object> pair in dict)
                { 
                    if (pair.Key is Var && _globalVarCache.Contains(pair.Key))
                    {
                        _globalVarCache.Add((Var)pair.Key);
                    }
                }
                return;
            }
        }

        #endregion

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

                var line = shapeSym.Shape as Line;
                if (line != null)
                {
                    foreach (AGPropertyExpr term in lst)
                    {
                        line.Reify(term.Goal);
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

                    var line = shapeExpr.ShapeSymbol.Shape as Line;
                    if (line != null)
                    {
                        line.UnReify(term);
                        #region Interaction
                        if (KnowledgeUpdated != null)
                            KnowledgeUpdated(this, ss);
                        #endregion
                    }
                }
            }
            return false;
        }
    
        private void EvalTerm(AGPropertyExpr prop)
        {
            EqGoal goal = prop.Goal;
            List<AGShapeExpr> lst = GetShapeFacts();
            if (lst.Count == 0) return; // No shape to eval

            //TODO constraint solving
            foreach (AGShapeExpr ss in lst)
            {
                var shapeExpr = ss as AGShapeExpr;
                if (shapeExpr.ShapeSymbol.Shape.Concrete) continue; 

                var pt = shapeExpr.ShapeSymbol.Shape as Point;
                if (pt != null)
                {
                    pt.Reify(goal);

                    #region Interaction

                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, shapeExpr);

                    #endregion
                }

                var line = shapeExpr.ShapeSymbol.Shape as Line;
                if (line != null)
                {
                    line.Reify(goal);

                    #region Interaction

                    if (KnowledgeUpdated != null)
                        KnowledgeUpdated(this, shapeExpr);

                    #endregion
                }
            }
            return;
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

        #endregion

        #region Test Purpose

        public List<AGShapeExpr> TestGetShapeFacts()
        {
            return GetShapeFacts();
        }

        public List<AGPropertyExpr> TestGetProperties()
        {
            return GetTermFacts();
        }

        #endregion
    }
}
