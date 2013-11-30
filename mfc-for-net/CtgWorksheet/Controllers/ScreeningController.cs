﻿using MVCEngine.Attributes;
using MVCEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtgWorksheet.Controllers
{
    [Controller("Screening")]    
    public class ScreeningController : IDisposable
    {
        #region Constructor
        public ScreeningController()
        {
        }
        #endregion Constructor

        #region Action Method
        [ActionMethod("Recalculate")]
        public object Recalculate(int id, int A, int B)
        {
            return new { Result = A + B };
        }
        #endregion Action Method

        #region Dispose
        public void Dispose()
        {
        }
        #endregion Dispose
    }
}