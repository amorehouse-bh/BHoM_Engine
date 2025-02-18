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
using System.Linq;

namespace BH.Engine.Serialiser
{
    public static partial class Compute
    {

        /*******************************************/
        /**** Private Methods                   ****/
        /*******************************************/
        
        private static T[] DeserialiseArray<T>(this BsonValue bson, T[] value, string version, bool isUpgraded)
        {
            bson = ExtractValue(bson);
            if (bson.IsBsonNull)
            {
                return null;
            }
            else if (!bson.IsBsonArray)
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise an array and received " + bson.ToString() + " instead.");
                return value;
            }

            List<T> values = new List<T>();
            foreach (BsonValue item in bson.AsBsonArray)
                values.Add((T)item.IDeserialise(typeof(T), null, version, isUpgraded));

            return values.ToArray();
        }

        /*******************************************/

        private static T[,] DeserialiseArray<T>(this BsonValue bson, T[,] value, string version, bool isUpgraded)
        {
            bson = ExtractValue(bson);
            if (bson.IsBsonNull)
            {
                return null;
            }
            else if (!bson.IsBsonArray)
            {
                BH.Engine.Base.Compute.RecordError("Expected to deserialise an array and received " + bson.ToString() + " instead.");
                return value;
            }

            List<T[]> values = new List<T[]>();
            foreach (BsonValue item in bson.AsBsonArray)
                values.Add(item.DeserialiseArray(new T[0], version, isUpgraded));

            int maxLength = values.Select(x => x.Length).Max();
            T[,] array = new T[values.Count, maxLength];
            for (int i = 0; i < values.Count; i++)
            {
                for (int j = 0; j < maxLength; j++)
                    array[i, j] = values[i][j];
            }
            return array;


        }

        /*******************************************/
    }
}
