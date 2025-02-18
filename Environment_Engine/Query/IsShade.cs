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

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        [Description("Ascertain whether a given PanelType is a type of shade.")]
        [Input("panelType", "PanelType to determine if it's a shade type or not.")]
        [Output("isShade", "Will return true if the provided panel type is a shade type.")]
        public static bool IsShade(this PanelType panelType)
        {
            List<PanelType> shadeTypes = new List<PanelType>() {
                PanelType.Shade,
                PanelType.TranslucentShade,
            };

            return shadeTypes.Contains(panelType);
        }

        [Description("Ascertain whether a given panel is a shading panel.")]
        [Input("panel", "Panel to determine if it's a shade type or not.")]
        [Output("isShade", "Will return true if the panel is a shade type.")]
        public static bool IsShade(this Panel panel)
        {
            if (panel == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query whether a panel is a shade or not if it is null.");
                return false;
            }

            return panel.Type.IsShade();
        }

    }
}

