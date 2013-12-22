using MVCEngine;
using MVCEngine.Session;
namespace MvcForNet.CtgWorksheet.GUI
{
    partial class WorksheetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            ControllerDispatcher.GetInstance().UnRegisterListener(this);
            Session.ReleaseSession(SessionId);
            base.Dispose(disposing);
        }

        ~WorksheetForm()
        {
            Dispose();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            this.gridViewScreening = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnScreening = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumnCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumnName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDeleteScreening = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAddScreening = new System.Windows.Forms.Button();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
            this.statusCtrl = new MvcForNet.CtgWorksheet.GUI.StatusCtrl();
            this.worksheetCtrl = new MvcForNet.CtgWorksheet.GUI.WorksheetCtrl();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewScreening)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.panel1.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
            this.SuspendLayout();
            // 
            // gridViewScreening
            // 
            this.gridViewScreening.ChildGridLevelName = "Probe_Screening";
            this.gridViewScreening.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnScreening});
            this.gridViewScreening.DefaultRelationIndex = 1;
            this.gridViewScreening.GridControl = this.gridControl;
            this.gridViewScreening.Name = "gridViewScreening";
            this.gridViewScreening.OptionsView.ShowDetailButtons = false;
            this.gridViewScreening.OptionsView.ShowGroupPanel = false;
            this.gridViewScreening.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewScreening_FocusedRowChanged);
            // 
            // gridColumnScreening
            // 
            this.gridColumnScreening.Caption = "Screening";
            this.gridColumnScreening.FieldName = "Id";
            this.gridColumnScreening.Name = "gridColumnScreening";
            this.gridColumnScreening.OptionsColumn.AllowEdit = false;
            this.gridColumnScreening.OptionsColumn.AllowFocus = false;
            this.gridColumnScreening.OptionsColumn.AllowShowHide = false;
            this.gridColumnScreening.OptionsColumn.ReadOnly = true;
            this.gridColumnScreening.OptionsFilter.AllowAutoFilter = false;
            this.gridColumnScreening.OptionsFilter.AllowFilter = false;
            this.gridColumnScreening.Visible = true;
            this.gridColumnScreening.VisibleIndex = 0;
            // 
            // gridControl
            // 
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.LevelTemplate = this.gridViewScreening;
            gridLevelNode1.RelationName = "Probe_Screening";
            this.gridControl.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.gridControl.Location = new System.Drawing.Point(0, 0);
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = "gridControl";
            this.gridControl.Size = new System.Drawing.Size(318, 522);
            this.gridControl.TabIndex = 0;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView,
            this.gridViewScreening});
            this.gridControl.FocusedViewChanged += new DevExpress.XtraGrid.ViewFocusEventHandler(this.gridControl_FocusedViewChanged);
            // 
            // gridView
            // 
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumnCode,
            this.gridColumnName});
            this.gridView.GridControl = this.gridControl;
            this.gridView.Name = "gridView";
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.MasterRowEmpty += new DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventHandler(this.gridView_MasterRowEmpty);
            this.gridView.MasterRowGetChildList += new DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventHandler(this.gridView_MasterRowGetChildList);
            this.gridView.MasterRowGetRelationName += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventHandler(this.gridView_MasterRowGetRelationName);
            this.gridView.MasterRowGetRelationCount += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventHandler(this.gridView_MasterRowGetRelationCount);
            this.gridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView_FocusedRowChanged);
            // 
            // gridColumnCode
            // 
            this.gridColumnCode.Caption = "Probe code";
            this.gridColumnCode.FieldName = "Code";
            this.gridColumnCode.Name = "gridColumnCode";
            this.gridColumnCode.OptionsColumn.AllowEdit = false;
            this.gridColumnCode.OptionsColumn.AllowFocus = false;
            this.gridColumnCode.OptionsColumn.AllowMove = false;
            this.gridColumnCode.OptionsColumn.AllowShowHide = false;
            this.gridColumnCode.OptionsColumn.ReadOnly = true;
            this.gridColumnCode.OptionsFilter.AllowAutoFilter = false;
            this.gridColumnCode.OptionsFilter.AllowFilter = false;
            this.gridColumnCode.Visible = true;
            this.gridColumnCode.VisibleIndex = 0;
            // 
            // gridColumnName
            // 
            this.gridColumnName.Caption = "Name";
            this.gridColumnName.FieldName = "Name";
            this.gridColumnName.Name = "gridColumnName";
            this.gridColumnName.OptionsColumn.AllowEdit = false;
            this.gridColumnName.OptionsColumn.AllowFocus = false;
            this.gridColumnName.OptionsColumn.AllowMove = false;
            this.gridColumnName.OptionsColumn.AllowShowHide = false;
            this.gridColumnName.OptionsColumn.ReadOnly = true;
            this.gridColumnName.OptionsFilter.AllowAutoFilter = false;
            this.gridColumnName.OptionsFilter.AllowFilter = false;
            this.gridColumnName.Visible = true;
            this.gridColumnName.VisibleIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnDeleteScreening);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnAddScreening);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 564);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1037, 44);
            this.panel1.TabIndex = 0;
            // 
            // btnDeleteScreening
            // 
            this.btnDeleteScreening.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteScreening.Enabled = false;
            this.btnDeleteScreening.Location = new System.Drawing.Point(839, 9);
            this.btnDeleteScreening.Name = "btnDeleteScreening";
            this.btnDeleteScreening.Size = new System.Drawing.Size(105, 23);
            this.btnDeleteScreening.TabIndex = 2;
            this.btnDeleteScreening.Text = "Delete Screening";
            this.btnDeleteScreening.UseVisualStyleBackColor = true;
            this.btnDeleteScreening.Click += new System.EventHandler(this.DeleteScreening);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(950, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.CloseClick);
            // 
            // btnAddScreening
            // 
            this.btnAddScreening.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddScreening.Location = new System.Drawing.Point(738, 9);
            this.btnAddScreening.Name = "btnAddScreening";
            this.btnAddScreening.Size = new System.Drawing.Size(95, 23);
            this.btnAddScreening.TabIndex = 0;
            this.btnAddScreening.Text = "Add Screening";
            this.btnAddScreening.UseVisualStyleBackColor = true;
            this.btnAddScreening.Click += new System.EventHandler(this.AddScreeningClick);
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.txtDescription);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1037, 42);
            this.pnlTop.TabIndex = 2;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(12, 12);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(645, 20);
            this.txtDescription.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gridControl);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 42);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(318, 522);
            this.panel2.TabIndex = 3;
            // 
            // xtraTabControl
            // 
            this.xtraTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl.Location = new System.Drawing.Point(318, 42);
            this.xtraTabControl.Name = "xtraTabControl";
            this.xtraTabControl.Size = new System.Drawing.Size(719, 189);
            this.xtraTabControl.TabIndex = 4;
            this.xtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl_SelectedPageChanged);
            // 
            // statusCtrl
            // 
            this.statusCtrl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusCtrl.Location = new System.Drawing.Point(318, 381);
            this.statusCtrl.Name = "statusCtrl";
            this.statusCtrl.Size = new System.Drawing.Size(719, 183);
            this.statusCtrl.TabIndex = 5;
            // 
            // worksheetCtrl
            // 
            this.worksheetCtrl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.worksheetCtrl.Location = new System.Drawing.Point(318, 231);
            this.worksheetCtrl.Name = "worksheetCtrl";
            this.worksheetCtrl.Size = new System.Drawing.Size(719, 150);
            this.worksheetCtrl.TabIndex = 6;
            // 
            // WorksheetForm
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1037, 608);
            this.Controls.Add(this.xtraTabControl);
            this.Controls.Add(this.worksheetCtrl);
            this.Controls.Add(this.statusCtrl);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.panel1);
            this.Name = "WorksheetForm";
            this.Text = "Worksheet Dialog";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.WorksheetFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.gridViewScreening)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnAddScreening;
        private System.Windows.Forms.Button btnDeleteScreening;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnName;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewScreening;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumnScreening;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl;
        private CtgWorksheet.GUI.StatusCtrl statusCtrl;
        private WorksheetCtrl worksheetCtrl;
    }
}

