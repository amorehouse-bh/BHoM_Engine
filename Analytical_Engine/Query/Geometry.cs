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


using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Analytical.Elements;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.Engine.Spatial;
using BH.oM.Analytical.Fragments;
using BH.Engine.Base;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the geometry of a INode as a Point. Method required for automatic display in UI packages.")]
        [Input("node", "INode to get the Point from.")]
        [Output("point", "The geometry of the INode.")]
        public static Point Geometry(this INode node)
        {
            return node?.Position;
        }

        /***************************************************/

        [PreviousInputNames("link", "bar")]
        [Description("Gets the geometry of a ILink as its centreline. Method required for automatic display in UI packages.")]
        [Input("link", "ILink to get the centreline geometry from.")]
        [Output("line", "The geometry of the ILink as its centreline.")]
        public static Line Geometry<TNode>(this ILink<TNode> link)
            where TNode : INode
        {
            return new Line { Start = link?.StartNode?.Position, End = link?.EndNode?.Position };
        }

        /***************************************************/

        [Description("Gets the geometry of a IEdge as its Curve. Method required for automatic display in UI packages.")]
        [Input("edge", "IEdge to get the curve geometry from.")]
        [Output("curve", "The geometry of the IEdge as its Curve.")]
        public static ICurve Geometry(this IEdge edge)
        {
            return edge?.Curve;
        }

        /***************************************************/

        [Description("Gets the geometry of a analytical IPanel at its centre. Method required for automatic display in UI packages.")]
        [Input("panel", "IPanel to get the planar surface geometry from.")]
        [Output("surface", "The geometry of the analytical IPanel at its centre.")]
        public static PlanarSurface Geometry<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel)
            where TEdge : IEdge
            where TOpening : IOpening<TEdge>
        {
            return new PlanarSurface(
                    Engine.Geometry.Compute.IJoin(panel?.ExternalEdges?.Select(x => x?.Curve).ToList()).FirstOrDefault(),
                    panel?.Openings.SelectMany(x => Engine.Geometry.Compute.IJoin(x?.Edges.Select(y => y?.Curve).ToList())).Cast<ICurve>().ToList());
        }

        /***************************************************/

        [Description("Gets the geometry of a analytical IOpening as an outline curve. Method required for automatic display in UI packages.")]
        [Input("opening", "IOpening to get the outline geometry from.")]
        [Output("outline", "The geometry of the analytical IOpening.")]
        public static PolyCurve Geometry<TEdge>(this IOpening<TEdge> opening)
            where TEdge : IEdge

        {
            return new PolyCurve { Curves = opening?.Edges?.Select(x => x?.Curve).ToList() };
        }

        /***************************************************/

        [Description("Gets the geometry of a analytical ISurface at its centre. Method required for automatic display in UI packages.")]
        [Input("surface", "Analytical ISurface to get the geometrical Surface geometry from.")]
        [Output("surface", "The underlying surface geometry of the analytical ISurface at its centre.")]
        public static IGeometry Geometry(this BH.oM.Analytical.Elements.ISurface surface)
        {
            return surface?.Extents;
        }

        /***************************************************/

        [PreviousInputNames("mesh", "feMesh")]
        [Description("Gets the geometry of a analytical IMesh as a geometrical Mesh. A geometrical mesh only supports 3 and 4 nodes faces, while a FEMesh does not have this limitation. For FEMeshFaces with more than 4 nodes or less than 3 this operation is therefore not possible. Method required for automatic display in UI packages.")]
        [Input("mesh", "Analytical IMesh to get the mesh geometry from.")]
        [Output("mesh", "The geometry of the IMesh as a geometrical Mesh.")]
        public static Mesh Geometry<TNode, TFace>(this IMesh<TNode, TFace> mesh)
            where TNode : INode
            where TFace : IFace
        {
            Mesh geoMesh = new Mesh();

            geoMesh.Vertices = mesh?.Nodes?.Select(x => x?.Position).ToList();

            geoMesh.Faces.AddRange(mesh?.Faces?.Geometry());

            return geoMesh;
        }

        /***************************************************/

        [PreviousInputNames("faces", "feFaces")]
        [Description("Gets the geometry of a collection of IFaces as a geometrical Mesh's Faces. A geometrical mesh face only supports 3 and 4 nodes faces, while a FEMeshFace does not have this limitation. For FEMeshFaces with more than 4 nodes or less than 3 this operation is therefore not possible. Method required for automatic display in UI packages.")]
        [Input("faces", "Analytical IFaces to get the mesh faces geometry from.")]
        [Output("faces", "The geometry of the IFaces as geometrical Mesh Faces.")]
        public static IEnumerable<Face> Geometry<TFace>(this IEnumerable<TFace> faces)
            where TFace : IFace
        {
            List<Face> result = new List<Face>();
            foreach (IFace feFace in faces)
            {
                Face face = Geometry(feFace);
                if (face != null)
                    result.Add(face);
            }
            return result;
        }

        /***************************************************/

        [PreviousInputNames("face", "feFace")]
        [Description("Gets the geometry of a analytical IFace as a geometrical Mesh's Face. A geometrical mesh face only supports 3 and 4 nodes faces, while a FEMeshFace does not have this limitation. For FEMeshFaces with more than 4 nodes or less than 3 this operation is therefore not possible. Method required for automatic display in UI packages.")]
        [Input("face", "Analytical IFace to get the mesh face geometry from.")]
        [Output("face", "The geometry of the IFace as geometrical Mesh Face.")]
        public static Face Geometry(this IFace face)
        {
            if (face?.NodeListIndices == null)
                return null;

            if (face.NodeListIndices.Count < 3)
            {
                Base.Compute.RecordError("Insuffiecient node indices");
                return null;
            }
            if (face.NodeListIndices.Count > 4)
            {
                Base.Compute.RecordError("To high number of node indices. Can only handle triangular and quads");
                return null;
            }

            Face geomFace = new Face();

            geomFace.A = face.NodeListIndices[0];
            geomFace.B = face.NodeListIndices[1];
            geomFace.C = face.NodeListIndices[2];

            if (face.NodeListIndices.Count == 4)
                geomFace.D = face.NodeListIndices[3];

            return geomFace;
        }

        /***************************************************/

        [Description("Gets the geometry of a IRegion as its Perimiter curve. Method required for automatic display in UI packages.")]
        [Input("region", "IRegion to get the curve geometry from.")]
        [Output("curve", "The geometry of the IRegion as its Perimiter curve.")]
        public static ICurve Geometry(this IRegion region)
        {
            return region?.Perimeter;
        }

        /***************************************************/

        [Description("Gets the geometry of a Graph as its relation curve arrows. For relations between entities of IElement0D types and outmatic curve is created if it does not exist. Method required for automatic display in UI packages.")]
        [Input("graph", "Graph to get the geometry from.")]
        [Output("Composite Geometry", "The CompositeGeometry geometry of the Graph.")]
        public static CompositeGeometry Geometry(this Graph graph)
        {
            if(graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the geometry of a null graph.");
                return null;
            }

            Dictionary<Guid, Point> element0DGeoms = graph.Entities.Where(x => x.Value is IElement0D).ToDictionary(x => x.Key, x => ((IElement0D)x.Value).IGeometry());

            bool relNoGeom = false;
            List<IGeometry> geometries = new List<IGeometry>();
            foreach (Relation relation in graph.Relations)
            {
                if (relation != null)
                {
                    Point sourcePt, targetPt;
                    if (relation.Curve != null)
                        geometries.Add(relation.RelationArrow());   //If Relation have a curve defined, use it to display
                    else if (element0DGeoms.TryGetValue(relation.Source, out sourcePt) && element0DGeoms.TryGetValue(relation.Target, out targetPt))
                        geometries.Add(new Relation { Curve = new Line { Start = sourcePt, End = targetPt } }.RelationArrow());  //Relation between two IElement0Ds - Generate a curve between them and draw arrow
                    else
                        relNoGeom = true;   //Some relations can not be displayed automatically, flag to raise warning
                }
            }

            geometries.AddRange(element0DGeoms.Values); //Add Points representing IElement0Ds to the list

            if(relNoGeom)   //Raise warning if relations that could not be displayed was found
                Base.Compute.RecordWarning("Geometry is only displayed for Relations that either have their Curve set or span between entities that are IElement0D.");


            return new CompositeGeometry { Elements = geometries };

        }

        /***************************************************/

        
    }
}


