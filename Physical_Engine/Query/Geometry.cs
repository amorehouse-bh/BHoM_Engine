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

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Physical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the centreline geometry from the framing element")]
        [Output("CL", "The centre line curve of the framing element")]
        public static ICurve Geometry(this IFramingElement framingElement)
        {
            return framingElement?.Location;
        }

        /***************************************************/

        [Description("Gets the defining surface geometry from the ISurface, the elements physical extents are further defined by its Offset")]
        [Output("surface", "The defining location surface geometry of the ISurface with its openings represented")]
        public static oM.Geometry.ISurface Geometry(this oM.Physical.Elements.ISurface surface)
        {
            ICurve exBound = (surface?.Location as PlanarSurface)?.ExternalBoundary;
            if (exBound == null)
                return null;

            List<ICurve> inBound = surface?.Openings?.Select(o => (o?.Location as PlanarSurface)?.ExternalBoundary).Where(x => x != null).ToList();
            return Engine.Geometry.Create.PlanarSurface(exBound, inBound);
        }

        /***************************************************/


    }
}



