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
    public class ScreeningController 
    {
        #region Constructor
        public ScreeningController()
        {
        }
        #endregion Constructor

        #region Action Method
        [ActionMethod("Recalculate", OnlySender=true)]
        public virtual void Recalculate(object sender, long id, string SessionId)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Screening screening = ctx.Screenings.FirstOrDefault(s => s.Id == id);
            if (screening.IsNotNull())
            {
                screening.ValueResult = (long.Parse(screening.ValueA) + long.Parse(screening.ValueB)).ToString();
            }
        }

        [ActionMethod("Lock", OnlySender=true)]
        public virtual object Lock(object sender, long id, string SessionId)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Screening screening = ctx.Screenings.FirstOrDefault(s => s.Id == id);
            if (screening.IsNotNull())
            {
                if (screening.IsFrozen) WorksheetContext.UnFreeze(screening);
                else WorksheetContext.Freeze(screening);  
            }
            return new { Frozen = screening.IsFrozen };
        }
        #endregion Action Method
    }
}
