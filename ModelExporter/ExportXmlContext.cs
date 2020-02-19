using Autodesk.Revit.DB;
using System.Text;
using System.Xml;

namespace ModelExporter
{
    class ExportXmlContext : IExportContext
    {
        private readonly Document doc = null;
        private readonly string outputFile = null;
        private XmlWriter writer = null;
        //private Materials m_materials = new Materials();
        //private Transformations m_transforms = new Transformations();
        //private ExportOptions m_exportOptions = null;

        public ExportXmlContext(Document doc, string outputFile)
        {
            this.doc = doc;
            this.outputFile = outputFile;
        }

        public bool Start()
        {
            writer = new XmlTextWriter(outputFile, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                Indentation = 2
            };

            writer.WriteStartDocument(false);
            writer.WriteStartElement("Document");
            writer.WriteAttributeString("Title", doc.Title);
            return true;
        }

        public void Finish()
        {
            if (null != writer)
            {
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
                writer = null;
            }
        }

        public bool IsCanceled() => false;

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            writer.WriteStartElement("View");
            writer.WriteAttributeString("Id", node.ViewId.IntegerValue.ToString());

            if (doc.GetElement(node.ViewId) is View3D view)
            {
                writer.WriteAttributeString("Name", view.Name);
            }

            CameraInfo camera = node.GetCameraInfo();
            if (camera != null)
            {
                writer.WriteStartElement("CameraInfo");
                writer.WriteAttributeString("IsPespective", camera.IsPespective.ToString());
                writer.WriteAttributeString("HorizontalExtent", camera.HorizontalExtent.ToString());
                writer.WriteAttributeString("VerticalExtent", camera.VerticalExtent.ToString());
                writer.WriteAttributeString("RightOffset", camera.RightOffset.ToString());
                writer.WriteAttributeString("UpOffset", camera.UpOffset.ToString());
                writer.WriteAttributeString("NearDistance", camera.NearDistance.ToString());
                writer.WriteAttributeString("FarDistance", camera.FarDistance.ToString());
                if (camera.IsPespective)
                {
                    writer.WriteAttributeString("TargetDistance", camera.TargetDistance.ToString());
                }
                writer.WriteEndElement();
            }

            node.LevelOfDetail = 5;

            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId)
        {
            writer.WriteEndElement();
            //m_transforms.ClearTransforms();  // reset our transform stack (ought to be clear, but just in case)
        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            writer.WriteStartElement("Element");
            writer.WriteAttributeString("Id", elementId.IntegerValue.ToString());
            var element = doc.GetElement(elementId);
            writer.WriteAttributeString("Id", element.Category.Name);
            // if we proceed, we get everything that belongs to the element

            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(ElementId elementId)
        {
            writer.WriteEndElement();
        }

        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            writer.WriteStartElement("Face");
            if (node.GetFace() is Face face)
            {
                var mesh = face.Triangulate();
                writer.WriteAttributeString("Id", face.GetHashCode().ToString());
                writer.WriteStartElement("Meshes");
                foreach (var v in mesh.Vertices)
                {
                    writer.WriteStartElement("Point");
                    writer.WriteAttributeString("X", v.X.ToString());
                    writer.WriteAttributeString("Y", v.Y.ToString());
                    writer.WriteAttributeString("Z", v.Z.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            return RenderNodeAction.Proceed;
        }

        public void OnFaceEnd(FaceNode node)
        {
            writer.WriteEndElement();
        }

        public RenderNodeAction OnInstanceBegin(InstanceNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnInstanceEnd(InstanceNode node) { }

        public void OnLight(LightNode node) { }

        public RenderNodeAction OnLinkBegin(LinkNode node)
        {
            return RenderNodeAction.Proceed;
        }

        public void OnLinkEnd(LinkNode node) { }

        public void OnMaterial(MaterialNode node)
        {
            //writer.WriteStartElement("Material");
            //writer.WriteAttributeString("MaterialId", node.MaterialId.ToString());
            //writer.WriteEndElement();
        }

        public void OnPolymesh(PolymeshTopology node) { }

        public void OnRPC(RPCNode node) { }
    }
}
