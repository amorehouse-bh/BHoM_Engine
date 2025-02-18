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

using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Geometry;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using BH.oM.Base.Attributes;
using BH.Engine.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Matter
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the unique Materials along with their volumes defining an object's make-up.")]
        [Input("elementM", "The element to get the VolumetricMaterialTakeoff from.")]
        [Input("checkForTakeoffFragment", "If true and the provided element is a BHoMObject, the incoming item is checked if it has a VolumetricMaterialTakeoff fragment attached, and if so, returns it. If false, the VolumetricMaterialTakeoff returned will be calculated, independant of fragment attached.")]
        [Output("volumetricMaterialTakeoff", "The kind of matter the element is composed of and in which volumes.")]
        public static VolumetricMaterialTakeoff IVolumetricMaterialTakeoff(this IElementM elementM, bool checkForTakeoffFragment = true)
        {
            if (elementM == null)
            {
                Base.Compute.RecordError("Cannot query the VolumetricMaterialTakeoff from a null element.");
                return null;
            }

            //If asked to look for fragment, and if fragment exists, return it
            VolumetricMaterialTakeoff matTakeoff;
            if (checkForTakeoffFragment)
            {
                if (TryGetVolumetricMaterialTakeoffFragment(elementM, out matTakeoff))
                    return matTakeoff;
            }
            //IElementMs should implement one of the following:
            // -SolidVolume and MaterialComposition or
            // -VolumetricMaterialTakeoff
            //- GeneralMaterialTakeoff
            //This method first checks if the VolumetricMaterialTakeoff method can be found and run, and if so uses it.
            //If not, it falls back to running the MaterialComposition and SolidVolume methods and gets the VolumetricMaterialTakeoff from them.


            if (TryGetVolumetricMaterialTakeoff(elementM, out matTakeoff))
                return matTakeoff;
            else
            {
                GeneralMaterialTakeoff generalMaterialTakeoff;
                if (TryGetGeneralMaterialTakeoff(elementM, out generalMaterialTakeoff))
                    return Create.VolumetricMaterialTakeoff(generalMaterialTakeoff);

                MaterialComposition matComp;
                double volume;
                if (TryGetMaterialComposition(elementM, out matComp) && TryGetSolidVolume(elementM, out volume))
                    return Create.VolumetricMaterialTakeoff(matComp, volume);
                else
                {
                    Base.Compute.RecordError($"The provided element of type {elementM.GetType()} does not implement VolumetricMaterialTakeoff or MaterialComposition and SolidVolume methods. The VolumetricMaterialTakeoff could not be extracted.");
                    return null;
                }
            }
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Tries running the VolumetricMaterialTakeoff method on the IElementM. Returns true if the method successfully can be found.")]
        private static bool TryGetVolumetricMaterialTakeoff(this IElementM elementM, out VolumetricMaterialTakeoff volumetricMaterialTakeoff)
        {
            object result;
            bool success = Base.Compute.TryRunExtensionMethod(elementM, "VolumetricMaterialTakeoff", out result);
            volumetricMaterialTakeoff = result as VolumetricMaterialTakeoff;
            return success;
        }

        /***************************************************/

        [Description("Tries to find a VolumetricMaterialTakeoff attached as a fragment to the elementM. If found, returns true.")]
        private static bool TryGetVolumetricMaterialTakeoffFragment(this IElementM elementM, out VolumetricMaterialTakeoff takeoffFragment)
        {
            takeoffFragment = null;

            //If bool is true, check for attached fragment
            IBHoMObject bhomObj = elementM as IBHoMObject;
            if (bhomObj != null)    //Is IBHoMObject
            {
                takeoffFragment = bhomObj.FindFragment<VolumetricMaterialTakeoff>();
                if (takeoffFragment != null)    //If fragment is not null (found) return true, if not, return false
                {
                    Base.Compute.RecordNote("Material takeoff used was extracted from Fragment. If any properties of the element has changed since the fragment was attached they will not be reflected in the takeoff. To use the computed takeoff set checkForTakeoffFragment to false.");
                    return true;
                }
            }

            return false;
        }

        /***************************************************/
    }
}




