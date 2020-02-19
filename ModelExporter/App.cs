using Autodesk.Revit.UI;
using System;
using System.Windows.Media.Imaging;

namespace ModelExporter
{
    class App : IExternalApplication
    {
        static readonly string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static string AssemblyPath => assemblyPath;

        public Result OnStartup(UIControlledApplication a)
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.OutputFolder))
            {
                Properties.Settings.Default.OutputFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Properties.Settings.Default.Save();
            }

            AddMenu(a);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a) => Result.Succeeded;

        private void AddMenu(UIControlledApplication a)
        {
            var ribbonPanel = a.CreateRibbonPanel("ModelExporter");
            var resourceString = "pack://application:,,,/ModelExporter;component/Resources/";
            var buttonData = new PushButtonData("CmdModelExporter", "ModelExporter", AssemblyPath, "ModelExporter.CmdExport")
            {
                ToolTip = "Export current model to external preview format.",
                Image = new BitmapImage(new Uri(string.Concat(resourceString, "export16.png"))),
                LargeImage = new BitmapImage(new Uri(string.Concat(resourceString, "export32.png"))),
            };
            ribbonPanel.AddItem(buttonData);
        }
    }
}
