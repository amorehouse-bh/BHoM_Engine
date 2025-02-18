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

using BH.oM.Base.Debugging;
using BH.oM.Base.Attributes;
using System.Linq;
using System.ComponentModel;
using System;
using BH.Engine.Base.Objects;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Records an event in the BHoM event log.")]
        [Input("message", "Message related to the event to be logged.")]
        [Input("type", "Type of the event to be logged.")]
        [Output("success", "True if the event is logged successfully.")]
        public static bool RecordEvent(string message, EventType type = EventType.Unknown)
        {
            return RecordEvent(new Event { Message = message, Type = type });
        }

        /***************************************************/

        [Description("Record an event with details of a C# exception within the BHoM logging system.")]
        [Input("exception", "The C# exception being caught to provide the event and stack information for.")]
        [Input("message", "An optional additional message which will be displayed first in the event log.")]
        [Input("type", "Type of the event to be logged.")]
        [Output("success", "True if the event has been successfully recorded as a BHoM Event.")]
        public static bool RecordEvent(Exception exception, string message = "", EventType type = EventType.Unknown)
        {
            if (exception == null)
                return false;

            string exceptionMessage = "";

            Exception e = exception;
            while(e != null)
            {
                if (!string.IsNullOrEmpty(exceptionMessage))
                    exceptionMessage += $"{Environment.NewLine}{Environment.NewLine}";

                exceptionMessage += $"{e.Message}";

                e = e.InnerException;
            }

            if (!string.IsNullOrEmpty(message))
                message = $"{message}\n\n{exceptionMessage}";
            else
                message = exceptionMessage;

            return RecordEvent(new Event { Message = message, StackTrace = exception.StackTrace, Type = type });
        }

        /***************************************************/

        [Description("Records an event in the BHoM event log.")]
        [Input("newEvent", "Event to be logged.")]
        [Output("success", "True if the event is logged successfully.")]
        public static bool RecordEvent(Event newEvent)
        {
            if (newEvent == null)
            {
                Compute.RecordError("Cannot record a null event.");
                return false;
            }

            if (string.IsNullOrEmpty(newEvent.StackTrace))
            {
                string trace = System.Environment.StackTrace;
                newEvent.StackTrace = string.Join("\n", trace.Split('\n').Skip(4).ToArray());
            }

            lock (Global.DebugLogLock)
            {
                Log log = Query.DebugLog();
                log.AllEvents.Add(newEvent);
                log.CurrentEvents.Add(newEvent);
                OnEventRecorded(newEvent);
            }

            return true;
        }


        /***************************************************/
        /**** Public Events                             ****/
        /***************************************************/

        [Description("Gets raised every time an event gets recorded in the debug log (see BH.Engine.Compute.RecordEvent method).")]
        public static event EventHandler<Event> EventRecorded;


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void OnEventRecorded(Event ev)
        {
            if (ev != null)
            {
                EventRecorded?.Invoke(null, ev);
            }
        }

        /***************************************************/
    }
}

