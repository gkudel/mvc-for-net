using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MVCEngine.Attributes;
using MVCEngine;
using MVCTestGui.CtgWorksheet.Model;

namespace MvcForNet.CtgWorksheet.GUI
{
    public partial class ScreeningControl : UserControl
    {
        #region Constructor
        public ScreeningControl(Screening screening)
        {
            InitializeComponent();
            Model = screening;
        }
        #endregion Constructor

        #region Properties
        [Browsable(false)]
        [ViewId("")]
        public long Id 
        {
            get { return Model.Id; }
        }

        [Browsable(false)]
        private Screening Model { get; set; }
        #endregion Properties

        #region GUI Events
        private void Recalculation(object sender, EventArgs e)
        {
        }
        #endregion GUI Events
    }
}
