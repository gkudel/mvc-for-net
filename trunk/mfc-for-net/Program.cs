using MVCEngine;
using MvcForNet.CtgWorksheet.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MVCEngine.Model;
using CtgWorksheet.Model;

namespace mfc_for_net
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);           
            ControllerDispatcher.GetInstance().AppeConfigInitialization();
            ModelContext.ModelContextInitialization<WorksheetContext>();

            Application.Run(new MainForm());
        }
    }
}
