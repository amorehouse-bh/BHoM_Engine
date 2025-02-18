﻿/*
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
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/
        
        private static FragmentSet DeserialiseFragmentSet(this BsonValue bson, FragmentSet value, string version, bool isUpgraded)
        {
            if (value == null)
                value = new FragmentSet();

            BsonArray array = null;
            if (bson.IsBsonNull)
                return null;
            else if (bson.IsBsonArray)
            {
                array = bson.AsBsonArray;
            }
            else if (bson.IsBsonDocument)
            {
                BsonDocument doc = bson.AsBsonDocument;
                if (doc.Contains("_v"))
                    array = doc["_v"].AsBsonArray;
                else if (doc.Contains("_Items"))
                    array = doc["_Items"].AsBsonArray;
            }

            if (array != null)
            {
                foreach (BsonValue item in array)
                {
                    IFragment fragment = item.IDeserialise(version, isUpgraded) as IFragment;
                    if (fragment != null)
                        value.Add(fragment);
                }
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise a FragmentSet and received " + bson.ToString() + " instead.");
            }

            return value;
        }

        /*******************************************/
    }
}
