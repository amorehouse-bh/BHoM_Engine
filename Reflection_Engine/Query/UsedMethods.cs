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
using System.Reflection;
using System.Linq;
using Mono.Cecil;
using Mono.Reflection;
using BH.Engine.Base;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<MethodBase> UsedMethods(this MethodBase method, bool onlyBHoM = false)
        {
            if(method == null)
            {
                Base.Compute.RecordWarning("Cannot query the used methods of a null method. An empty list will be returned as the list of used methods.");
                return new List<MethodBase>();
            }

            try
            {
                if (method.GetMethodBody() == null)
                    return new List<MethodBase>();
                IEnumerable<MethodBase> methods = Disassembler.GetInstructions(method)
                                .Select(x => x.Operand)
                                .OfType<MethodBase>()
                                .Distinct()
                                .Where(x => x.DeclaringType.Namespace != null)
                                .SelectMany(x => x.IsAutoGenerated() ? x.UsedMethods(onlyBHoM) : new List<MethodBase> { x });

                if (onlyBHoM)
                    return methods.Where(x => x.DeclaringType.Namespace.StartsWith("BH.")).ToList();
                else
                    return methods.ToList();
            }
            catch (Exception e)
            {
                Base.Compute.RecordError("Method " + method.DeclaringType.Namespace + "." + method.Name + " failed to extract the information about the method.\nError: " + e.ToString());
                return new List<MethodBase>();
            }
        }

        /***************************************************/
    }
}




