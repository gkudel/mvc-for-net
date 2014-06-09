﻿using System;
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
using System.Diagnostics;

namespace MvcForNet.CtgWorksheet.GUI
{
    public partial class ScreeningCtrl : UserControl
    {
        #region Constructor
        public ScreeningCtrl()
        {
            InitializeComponent();
        }

        public ScreeningCtrl(Screening screening, string sessionId)
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
            ControllerDispatcher.GetInstance().InvokeActionMethod("Screening", "Recalculate", new { id = Id, SessionId = SessionId });
        }

        private void LockClick(object sender, EventArgs e)
        {            
            ControllerDispatcher.GetInstance().InvokeActionMethod("Screening", "Lock", new { id = Id, SessionId = SessionId });
        }
        
        private void AcceptClick(object sender, EventArgs e)
        {
            ControllerDispatcher.GetInstance().InvokeActionMethod("Screening", "AcceptChanges", new { id = Id, SessionId = SessionId });
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
