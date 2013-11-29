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
            _controllerdispatcher.UnRegisterView(this);
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAddScreening = new System.Windows.Forms.Button();
            this.tabScreening = new System.Windows.Forms.TabControl();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnAddScreening);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 448);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(669, 44);
            this.panel1.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(582, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnAddScreening
            // 
            this.btnAddScreening.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddScreening.Location = new System.Drawing.Point(481, 9);
            this.btnAddScreening.Name = "btnAddScreening";
            this.btnAddScreening.Size = new System.Drawing.Size(95, 23);
            this.btnAddScreening.TabIndex = 0;
            this.btnAddScreening.Text = "Add Screening";
            this.btnAddScreening.UseVisualStyleBackColor = true;
            this.btnAddScreening.Click += new System.EventHandler(this.AddScreeningClick);
            // 
            // tabScreening
            // 
            this.tabScreening.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabScreening.Location = new System.Drawing.Point(0, 0);
            this.tabScreening.Name = "tabScreening";
            this.tabScreening.SelectedIndex = 0;
            this.tabScreening.Size = new System.Drawing.Size(669, 448);
            this.tabScreening.TabIndex = 1;
            // 
            // WorksheetForm
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 492);
            this.Controls.Add(this.tabScreening);
            this.Controls.Add(this.panel1);
            this.Name = "WorksheetForm";
            this.Text = "Worksheet Dialog";
            this.Load += new System.EventHandler(this.WorksheetFormLoad);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnAddScreening;
        private System.Windows.Forms.TabControl tabScreening;
    }
}

