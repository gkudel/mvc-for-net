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
            //ControllerDispatcher.GetInstance().RegisterListener(this);
            SessionId = sessionId;
            _model = screening;

            txtA.DataBindings.Add(new Binding("Text", _model, "ValueA"));
            txtB.DataBindings.Add(new Binding("Text", _model, "ValueB"));
            lblResult.DataBindings.Add(new Binding("Text", _model, "ValueResult"));
        }
        #endregion Constructor

        #region Properties
        [Browsable(false)]
        private string SessionId { get; set; }

        [Browsable(false)]
        public long Id 
        {
            get { return _model.Id; }
        }
        #endregion Properties

        #region GUI Events
        private void Recalculation(object sender, EventArgs e)
        {
            /*try
            {
                ControllerDispatcher.GetInstance().InvokeActionMethod("Screening", "Recalculate", new { _model.Id } , new { SessionId });
            }
            catch(ActionMethodInvocationException exc)
            {
                MessageBox.Show(exc.Message);
            }*/
        }

        private void LockClick(object sender, EventArgs e)
        {
            /*try
            {
                ControllerDispatcher.GetInstance().InvokeActionMethod("Screening", "Lock", new { _model.Id }, new { SessionId }, sender:this);
            }
            catch (ActionMethodInvocationException exc)
            {
                MessageBox.Show(exc.Message);
            } */           
        }
        #endregion GUI Events

        #region Calls Back
        [ActionMethodCallBack("Screening", "Lock")]
        public void Locked()
        {
            btnLock.Text = _model.IsFrozen ? "UnLock" : "Lock";
        }
        #endregion Calls Back
    }
}
