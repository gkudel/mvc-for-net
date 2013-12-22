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

namespace MvcForNet.CtgWorksheet.GUI
{
    public partial class StatusCtrl : UserControl
    {
        public StatusCtrl()
        {
            InitializeComponent();
        }

        #region GUI Events
        private void StatusCtrl_Load(object sender, EventArgs e)
        {
            ControllerDispatcher.GetInstance().RegisterListener(this);
        }
        #endregion GUI Events

        #region Action Calls Back
        [ActionMethodCallBack("Worksheet", "ScreeningChanged")]
        public void ScreeningChanged(Screening screening)
        {
            if (dateEdit.DataBindings.Count > 0) dateEdit.DataBindings[0].WriteValue();
            dateEdit.DataBindings.Clear();
            if (screening.IsNotNull())
            {
                dateEdit.DataBindings.Add(new Binding("EditValue", screening, "Date"));
            }
            else
            {
                dateEdit.EditValue = null;
            }

            if (checkEdit.DataBindings.Count > 0) checkEdit.DataBindings[0].WriteValue();
            checkEdit.DataBindings.Clear();
            if (screening.IsNotNull())
            {
                checkEdit.Enabled = true;
                checkEdit.DataBindings.Add(new Binding("EditValue", screening, "Completed"));
            }
            else
            {
                checkEdit.Enabled = false;
            }

            if (richTextBox.DataBindings.Count > 0) richTextBox.DataBindings[0].WriteValue();
            richTextBox.DataBindings.Clear();
            if (screening.IsNotNull())
            {
                richTextBox.DataBindings.Add(new Binding("Text", screening, "Comment"));
                richTextBox.Enabled = true;
            }
            else
            {
                richTextBox.Text = string.Empty;
                richTextBox.Enabled = false;
            }
        }
        #endregion Action Calls Back
    }
}
