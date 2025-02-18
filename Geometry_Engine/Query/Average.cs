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

using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point Average(this IEnumerable<Point> points)
        {
            int count = points.Count();
            if (count < 1)
                return null;

            Point mean = new Point { X = 0, Y = 0, Z = 0 };

            foreach (Point pt in points)
                mean += pt;

            return mean /= count;
        }

        /***************************************************/

        public static Vector Average(this List<Vector> vs)
        {
            int count = vs.Count();
            if (count < 1)
                return null;

            Vector mean = new Vector { X = 0, Y = 0, Z = 0 };

            foreach (Vector v in vs)
                mean += v;

            return mean /= count;
        }

        /***************************************************/
    }
}




