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
using BH.oM.Humans.ViewQuality;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Humans.ViewQuality
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Define the method used to find Cvalue focal points")]
        [Input("typeNum", "0 = OffsetThroughCorners,1 = Closest,2 = Perpendicular,3 = Undefined")]
        public static CvalueFocalMethodEnum CvalueFocalMethod(int typeNum)
        {
            var enumCount = CvalueFocalMethodEnum.GetNames(typeof(CvalueFocalMethodEnum)).Length;

            //last name is undefined so total possible types is enumCount-2
            if (typeNum > enumCount - 2) typeNum = 0;

            CvalueFocalMethodEnum value = (CvalueFocalMethodEnum)typeNum;

            return value;
        }

        /***************************************************/
    }
}




