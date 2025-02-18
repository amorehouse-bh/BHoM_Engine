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

using BH.oM.Structure.Elements;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.Engine.Spatial;
using BH.oM.Physical.Materials;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates the mass of a Bar as its solid volume (generally taken as length times section area) times density(ies) of its material. No offsets or similar are taken into account.")]
        [Input("bar", "The Bar to calculate the mass for.")]
        [Output("mass", "The mass of the Bar.", typeof(Mass))]
        public static double Mass(this Bar bar)
        {
            if(bar.IsNull())
                return 0;

            double volume = bar.SolidVolume();
            MaterialComposition comp = bar.MaterialComposition();
            return comp.Materials.Zip(comp.Ratios, (m, r) => m.Density * r * volume).Sum();
        }

        /***************************************************/

        [Description("Calculates the mass of a Panel as its area times mass per area. The mass per area is for a constant thickness calculated as thickness multiplied by the density.")]
        [Input("panel", "The Panel to calculate the mass for.")]
        [Output("mass", "The mass of the Panel.", typeof(Mass))]
        public static double Mass(this Panel panel)
        {
            return panel.IsNull() ? 0 : panel.Area() * panel.Property.IMassPerArea();
        }

        /***************************************************/

    }
}




