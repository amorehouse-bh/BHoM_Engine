/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */


using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using BH.Engine.Geometry;
using BH.Engine.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Determines whether a panel's outline is a square.")]
        [Input("panel", "The IPanel to check if the outline is a square.")]
        [Output("bool", "True for panels with a square outline or false for panels with a non square outline.")]
        public static bool IsOutlineSquare<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel)
            where TEdge : IEdge
            where TOpening : IOpening<TEdge>
        {
            PolyCurve polycurve = ExternalPolyCurve(panel);
            if (polycurve == null)
                return false;

            if (polycurve.SubParts().Any(x => !x.IIsLinear()))
                return false;

            List<Point> points = polycurve.DiscontinuityPoints();
            if (points.Count != 4)
                return false;
            if (!points.IsCoplanar())
                return false;

            List<Vector> vectors = VectorsBetweenPoints(points);

            List<double> angles = AnglesBetweenVectors(vectors);

            //Check the three angles are pi/2 degrees within tolerance
            if (angles.Any(x => Math.Abs(Math.PI / 2 - x) > Tolerance.Angle))
                return false;

            //Check all lengths are the same within tolerance
            double length = vectors.First().Length();
            return vectors.Skip(0).All(x => (Math.Abs(x.Length() - length) < Tolerance.Distance)) ? true : false;
        }

        /***************************************************/

    }

}


