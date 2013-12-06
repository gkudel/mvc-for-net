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
using CtgWorksheet.Controllers;
using CtgWorksheet.Model;
using MVCEngine.Session;
using MVCEngine.Internal;

namespace MvcForNet.CtgWorksheet.GUI
{
    public partial class WorksheetForm : Form
    {        
        #region Constructor
        public WorksheetForm()
        {
            InitializeComponent();
        }
        #endregion Constructor

        #region GUI Events
        private void WorksheetFormLoad(object sender, EventArgs e)
        {
            SessionId = Session.CreateSession();
            Session.SetSessionData(SessionId, "WorksheetContext", new WorksheetContext());            
            ControllerDispatcher.GetInstance().RegisterListener(this);
            try
            {
                ControllerDispatcher.GetInstance().InvokeActionMethod("Worksheet", "Load", new { Id = 1 }, new { SessionId });
            }
            catch(ActionMethodInvocationException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void AddScreeningClick(object sender, EventArgs e)
        {
            try
            {
                ControllerDispatcher.GetInstance().InvokeActionMethod("Worksheet", "AddScreening", null, new { SessionId });
            }
            catch(ActionMethodInvocationException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void DeleteScreening(object sender, EventArgs e)
        {
            if(tabScreening.SelectedTab.IsNotNull())
            {
                ScreeningControl control = tabScreening.SelectedTab.Controls[0].CastToType<ScreeningControl>();
                if (control.IsNotNull())
                {
                    try
                    {
                        ControllerDispatcher.GetInstance().InvokeActionMethod("Worksheet", "DeleteScreening", new { Id = control.Id }, new { SessionId });
                    }
                    catch(ActionMethodInvocationException exc) 
                    {
                        MessageBox.Show(exc.Message);
                    }
                }
            }
        }

        private void CloseClick(object sender, EventArgs e)
        {
            Close();
        }
        #endregion GUI Events

        #region Properties
        [Browsable(false)]
        private string SessionId { get; set; }
        #endregion Properties

        #region Calls Back
        [ActionMethodCallBack("Worksheet", "Load")]
        public void Loaded(Worksheet model)
        {
            txtDescription.DataBindings.Add(new Binding("Text", model, "Description"));
        }

        [ActionMethodCallBack("Worksheet", "AddScreening")]
        public void ScreeningAdded(Screening model, int screeningNumber)
        {
            ScreeningControl screening = new ScreeningControl(model, SessionId);
            screening.Dock = DockStyle.Fill;
            TabPage tabpage = new TabPage("Screenig" + model.Id);
            tabpage.Controls.Add(screening);
            tabScreening.TabPages.Add(tabpage);
            tabScreening.SelectedTab = tabpage;

            btnDeleteScreening.Enabled = screeningNumber > 0;
        }

        [ActionMethodCallBack("Worksheet", "DeleteScreening")]
        public void ScreeningDeleted(int screeningNumber)
        {
            if (tabScreening.SelectedTab.IsNotNull())
            {
                TabPage tp = tabScreening.SelectedTab;
                tabScreening.TabPages.Remove(tp);
                tp.Dispose();
            }
            btnDeleteScreening.Enabled = screeningNumber > 0;
        }
        #endregion Calls Back
    }
}
