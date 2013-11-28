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

namespace MVCTestGui.CtgWorksheet.GUI
{
    public partial class WorksheetForm : Form
    {
        #region Members
        ControllerDispatcher _controllerdispatcher = new ControllerDispatcher();
        ModelContext _ctx = new ModelContext();
        #endregion Members
        
        #region Constructor
        public WorksheetForm()
        {
            InitializeComponent();
            _controllerdispatcher.RegisterController(typeof(WorksheetController), () => { return new object[] { _ctx }; });
            _controllerdispatcher.RegisterView(this);
        }
        #endregion Constructor

        #region GUI Events
        private void WorksheetFormLoad(object sender, EventArgs e)
        {
            TryCatchStatment.Try().Invoke(() =>
            {
                _controllerdispatcher.InvokeActionMethod("Worksheet", "Load", new { Id = -2 });
            }).Catch<ActionMethodInvocationException>((exc) =>
            {
                MessageBox.Show(exc.Message);
            });
        }
        #endregion GUI Events

        #region Calls Back
        [ActionMethodCallBack("Worksheet", "Load")]
        public void Loaded(WorksheetForm model)
        {
            MessageBox.Show("Data Loaded");
        }
        #endregion Calls Back
    }
}
