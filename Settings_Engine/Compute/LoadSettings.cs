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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace BH.Engine.Settings
{
    public static partial class Compute
    {
        [Description("Load all the JSON settings stored within the provided folder into memory. If no folder is provided, the default folder of %ProgramData%/BHoM/Settings is used instead. All JSON files are scraped within the directory (including subdirectories) and deserialised to ISettings objects.")]
        [Input("settingsFolder", "Optional input to determine where to load settings from. Defaults to %ProgramData%/BHoM/Settings if no folder is provided.")]
        public static void LoadSettings(string settingsFolder = null)
        {
            if (string.IsNullOrEmpty(settingsFolder))
                settingsFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), "BHoM", "Settings"); //Defaults to C:/ProgramData/BHoM/Settings if no folder is provided

            var settingsFiles = Directory.EnumerateFiles(settingsFolder, "*.json", SearchOption.AllDirectories);

            foreach (var file in settingsFiles)
            {
                string contents = "";
                try
                {
                    contents = File.ReadAllText(file);
                }
                catch (Exception ex)
                {
                    BH.Engine.Base.Compute.RecordError(ex, $"Error when trying to read settings file: {file}.");
                }

                if (string.IsNullOrEmpty(contents))
                    continue;

                try
                {
                    ISettings settings = BH.Engine.Serialiser.Convert.FromJson(contents) as ISettings;
                    Type type = settings.GetType();
                    Global.BHoMSettings[type] = settings;
                    Global.BHoMSettingsFilePaths[type] = file;
                }
                catch (Exception ex)
                {
                    BH.Engine.Base.Compute.RecordWarning(ex, $"Cannot deserialise the contents of {file} to an ISettings object.");
                }
            }
        }
    }
}
