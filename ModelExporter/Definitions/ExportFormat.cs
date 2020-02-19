using Autodesk.Revit.DB;

namespace ModelExporter.Definitions
{
    public class ExportFormat
    {
        public string Format { get; set; }
        public IExportContext ExportContext { get; set; }
    }
}
