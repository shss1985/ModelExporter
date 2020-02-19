using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Windows;

namespace ModelExporter.Forms
{
    public partial class ExportOptions : Window
    {
        public ExportOptions(Document doc, List<View3D> views)
        {
            InitializeComponent();
            DataContext = new ExportOptionsDataContext(doc, views);
        }

        public bool StartExport => 
            ((ExportOptionsDataContext)DataContext).StartExport;

        public Definitions.ExportFormat Format =>
            ((ExportOptionsDataContext)DataContext).Format;
    }
}
