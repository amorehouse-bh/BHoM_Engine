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
using BH.oM.Structure.MaterialFragments;
using BH.oM.Physical.Materials;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if the physical Material contains a single structural MaterialFragment, and if it does returns this fragment, containing all relevant structural material data. Returns null if not exactly one structural MaterialFragment is found.")]
        [Input("material", "The physical Material to extract a structural MaterialFragment from.")]
        [Output("strMat", "The structural MaterialFragment.")]
        public static IMaterialFragment StructuralMaterialFragment(this Material material)
        {
            if (material == null)
            {
                Engine.Base.Compute.RecordError("Material is null and therefore the StructuralMaterialFragment cannot be evaluated.");
                return null;
            }

            if (!material.IsValidStructural())
            {
                Base.Compute.RecordWarning("Material with name " + material.Name + " does not contain a structural material fragment.");
                return null;
            }

            return material.Properties.Where(x => x is IMaterialFragment).Cast<IMaterialFragment>().FirstOrDefault();
        }

        /***************************************************/

    }
}




