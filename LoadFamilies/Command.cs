#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Windows.Forms;
using FolderSelect;
#endregion

namespace LoadFamilies
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            bool singleTransaction = true;

            //var ofd = new OpenFileDialog();

            var fsd = new FolderSelectDialog();
            fsd.ShowDialog();
            string familyDir = fsd.FileName;

            var revitFiles = Directory.EnumerateFiles(familyDir, "*.rfa", SearchOption.AllDirectories);


            if(singleTransaction)
            {
                // Modify document within a single transaction
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Transaction Name");
                    foreach (string filepath in revitFiles)
                     {
                        Debug.Print("File: {0}\n", filepath);
                        doc.LoadFamily(filepath);
                     }
                    tx.Commit();
                }
            } else {
                // Modify document within a multiple transactions
                foreach (string filepath in revitFiles)
                {
                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Transaction Name");
                    
                            Debug.Print("File: {0}\n", filepath);
                            doc.LoadFamily(filepath);
                    
                        tx.Commit();
                    }
                }
            }

            return Result.Succeeded;
        }
    }
}
