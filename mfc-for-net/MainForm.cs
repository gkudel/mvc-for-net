﻿using MvcForNet.CtgWorksheet.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mfc_for_net
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using(WorksheetForm form = new WorksheetForm())
            {
                form.ShowDialog();
            }            
        }
    }
}
