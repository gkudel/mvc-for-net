using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MVCEngine;
using MVCEngine.Attributes;
using CtgWorksheet.Model;
using CtgWorksheet.Controllers;

namespace MvcForNet.CtgWorksheet.GUI
{
    public partial class WorksheetCtrl : UserControl
    {
        public WorksheetCtrl()
        {
            InitializeComponent();
        }

        #region GUI Events
        private void WorksheetCtrl_Load(object sender, EventArgs e)
        {
            ControllerDispatcher.GetInstance().RegisterListener(this);
        }
        #endregion GUI Events

        #region Action Calls Back
        [ActionMethodCallBack("Worksheet", "ScreeningChanged")]
        public void ScreeningChanged(Screening screening)
        {
            if (screening.IsNotNull())
            {
                gridControl.DataSource = screening.WorksheetRows;
                gridControl.Enabled = true;
                gridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            }
            else
            {
                gridControl.DataSource = null;
                gridControl.Enabled = false;
                gridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None;
            }
        }
        #endregion Action Calls Back
    }
}
