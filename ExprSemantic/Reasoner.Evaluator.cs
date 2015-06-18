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
using starPadSDK.MathExpr;

namespace ExprSemantic
{
    public partial class Reasoner
    {
        private bool EvalExprPatterns(Expr expr, object obj, 
                                      out object output)
        {
            output = null;

            //deterministic
            var ss = obj as ShapeSymbol;
            if (ss != null) return EvalExprPatterns(expr, ss, out output);

            //deterministic
            var goal = obj as EqGoal;
            if (goal != null) return EvalExprPatterns(expr, goal, out output);

            //non-deterministic
            var str = obj as string;
            if (str != null) return EvalExprPatterns(expr, str, out output);

            //non-deterministic
            var dict = obj as Dictionary<PatternEnum, object>;
            //ambiguity of input pattern
            if (dict != null) throw new Exception("TODO");

            return false;
        }

        /// <summary>
        /// Relation Input Pattern Match
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="str"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool EvalExprPatterns(Expr expr, string str,
            out object output)
        {
            output = null;

            var charArr = str.ToCharArray();
            object relationObj;
            bool findNodes = _logicGraph.FindNodes(charArr, out relationObj);

           
        }

        /// <summary>
        /// ShapeSymbol Input Pattern Match
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="ps"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool EvalExprPatterns(Expr expr, ShapeSymbol ps,
            out object output)
        {
            _logicGraph.AddShapeNode(ps.Shape);
            output = new AGShapeExpr(expr, ps);
            return true;
        }

        /// <summary>
        /// Goal Input Patter Match
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="goal"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool EvalExprPatterns(Expr expr, EqGoal goal,
            out object output)
        {
            _logicGraph.AddGoalNode(goal);
            output = new AGPropertyExpr(expr, goal);
            return true;
        }

        public bool InferRelation(Expr inputExpr, Tuple<IKnowledge, IKnowledge> tuple, out object obj)
        {
            obj = null;

            var shapeExpr1 = tuple.Item1 as AGShapeExpr;
            var shapeExpr2 = tuple.Item2 as AGShapeExpr;
            var propExpr1 = tuple.Item1 as AGPropertyExpr;
            var propExpr2 = tuple.Item2 as AGPropertyExpr;

            bool inferResult = false;

            if (shapeExpr1 != null && shapeExpr2 != null)
            {
                var gTuple = new Tuple<object, object>(shapeExpr1.ShapeSymbol.Shape, shapeExpr2.ShapeSymbol.Shape);
                inferResult = _logicGraph.AddRelation(gTuple, out obj);
            }

            if (shapeExpr1 != null && propExpr2 != null)
            {
                var gTuple = new Tuple<object, object>(shapeExpr1.ShapeSymbol.Shape, propExpr2.Goal);
                inferResult = _logicGraph.AddRelation(gTuple, out obj);
            }

            if (propExpr1 != null && shapeExpr2 != null)
            {
                var gTuple = new Tuple<object, object>(propExpr1.Goal, shapeExpr2.ShapeSymbol.Shape);
                inferResult = _logicGraph.AddRelation(gTuple, out obj)
            }

            if (propExpr1 != null && propExpr2 != null)
            {
                var gTuple = new Tuple<object, object>(propExpr1.Goal, propExpr2.Goal);
                inferResult = _logicGraph.AddRelation(gTuple, out obj);
            }

            if (inferResult)
            {
                var gShape = obj as Shape;
                Debug.Assert(gShape != null);

                ShapeSymbol ss = null;
                var pointShape = obj as Point;
                if (pointShape != null)
                {
                    ss = new PointSymbol(pointShape);
                }

                var lineShape = obj as Line;
                if (lineShape != null)
                {
                    ss = new LineSymbol(lineShape);
                }

                Debug.Assert(ss != null);
                if (inputExpr == null) //touch input
                {
                    Debug.Assert(ss.Shape.Label != null);
                    throw new Exception("TODO touch selectioon");
                }
                else // sketch input
                {
                    obj = new AGShapeExpr(inputExpr, ss);
                }
                return true;
            }
            return false;
        }



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

        private List<IKnowledge> GetKnowledgeFacts()
        {
            var lst = new List<IKnowledge>();
            foreach (KeyValuePair<object, object> pair in _cache)
            {
               lst.Add(pair);
            }
            return lst;
        }

        #endregion

        #region Test Purpose

        public List<IKnowledge> TestGetKnowledgeFacts()
        {
            return GetKnowledgeFacts();
        }

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
