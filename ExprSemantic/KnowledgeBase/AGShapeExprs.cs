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

namespace ExprSemantic.KnowledgeBase
{
    using starPadSDK.MathExpr;
    using System;
   
    public abstract class ShapeExpr :  IEquatable<ShapeExpr>
    {
        #region Constructors

        protected ShapeExpr(Expr expr)
        {
            AST = expr;
        }

        protected ShapeExpr()
        { }

        #endregion

        #region IEquatable

        public virtual bool Equals(ShapeExpr other)
        {
            return this.Repr.Equals(other.Repr) && this.ShapeEntity.Equals(other.ShapeEntity);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ShapeExpr);
        }

        public override int GetHashCode()
        {
            return Repr.GetHashCode() ^ ShapeEntity.GetHashCode();
        }

        #endregion

        #region Properties

        public RepresentationType Repr { get; set; }
        public Shape ShapeEntity { get; set; }
        public Expr AST { get;set;}

        #endregion         
    }

    public sealed class PointExpr : ShapeExpr
    {
        public PointExpr(Expr expr)
            :base(expr)
        {
            
        }          
    }

    public sealed class LineExpr : ShapeExpr
    {
        
    }

    public abstract class QuadraticCurveExpr : ShapeExpr
    {
        
    }

    public sealed class CircleExpr : QuadraticCurveExpr
    {
        
    }

    public sealed class EllipseExpr : QuadraticCurveExpr
    {
        
    }

    public sealed class HyperbolaExpr : QuadraticCurveExpr
    {
        
    }

    public sealed class ParabolaExpr : QuadraticCurveExpr
    {
        
    }
}
