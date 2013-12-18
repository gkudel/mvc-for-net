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
using System.Diagnostics;

namespace MvcForNet.CtgWorksheet.GUI
{
    public partial class WorksheetForm : Form
    {
        #region Members
        private long _worksheetid;
        #endregion Members

        #region Constructor
        public WorksheetForm()
        {
            InitializeComponent();
            _worksheetid = -1;
        }
        #endregion Constructor

        #region GUI Events
        private void WorksheetFormLoad(object sender, EventArgs e)
        {
            SessionId = Session.CreateSession();
            Session.SetSessionData(SessionId, "WorksheetContext", new WorksheetContext()
            {
                ContextModifed = () =>
                {
                    this.Text = "Worksheet Dialog(*)";
                },
                ChangesAccepted = () =>
                {
                    this.Text = "Worksheet Dialog"; 
                }
            });            
            ControllerDispatcher.GetInstance().RegisterListener(this);
            try
            {
                ControllerDispatcher.GetInstance("Worksheet").CastToType<WorksheetController>().Load(SessionId);
            }
            catch (Exception exc)
            { }
        }

        private void AddScreeningClick(object sender, EventArgs e)
        {
            ControllerDispatcher.GetInstance("Worksheet").CastToType<WorksheetController>().AddScreening(_worksheetid, SessionId);
        }

        private void DeleteScreening(object sender, EventArgs e)
        {
            if(tabScreening.SelectedTab.IsNotNull())
            {
                ScreeningControl control = tabScreening.SelectedTab.Controls[0].CastToType<ScreeningControl>();
                if (control.IsNotNull())
                {
                    ControllerDispatcher.GetInstance("Worksheet").CastToType<WorksheetController>().DeleteScreening(control.Id, SessionId);
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
            _worksheetid = model.Id;
            txtDescription.DataBindings.Add(new Binding("Text", model, "Description"));
        }

        [ActionMethodCallBack("Worksheet", "AddScreening")]
        public void ScreeningAdded(Screening model)
        {
            ScreeningControl screening = new ScreeningControl(model, SessionId);
            screening.Dock = DockStyle.Fill;
            TabPage tabpage = new TabPage("Screenig" + model.Id);
            tabpage.Controls.Add(screening);
            tabScreening.TabPages.Add(tabpage);
            tabScreening.SelectedTab = tabpage;

            btnDeleteScreening.Enabled = model.Worksheet.Screenings.Count() > 0;
        }

        [ActionMethodCallBack("Worksheet", "DeleteScreening")]
        public void ScreeningDeleted(int SreeeningNumber)
        {
            if (tabScreening.SelectedTab.IsNotNull())
            {
                TabPage tp = tabScreening.SelectedTab;
                ScreeningControl control = tp.Controls[0].CastToType<ScreeningControl>();
                if (control.IsNotNull())
                {
                    control.Remove();
                    tabScreening.TabPages.Remove(tp);
                    tp.Dispose();
                }
            }
            btnDeleteScreening.Enabled = SreeeningNumber > 0;
        }
        #endregion Calls Back
    }
}
