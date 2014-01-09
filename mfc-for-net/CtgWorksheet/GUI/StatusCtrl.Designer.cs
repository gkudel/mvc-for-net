using MVCEngine;
namespace MvcForNet.CtgWorksheet.GUI
{
    partial class StatusCtrl
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
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkEdit = new DevExpress.XtraEditors.CheckEdit();
            this.dateEdit = new DevExpress.XtraEditors.DateEdit();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkEdit);
            this.panel1.Controls.Add(this.dateEdit);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(670, 49);
            this.panel1.TabIndex = 1;
            // 
            // checkEdit
            // 
            this.checkEdit.Location = new System.Drawing.Point(163, 19);
            this.checkEdit.Name = "checkEdit";
            this.checkEdit.Properties.Caption = "Completed";
            this.checkEdit.Properties.ValueChecked = "Y";
            this.checkEdit.Properties.ValueGrayed = "N";
            this.checkEdit.Properties.ValueUnchecked = "N";
            this.checkEdit.Size = new System.Drawing.Size(75, 19);
            this.checkEdit.TabIndex = 1;
            // 
            // dateEdit
            // 
            this.dateEdit.EditValue = null;
            this.dateEdit.Enabled = false;
            this.dateEdit.Location = new System.Drawing.Point(13, 16);
            this.dateEdit.Name = "dateEdit";
            this.dateEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});            
            this.dateEdit.Size = new System.Drawing.Size(129, 20);
            this.dateEdit.TabIndex = 0;
            // 
            // richTextBox
            // 
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox.Location = new System.Drawing.Point(0, 49);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(670, 318);
            this.richTextBox.TabIndex = 2;
            this.richTextBox.Text = "";
            // 
            // StatusCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.panel1);
            this.Name = "StatusCtrl";
            this.Size = new System.Drawing.Size(670, 367);
            this.Load += new System.EventHandler(this.StatusCtrl_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checkEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateEdit.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.CheckEdit checkEdit;
        private DevExpress.XtraEditors.DateEdit dateEdit;
        private System.Windows.Forms.RichTextBox richTextBox;
    }
}
