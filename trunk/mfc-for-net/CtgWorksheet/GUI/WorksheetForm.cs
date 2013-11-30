using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MVCEngine;
using MVCEngine.Model;
using MVCEngine.Exceptions;
using MVCEngine.Attributes;
using MVCTestGui.CtgWorksheet.Controllers;
using MVCTestGui.CtgWorksheet.Model;

namespace MvcForNet.CtgWorksheet.GUI
{
    public partial class WorksheetForm : Form
    {
        #region Members
        ModelContext _ctx = new ModelContext();
        #endregion Members
        
        #region Constructor
        public WorksheetForm()
        {
            InitializeComponent();
            ControllerDispatcher.GetInstance().RegisterController(typeof(WorksheetController), () => { return new object[] { _ctx }; });
            ControllerDispatcher.GetInstance().RegisterView(this);
        }
        #endregion Constructor

        #region GUI Events
        private void WorksheetFormLoad(object sender, EventArgs e)
        {
            TryCatchStatment.Try().Invoke(() =>
            {
                ControllerDispatcher.GetInstance().InvokeActionMethod("Worksheet", "Load", new { Id = 1 } );
            }).Catch<ActionMethodInvocationException>((exc) =>
            {
                MessageBox.Show(exc.Message);
            });
        }

        private void AddScreeningClick(object sender, EventArgs e)
        {
            TryCatchStatment.Try().Invoke(() =>
            {
                ControllerDispatcher.GetInstance().InvokeActionMethod("Worksheet", "AddScreening", null);
            }).Catch<ActionMethodInvocationException>((exc) =>
            {
                MessageBox.Show(exc.Message);
            });
        }
        #endregion GUI Events

        #region Calls Back
        [ActionMethodCallBack("Worksheet", "Load")]
        public void Loaded(Worksheet model)
        {
        }

        [ActionMethodCallBack("Worksheet", "AddScreening")]
        public void ScreeningAdded(Screening model)
        {
            ScreeningControl screening = new ScreeningControl(model);
            screening.Dock = DockStyle.Fill;
            TabPage tabpage = new TabPage("Screenig" + model.Id);
            tabpage.Controls.Add(screening);
            tabScreening.TabPages.Add(tabpage);
            tabScreening.SelectedTab = tabpage;
        }
        #endregion Calls Back
    }
}
