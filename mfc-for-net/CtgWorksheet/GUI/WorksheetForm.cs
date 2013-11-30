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
            ControllerDispatcher.GetInstance().RegisterController(typeof(WorksheetController));
            ControllerDispatcher.GetInstance().RegisterController(typeof(ScreeningController));
            ControllerDispatcher.GetInstance().RegisterView(this);
        }
        #endregion Constructor

        #region GUI Events
        private void WorksheetFormLoad(object sender, EventArgs e)
        {
            TryCatchStatment.Try().Invoke(() =>
            {
                ControllerDispatcher.GetInstance().InvokeActionMethod("Worksheet", "Load", new { Id = 1 }, () => { return new object[] { _ctx }; });
            }).Catch<ActionMethodInvocationException>((exc) =>
            {
                MessageBox.Show(exc.Message);
            });
        }

        private void AddScreeningClick(object sender, EventArgs e)
        {
            TryCatchStatment.Try().Invoke(() =>
            {
                ControllerDispatcher.GetInstance().InvokeActionMethod("Worksheet", "AddScreening", null, () => { return new object[] { _ctx }; });
            }).Catch<ActionMethodInvocationException>((exc) =>
            {
                MessageBox.Show(exc.Message);
            });
        }

        private void DeleteScreening(object sender, EventArgs e)
        {
            if(tabScreening.SelectedTab.IsNotNull())
            {
                ScreeningControl control = tabScreening.SelectedTab.Controls[0].CastToType<ScreeningControl>();
                if (control.IsNotNull())
                {
                    TryCatchStatment.Try().Invoke(() =>
                    {
                        ControllerDispatcher.GetInstance().InvokeActionMethod("Worksheet", "DeleteScreening", new { Id = control.Id }, () => { return new object[] { _ctx }; });
                    }).Catch<ActionMethodInvocationException>((exc) =>
                    {
                        MessageBox.Show(exc.Message);
                    });
                }
            }
        }
        #endregion GUI Events

        #region Calls Back
        [ActionMethodCallBack("Worksheet", "Load")]
        public void Loaded(Worksheet model)
        {
        }

        [ActionMethodCallBack("Worksheet", "AddScreening")]
        public void ScreeningAdded(Screening model, int screeningNumber)
        {
            ScreeningControl screening = new ScreeningControl(model);
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
