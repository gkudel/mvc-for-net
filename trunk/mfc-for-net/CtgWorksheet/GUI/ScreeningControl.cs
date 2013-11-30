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
using CtgWorksheet.Model;

namespace MvcForNet.CtgWorksheet.GUI
{
    public partial class ScreeningControl : UserControl
    {
        #region Constructor
        public ScreeningControl(Screening screening, string sessionId)
        {
            InitializeComponent();
            ControllerDispatcher.GetInstance().RegisterView(this);
            SessionId = sessionId;
            Model = screening;
        }
        #endregion Constructor

        #region Properties
        [Browsable(false)]
        private string SessionId { get; set; }

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
            ControllerDispatcher.GetInstance().InvokeActionMethod("Screening", "Recalculate", new { Id = Id, A = txtA.Text, B = txtB.Text });
        }
        #endregion GUI Events

        #region Calls Back
        [ActionMethodCallBack("Screening", "Recalculate")]
        public void Loaded(int Result)
        {
            lblResult.Text = Result.ToString();
        }
        #endregion Calls Back
    }
}
