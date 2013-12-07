using CtgWorksheet.Model;
using MVCEngine.Attributes;
using MVCEngine.Model;
using MVCEngine.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine;

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
        public void Recalculate(int id)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Screening screening = ctx.Screenings.FirstOrDefault(s => s.Id == id);
            if (screening.IsNotNull())
            {
                screening.ValueResult = (long.Parse(screening.ValueA) + long.Parse(screening.ValueB)).ToString();
            }
        }

        [ActionMethod("Lock")]
        public object Lock(int id)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Screening screening = ctx.Screenings.FirstOrDefault(s => s.Id == id);
            if (screening.IsNotNull())
            {
                screening.IsFrozen = !screening.IsFrozen;                 
            }
            return new { Id = id };
        }
        #endregion Action Method

        #region Property
        public string SessionId { get; set; }
        #endregion Property

        #region Dispose
        public void Dispose()
        {
        }
        #endregion Dispose
    }
}
