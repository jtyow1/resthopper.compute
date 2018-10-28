﻿using System;
using System.Collections.Generic;
using compute.geometry;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Newtonsoft.Json;
using Grasshopper.Kernel.Types;

namespace locust
{
    public class locustComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public locustComponent()
          : base("locust", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("serialized", "serialized", "response from the server", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Numbers", "Numbers", "Deserialized Numbers", GH_ParamAccess.list);
            pManager.AddMeshParameter("Meshes", "Meshes", "Deserialized Meshes", GH_ParamAccess.list);
            pManager.AddCircleParameter("Circles", "Circles", "Deserialized Circles", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string input = string.Empty;
            DA.GetData<string>(0, ref input);

            List<double> Numbers = new List<double>();
            List<GH_Circle> Circles = new List<GH_Circle>();
            List<GH_Mesh> Meshes = new List<GH_Mesh>();

            GrasshopperOutput objectList = JsonConvert.DeserializeObject<GrasshopperOutput>(input);
            List<GrasshopperOutputItem> items = objectList.Items;
            if (items != null)
            {
                foreach (GrasshopperOutputItem output in items)
                {
                    switch (output.TypeHint)
                    {
                        case "number": Numbers.Add(JsonConvert.DeserializeObject<double>(output.Data)); break;
                        case "circle":
                            GH_Circle circle = new GH_Circle();
                            var cast = circle.CastFrom(JsonConvert.DeserializeObject<Circle>(output.Data));
                            Circles.Add(circle); break;

                        case "mesh":
                            GH_Mesh mesh = new GH_Mesh();
                            cast = mesh.CastFrom(JsonConvert.DeserializeObject<Mesh>(output.Data));
                            Meshes.Add(mesh); break;
                    }
                }
            }

            DA.SetDataList(0, Numbers);
            DA.SetDataList(1, Meshes);
            DA.SetDataList(2, Circles);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("5E02C886-1485-4309-84C0-F07FBA36B82B"); }
        }
    }
}