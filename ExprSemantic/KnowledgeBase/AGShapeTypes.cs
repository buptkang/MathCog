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

namespace AGSemantic.KnowledgeBase
{
    public enum RepresentationType
    {
        Explicit, Implicit, Parametric
    }

    public enum CoordinateSystemType
    {
        Cartesian, Polar
    }
 
    public enum ShapeType
    {
        Point = 0,
        Line = 1,
        QuadraticCurve = 2,
        Circle = 3,
        Ellipse = 4,
        Parabola = 5,
        Hyperbola = 6,
        None = -1       
    }
}
