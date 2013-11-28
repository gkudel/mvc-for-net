using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MVCEngine.Attributes;

namespace MvcForNet.CtgWorksheet.GUI
{
    public partial class Screenig : UserControl
    {
        #region Member
        private int _id = -1;
        #endregion Member

        #region Constructor
        public Screenig(int id)
        {
            InitializeComponent();
            this._id = id;

        }
        #endregion Constructor

        #region Properties
        [ViewId("")]
        public int Id 
        {
            get { return _id; }
        }
        #endregion Properties
    }
}
