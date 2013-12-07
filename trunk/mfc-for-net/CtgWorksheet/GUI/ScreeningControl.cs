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
using MVCEngine.Exceptions;

namespace MvcForNet.CtgWorksheet.GUI
{
    public partial class ScreeningControl : UserControl
    {
        #region Memebers
        private Screening _model;
        #endregion Members

        #region Constructor
        public ScreeningControl()
        {
            InitializeComponent();
        }

        public ScreeningControl(Screening screening, string sessionId)
            : this()
        {
            ControllerDispatcher.GetInstance().RegisterListener(this);
            SessionId = sessionId;
            _model = screening;
        }
        #endregion Constructor

        #region Properties
        [Browsable(false)]
        private string SessionId { get; set; }

        [Browsable(false)]
        [ViewId("")]
        public long Id 
        {
            get { return _model.Id; }
        }
        #endregion Properties

        #region GUI Events
        private void Recalculation(object sender, EventArgs e)
        {
            try
            {            
                ControllerDispatcher.GetInstance().InvokeActionMethod("Screening", "Recalculate", new { Id = Id, A = txtA.Text, B = txtB.Text });
            }
            catch(ActionMethodInvocationException exc)
            {
                MessageBox.Show(exc.Message);
            }

        }
        #endregion GUI Events

        #region Calls Back
        [ActionMethodCallBack("Screening", "Recalculate")]
        public void Recalculate(int Result)
        {            
            Invoke(new Action<int>((r) => { lblResult.Text = r.ToString(); }), new object[] { Result });            
        }

        [ActionMethodErrorBack("Screening", "Recalculate")]
        public void Recalculate(string Message, string StackTrace)
        {
            Invoke(new Action<string>((r) => { lblResult.Text = r; }), new object[] { Message });
        }
        #endregion Calls Back
    }
}
