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

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Type> PropertyTypes(this object obj, bool goDeep = false)
        {
            if(obj == null)
            {
                Base.Compute.RecordWarning("Cannot query the property types of a null object. An empty list of types will be returned.");
                return new List<Type>();
            }

            return obj.GetType().PropertyTypes(goDeep);
        }

        /***************************************************/

        public static List<Type> PropertyTypes(this Type type, bool goDeep = false)
        {
            if(type == null)
            {
                Base.Compute.RecordWarning("Cannot query the property types of a null type. An empty list of types will be returned.");
                return new List<Type>();
            }

            HashSet<Type> properties = new HashSet<Type>();
            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanRead || prop.GetMethod.GetParameters().Count() > 0) continue;
                properties.Add(prop.PropertyType);
                if (goDeep)
                {
                    foreach (Type t in prop.PropertyType.PropertyObjects(true))
                        properties.Add(t);
                }
            }
            return properties.ToList();
        }

        /***************************************************/
    }
}




