﻿using MVCEngine.Attributes;
using MVCEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CtgWorksheet.Model;
using MVCEngine.View;

namespace CtgWorksheet.Controllers
{
    [Controller("Worksheet")]
    public class WorksheetController
    {
        #region Constructor
        public WorksheetController()
        { }
        #endregion Constructor

        #region Action Method
        [ActionMethod("Load")]
        public object Load(int id)
        {
            return new RedirectView("AddScreening") { Params = new { Model = new Worksheet() { Id = id } } };
        }

        private static int screeningrecid = 0;
        [ActionMethod("AddScreening")]
        public object AddScreening()
        {
            int ScreeningNumber = ++screeningrecid;
            return new { Model = new Screening() { Id = ScreeningNumber }, ScreeningNumber };
        }

        [ActionMethod("DeleteScreening")]
        public object AddScreening(int id)
        {
            int ScreeningNumber = --screeningrecid;
            return new { ScreeningNumber };
        }
        #endregion Action Method

        [ValueFromController("")]
        public string Test { get; set; }
    }
}
