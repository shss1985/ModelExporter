using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ModelExporter.Forms;
using System.Linq;

namespace ModelExporter
{
    [Transaction(TransactionMode.Manual)]
    public class CmdExport : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (commandData.Application.ActiveUIDocument is UIDocument uidoc)
            {
                var doc = uidoc.Document;
                var views = new FilteredElementCollector(doc)
                            .OfClass(typeof(View3D))
                            .Cast<View3D>()
                            .Where(v => v.CanBePrinted && !v.IsTemplate)
                            .ToList();

                if (0 == views.Count)
                {
                    message = "No 3D views in the document.";
                    return Result.Failed;
                }

                var exportOptions = new ExportOptions(doc, views);
                new System.Windows.Interop.WindowInteropHelper(exportOptions)
                {
                    Owner = Autodesk.Windows.ComponentManager.ApplicationWindow
                };
                exportOptions.ShowDialog();

                if (exportOptions.StartExport)
                {
                    using (CustomExporter customExporter =
                        new CustomExporter(doc, exportOptions.Format.ExportContext))
                    {
                        var dc = (ExportOptionsDataContext)exportOptions.DataContext;
                        try
                        {
                            customExporter.Export(dc.View);
                        }
                        catch (System.Exception ex)
                        {
                            message = "Exception: " + ex.Message;
                            return Result.Failed;
                        }
                    }
                }

                return Result.Succeeded;
            }
            else
            {
                message = "Run this command in an active project document.";
                return Result.Failed;
            }
        }
    }
}
