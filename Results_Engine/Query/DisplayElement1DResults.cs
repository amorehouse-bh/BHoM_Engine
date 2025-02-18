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

using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using System.Linq;
using System.Reflection;
using BH.oM.Geometry;
using BH.oM.Structure.Results;
using BH.oM.Graphics;
using System;
using BH.Engine.Graphics;
using BH.Engine.Geometry;
using BH.Engine.Base;
using BH.Engine.Analytical;
using BH.oM.Analytical.Results;
using BH.oM.Analytical.Elements;
using BH.oM.Graphics.Colours;
using BH.oM.Dimensional;
using BH.oM.Quantities.Attributes;
using System.Drawing;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Colours a IElement1D type BHoMObject based on the provided result and choosen property. The object will be coloured in ranges along the element depending on how the results vary along the element. Linear interpolation will be used to determine the step changes in colour.")]
        [Input("objects", "BHoMObjects to colour. Should be IElement1D type of objects.")]
        [Input("results", "The IElement1DResult to colour by.")]
        [Input("objectIdentifier", "Should either be a string specifying what property on the object that should be used to map the objects to the results, or a type of IAdapterId fragment to be used to extract the object identification, i.e. which fragment type to look for to find the identifier of the object. If no identifier is provided, the object will be scanned an IAdapterId to be used.")]
        [Input("filter", "Optional filter for the results. If nothing is provided, all results will be used.")]
        [Input("displayProperty", "The name of the property on the result to colour by. If nothing is provided, the first available property will be used.")]
        [Input("gradientOptions", "How to color the mesh, null defaults to `BlueToRed` with automatic range. Gradient required to be a SteppedGradient type. The Gradient will be turned into a stepped gradient if another gradient type is provided.")]
        [MultiOutput(0, "results", "A List of Lists of RenderGeometry, where the outer list corresponds to the object and the inner list correspond to the result steps on the element.")]
        [MultiOutput(1, "gradientOptions", "The gradientOptions that were used to colour the meshes.")]
        public static Output<List<List<RenderGeometry>>, GradientOptions> DisplayElement1DResults(this IEnumerable<IBHoMObject> objects, IEnumerable<IElement1DResult> results, string displayProperty = "", object objectIdentifier = null, ResultFilter filter = null, GradientOptions gradientOptions = null)
        {
            if (objects == null || objects.Count() < 1)
            {
                Engine.Base.Compute.RecordError("No objects found. Make sure that your objects are input correctly.");
                return new Output<List<List<RenderGeometry>>, GradientOptions>
                {
                    Item1 = new List<List<RenderGeometry>>(),
                    Item2 = gradientOptions
                };
            }
            if (results == null || results.Count() < 1)
            {
                Engine.Base.Compute.RecordError("No results found. Make sure that your results are input correctly.");
                return new Output<List<List<RenderGeometry>>, GradientOptions>
                {
                    Item1 = new List<List<RenderGeometry>>(),
                    Item2 = gradientOptions
                };
            }

            //Get function for extracting property from results
            Output<string, Func<IResult, double>, QuantityAttribute> propName_selector_quantity = results.First().ResultValueProperty(displayProperty, filter);
            Func<IResult, double> resultPropertySelector = propName_selector_quantity?.Item2;

            if (resultPropertySelector == null)
            {
                return new Output<List<List<RenderGeometry>>, GradientOptions>
                {
                    Item1 = new List<List<RenderGeometry>>(),
                    Item2 = gradientOptions
                };
            }

            List<IBHoMObject> objectList = objects.ToList();

            // Map the Results to Objects
            List<List<IElement1DResult>> mappedResults = objectList.MapResultsToObjects(results, "ObjectId", objectIdentifier, filter);

            if (gradientOptions == null)
                gradientOptions = new GradientOptions();
            else
                gradientOptions = gradientOptions.DeepClone();

            //If unset, set name of gradient options to match property and unit
            if (string.IsNullOrWhiteSpace(gradientOptions.Name))
            {
                gradientOptions.Name = propName_selector_quantity.Item1;
                QuantityAttribute quantity = propName_selector_quantity.Item3;
                if (quantity != null)
                    gradientOptions.Name += $" [{quantity.SIUnit}]";
            }

            gradientOptions.SetDefaultGradient();

            if (!(gradientOptions.Gradient is SteppedGradient))
            {
                Engine.Base.Compute.RecordNote("Method requires a SteppedGradient to function. Provided gradient has been turned into a SteppedGradient with 9 steps");
                gradientOptions.Gradient = Graphics.Convert.ToSteppedGradient(gradientOptions.Gradient, 9);
            }

            //Set up gradient based on values
            gradientOptions.SetGradientBounds(mappedResults.SelectMany(l => l.Select(x => resultPropertySelector(x))));
            gradientOptions.ApplyGradientCentering();
            double from = gradientOptions.LowerBound;
            double to = gradientOptions.UpperBound;
            IGradient gradient = gradientOptions.Gradient;

            List<Tuple<double, Color>> scaledMarkers = new List<Tuple<double, Color>>();

            foreach (var gradientPair in gradient.Markers)
            {
                scaledMarkers.Add(new Tuple<double, Color>((double)gradientPair.Key * (to - from) + from, gradientPair.Value));
            }

            List<List<RenderGeometry>> renderGeometries = new List<List<RenderGeometry>>();

            renderGeometries = objectList.AsParallel().AsOrdered().Zip(mappedResults.AsParallel().AsOrdered(), (obj, res) => ObjectResults(obj, res, resultPropertySelector, scaledMarkers)).ToList();

            return new Output<List<List<RenderGeometry>>, GradientOptions>
            {
                Item1 = renderGeometries,
                Item2 = gradientOptions
            };
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<RenderGeometry> ObjectResults(IBHoMObject obj, List<IElement1DResult> results, Func<IResult, double> resultPropertySelector, List<Tuple<double, Color>> scaledMarkers)
        {
            ICurve curve = obj.IGeometry() as ICurve;
            List<RenderGeometry> resultDisplays = new List<RenderGeometry>();

            foreach (List<IElement1DResult> objRes in results.GroupByScenario())
            {
                List<Tuple<double, double, Color>> ranges = GetColourRanges(objRes.OrderBy(x => x.Position).ToList(), resultPropertySelector, scaledMarkers);
                foreach (var range in ranges)
                {
                    resultDisplays.Add(new RenderGeometry
                    {
                        Geometry = new Line { Start = curve.IPointAtParameter(range.Item1), End = curve.IPointAtParameter(range.Item2) },
                        Colour = range.Item3
                    });
                }
            }
            return resultDisplays;
        }

        /***************************************************/

        private static List<Tuple<double, double, Color>> GetColourRanges(List<IElement1DResult> results, Func<IResult, double> selector, List<Tuple<double, Color>> markers)
        {

            //Temporary storage of ranges of the element to colour pointing at what colour item that should be used for that range
            List<Tuple<double, double, int>> indexRanges = new List<Tuple<double, double, int>>();

            //Iterate through the results. Assumes the results to be ordered by position
            for (int i = 0; i < results.Count - 1; i++)
            {
                //Get the current and next result in the list
                IElement1DResult current = results[i];
                IElement1DResult next = results[i + 1];
                //Extract the values of the current and next result of the list using the selector
                double currentVal = selector(current);
                double nextVal = selector(next);

                //Find which gradient index the marker list that the values correspond to
                int currColIndex = GetColourIndex(currentVal, markers);
                int nextColIndex = GetColourIndex(nextVal, markers);

                //If both values point to the same marker, add a range for the full range between the positions
                if (currColIndex == nextColIndex)
                    indexRanges.Add(new Tuple<double, double, int>(current.Position, next.Position, currColIndex));
                else
                {
                    //For other cases, linear interpolation will be used over the remaining colour markers
                    double startPos = current.Position;
                    double k = (next.Position - current.Position) / (nextVal - currentVal);
                    if (currColIndex < nextColIndex)
                    {
                        //If current index is lower than next index for colour markers, iterate from current to next
                        //with positive incrementation steps
                        for (int j = currColIndex; j < nextColIndex; j++)
                        {
                            double nextStepMarker = markers[j + 1].Item1;   //When incrementing in upwards direction, the next colour change will come at the next marker step
                            indexRanges.Add(GetInterpolationRange(k, nextStepMarker, currentVal, current.Position, next.Position, j, ref startPos));
                        }
                    }
                    else
                    {
                        //If current index is lower than next index for colour markers, iterate from current to next
                        //with negative incrementation steps
                        for (int j = currColIndex; j >= nextColIndex; j--)
                        {
                            double nextStepMarker = markers[j].Item1; //When incrementing in downward direction, the next colour change will come at the current marker step
                            indexRanges.Add(GetInterpolationRange(k, nextStepMarker, currentVal, current.Position, next.Position, j, ref startPos));
                        }
                    }
                    if (startPos < next.Position)   //If whole range is not covered, add value from last found start position to end of position range
                        indexRanges.Add(new Tuple<double, double, int>(startPos, next.Position, nextColIndex));
                }

            }
            //Merge subsequent ranges that are pointing to the same colour marker.
            //THis is to reduce the number of final coloured range lines to be output by the method
            for (int i = 0; i < indexRanges.Count - 1; i++)
            {
                if (indexRanges[i].Item3 == indexRanges[i + 1].Item3)   //If next range has the same colour index as current
                {
                    //Replace current range index with one that goes from current to next
                    indexRanges[i] = new Tuple<double, double, int>(indexRanges[i].Item1, indexRanges[i + 1].Item2, indexRanges[i].Item3);
                    indexRanges.RemoveAt(i + 1);    //Remove next range as included in current
                    i--;    //Reduce indexer to chech current at next step as well
                }
            }
            //Finaly, return ranges of startPos, endPos and colour by accecing the colours from the provided markers
            return indexRanges.Select(x => new Tuple<double, double, Color>(x.Item1, x.Item2, markers[x.Item3].Item2)).ToList();
        }

        /***************************************************/

        private static Tuple<double, double, int> GetInterpolationRange(double k, double nextStepMarker, double firstVal, double firstPos, double nextPos, int index, ref double startPos)
        {
            double endPos = k * (nextStepMarker - firstVal) + firstPos;
            endPos = Math.Min(endPos, nextPos);
            var range = new Tuple<double, double, int>(startPos, endPos, index);
            startPos = endPos;
            return range;
        }

        /***************************************************/

        private static int GetColourIndex(double val, List<Tuple<double, Color>> markers)
        {
            //Gets the index of the last marker in the list that has a value larger or equal to the value
            int colIndex = markers.FindLastIndex(x => x.Item1 <= val);

            //If no index is found it means the value is lower than the first item in the marker.
            //For this case, the lowest index marker is used.
            if (colIndex == -1)
                colIndex = 0;

            return colIndex;
        }

        /***************************************************/
    }
}

