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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.Engine.Base;
using BH.oM.Base.Attributes;
using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a list of Environment Spaces with the provided spacetype assigned by a string and a point in the space.\nThe method checks whether the space perimeter IsContaining the point. The string is being parsed to an Enum to set the space type. .")]
        [Input("spaces", "A collection of Environment Spaces to set the type for.")]
        [Input("searchPoints", "A collection of points to search. The points should be contained by the space geometry.")]
        [Input("type", "A string representing the space type to assign.")]
        [Input("ignoreCase", "Whether or not the parse will be case sensitive.")]
        [Output("spaces", "A collection of modified Environment Spaces with assigned space types.")]
        public static List<Space> AssignSpaceTypeByPoint(this List<Space> spaces, List<Point> searchPoints, string type, bool ignoreCase = true)
        {
            SpaceType spaceType = SpaceType.Undefined;
            object value = Enum.Parse(typeof(SpaceType), type, ignoreCase);
            if (value != null)
                spaceType = (SpaceType)value;
            else
            {
                BH.Engine.Base.Compute.RecordError("This string does not match any of the options for SpaceType Enum");
                return null;
            }

            return spaces.AssignSpaceTypeByPoint(searchPoints, spaceType);
        }

        [Description("Returns a list of Environment Spaces with the provided space type assigned by an Enum and a point in the space.\n The method checks whether the space perimeter IsContaining the point.")]
        [Input("spaces", "A collection of Environment Spaces to set the type for.")]
        [Input("searchPoints", "A collection of points to search. The points should be contained by the space geometry.")]
        [Input("spaceType", "The space type to assign.")]
        [Output("spaces", "A collection of modified Environment Spaces with assigned space types.")]
        public static List<Space> AssignSpaceTypeByPoint(this List<Space> spaces, List<Point> searchPoints, SpaceType spaceType)
        {
            List<Space> returnSpaces = new List<Space>();
            for (int x = 0; x < searchPoints.Count; x++)
            {
                Space update = spaces.Where(a => a.Perimeter.IIsContaining(new List<Point> { searchPoints[x] })).FirstOrDefault();
                if (update == null)
                    continue;

                update.SpaceType = spaceType;
                returnSpaces.Add(update);
            }
            return returnSpaces;
        }
    }
}


