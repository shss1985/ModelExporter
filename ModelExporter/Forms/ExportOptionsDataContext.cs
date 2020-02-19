using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace ModelExporter.Forms
{
    class ExportOptionsDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Document doc;

        public ExportOptionsDataContext(Document doc, List<View3D> views)
        {
            Views = views.OrderBy(x => x.Name).ToList();
            View = Views[0];
            this.doc = doc;
        }

        public List<View3D> Views { get; set; }
        public View3D View { get; set; }
        public string OutputFolder => Properties.Settings.Default.OutputFolder;

        private Utils.CommandHandler browse;
        public Utils.CommandHandler CmdBrowse
        {
            get
            {
                return browse ?? (
                browse = new Utils.CommandHandler(obj =>
                {
                    var parameters = obj as List<object>;
                    var window = parameters[0] as Window;

                    using (var fbd = new FolderBrowserDialog())
                    {
                        DialogResult result = fbd.ShowDialog();
                        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                        {
                            Properties.Settings.Default.OutputFolder = fbd.SelectedPath;
                            Properties.Settings.Default.Save();
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OutputFolder"));
                        }
                    }
                },
                true));
            }
        }

        public List<Definitions.ExportFormat> Formats =>
            new List<Definitions.ExportFormat>() { new Definitions.ExportFormat() { Format = "*.xaml", ExportContext = new ExportXmlContext(doc, $"{OutputFolder}\\{View.Name}.xml") } };

        public Definitions.ExportFormat Format { get; set; }

        public bool StartExport { get; set; } = false;

        private Utils.CommandHandler export;
        public Utils.CommandHandler CmdExport
        {
            get
            {
                return export ?? (
                export = new Utils.CommandHandler(obj =>
                {
                    var parameters = obj as List<object>;
                    var window = parameters[0] as Window;

                    StartExport = true;
                    window.Close();
                },
                true));
            }
        }
    }
}
