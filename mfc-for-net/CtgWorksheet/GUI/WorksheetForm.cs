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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

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
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void AddScreeningClick(object sender, EventArgs e)
        {
            Probe probe = gridView.GetFocusedRow() as Probe;
            if (probe.IsNotNull())
            {
                ControllerDispatcher.GetInstance("Worksheet").CastToType<WorksheetController>().AddScreening(_worksheetid, probe.Id, SessionId);
            }
        }

        private void DeleteScreening(object sender, EventArgs e)
        {
            if(xtraTabControl.SelectedTabPage.IsNotNull())
            {
                ScreeningCtrl control = xtraTabControl.SelectedTabPage.Controls[0].CastToType<ScreeningCtrl>();
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

        private void gridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            Probe probe = gridView.GetFocusedRow() as Probe;
            if (probe.IsNotNull())
            {
                ControllerDispatcher.GetInstance("Worksheet").CastToType<WorksheetController>().ProbeChenged(probe.Id, SessionId);
            }
        }

        private void gridView_MasterRowEmpty(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                Probe probe = gridView.GetRow(e.RowHandle) as Probe;
                if (probe.IsNotNull())
                {
                    e.IsEmpty = probe.Screenings.Count == 0;
                }
            }
        }

        private void gridView_MasterRowGetChildList(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            if (e.RowHandle >= 0)
            {
                Probe probe = gridView.GetRow(e.RowHandle) as Probe;
                if (probe.IsNotNull())
                {
                    e.ChildList = probe.Screenings;
                }
            }
        }

        private void gridView_MasterRowGetRelationName(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "Probe_Screening";
        }

        private void gridView_MasterRowGetRelationCount(object sender, DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gridViewScreening_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.IsNotNull())
            {
                Screening screening = view.GetRow(e.FocusedRowHandle) as Screening;
                FocusProperProbe(screening);
                ControllerDispatcher.GetInstance("Worksheet").CastToType<WorksheetController>().ScreeningChanged(screening.Id, SessionId);
            }
        }

        private void gridControl_FocusedViewChanged(object sender, DevExpress.XtraGrid.ViewFocusEventArgs e)
        {
            GridView view = e.View as GridView;
            if (view.IsNotNull())
            {
                if (view.Name == "gridViewScreening")
                {
                    Screening screening = view.GetRow(view.FocusedRowHandle) as Screening;
                    FocusProperProbe(screening);
                    ControllerDispatcher.GetInstance("Worksheet").CastToType<WorksheetController>().ScreeningChanged(screening.Id, SessionId);
                }
            }
        }

        private void FocusProperProbe(Screening screening)
        {
            if (screening.IsNotNull())
            {
                for (int i = 0; i < gridView.RowCount; i++)
                {
                    Probe probe = gridView.GetRow(i) as Probe;
                    if (probe.IsNotNull())
                    {
                        if (probe.Id == screening.ProbetId)
                        {
                            if (gridView.FocusedRowHandle != i)
                            {
                                gridView.FocusedRowHandle = i;
                            }
                            break;
                        }
                    }
                }
            } 
        }

        private void xtraTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            InvokeScreeningChanged();
        }

        private void InvokeScreeningChanged()
        {
            long id = -1;
            if (xtraTabControl.SelectedTabPage.IsNotNull())
            {
                ScreeningCtrl control = xtraTabControl.SelectedTabPage.Controls[0].CastToType<ScreeningCtrl>();                
                if (control.IsNotNull())
                {
                    id = control.Id;
                }                
            }
            ControllerDispatcher.GetInstance("Worksheet").CastToType<WorksheetController>().ScreeningChanged(id, SessionId);
        }
        #endregion GUI Events

        #region Properties
        [Browsable(false)]
        private string SessionId { get; set; }
        #endregion Properties

        #region Calls Back
        [ActionMethodCallBack("Worksheet", "Load")]
        public void Loaded(Worksheet model, List<Probe> probes)
        {
            _worksheetid = model.Id;
            gridControl.DataSource = probes;
            txtDescription.DataBindings.Add(new Binding("Text", model, "Description"));
            InvokeScreeningChanged();
        }

        [ActionMethodCallBack("Worksheet", "AddScreening")]
        public void ScreeningAdded(Screening model)
        {
            ScreeningCtrl screening = new ScreeningCtrl(model, SessionId);
            screening.Dock = DockStyle.Fill;
            XtraTabPage tabpage = new XtraTabPage();
            tabpage.Text = "Screenig" + model.Id;
            tabpage.Controls.Add(screening);
            xtraTabControl.TabPages.Add(tabpage);
            xtraTabControl.SelectedTabPage = tabpage;

            btnDeleteScreening.Enabled = model.Worksheet.Screenings.Count() > 0;
        }

        [ActionMethodCallBack("Worksheet", "DeleteScreening")]
        public void ScreeningDeleted(int SreeeningNumber)
        {
            if (xtraTabControl.SelectedTabPage.IsNotNull())
            {
                XtraTabPage tp = xtraTabControl.SelectedTabPage;
                ScreeningCtrl control = tp.Controls[0].CastToType<ScreeningCtrl>();
                if (control.IsNotNull())
                {
                    control.Remove();
                    xtraTabControl.TabPages.Remove(tp);
                    tp.Dispose();
                }
            }
            btnDeleteScreening.Enabled = SreeeningNumber > 0;
        }

        [ActionMethodCallBack("Worksheet", "ProbeChenged")]
        public void ProbeChenged(List<Screening> Screenings)
        {
            this.xtraTabControl.SelectedPageChanged -= new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl_SelectedPageChanged);
            for (int i = xtraTabControl.TabPages.Count - 1; i >= 0; i--)
            {
                XtraTabPage tp = xtraTabControl.TabPages[i];
                ScreeningCtrl control = tp.Controls[0].CastToType<ScreeningCtrl>();
                if (control.IsNotNull())
                {
                    control.Remove();
                    xtraTabControl.TabPages.Remove(tp);
                    tp.Dispose();
                }
            }
            btnDeleteScreening.Enabled = false;           
            foreach (Screening screening in Screenings)
            {
                ScreeningAdded(screening);
            }
            InvokeScreeningChanged();
            this.xtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl_SelectedPageChanged);
        }

        [ActionMethodCallBack("Worksheet", "ScreeningChanged")]
        public void ScreeningChanged(Screening screening)
        {
            foreach (XtraTabPage tp in xtraTabControl.TabPages)
            {
                ScreeningCtrl control = tp.Controls[0].CastToType<ScreeningCtrl>();
                if (control.IsNotNull() && control.Id == screening.Id)
                {
                    if (xtraTabControl.SelectedTabPage != tp)
                    {
                        this.xtraTabControl.SelectedPageChanged -= new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl_SelectedPageChanged);
                        xtraTabControl.SelectedTabPage = tp;
                        this.xtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl_SelectedPageChanged);
                    }
                    break;
                }
            }

            Probe probe = gridView.GetFocusedRow() as Probe;
            if (probe.IsNotNull())
            {
                GridView view = gridView.GetDetailView(gridView.FocusedRowHandle, 0) as GridView;
                if (view.IsNotNull())
                {
                    for (int i = 0; i < view.RowCount; i++)
                    {
                        Screening s = view.GetRow(i) as Screening;
                        if (s.IsNotNull() && s.Id == screening.Id)
                        {
                            if (view.FocusedRowHandle != i)
                            {
                                view.FocusedRowChanged -= new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewScreening_FocusedRowChanged);
                                view.FocusedRowHandle = i;
                                view.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewScreening_FocusedRowChanged);
                            }
                            break;
                        }
                    }
                }
            }
        }
        #endregion Calls Back     
    }
}
