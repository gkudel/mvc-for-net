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
using CtgWorksheet.Controllers;

namespace MvcForNet.CtgWorksheet.GUI
{
    public partial class ScreeningControl : UserControl
    {
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
            Id = screening.Id;

            txtA.DataBindings.Add(new Binding("Text", screening, "ValueA"));
            txtB.DataBindings.Add(new Binding("Text", screening, "ValueB"));
            lblResult.DataBindings.Add(new Binding("Text", screening, "ValueResult"));
        }
        #endregion Constructor

        #region Properties
        [Browsable(false)]
        private string SessionId { get; set; }

        [Browsable(false)]
        [Id()]
        public long Id  { get; private set; }
        #endregion Properties

        #region GUI Events
        private void Recalculation(object sender, EventArgs e)
        {
            ControllerDispatcher.GetInstance("Screening").CastToType<ScreeningController>().Recalculate(Id, SessionId);
        }

        private void LockClick(object sender, EventArgs e)
        {
            ControllerDispatcher.GetInstance("Screening").CastToType<ScreeningController>().Lock(Id, SessionId);
        }
        #endregion GUI Events

        #region Calls Back
        [ActionMethodCallBack("Screening", "Lock")]
        public void Locked(bool Frozen)
        {
            btnLock.Text = Frozen ? "UnLock" : "Lock";
        }
        #endregion Calls Back

        #region Methods
        public void Remove()
        {
            txtA.DataBindings.Clear();
            txtB.DataBindings.Clear();
            lblResult.DataBindings.Clear();
        }
        #endregion Methods
    }
}
