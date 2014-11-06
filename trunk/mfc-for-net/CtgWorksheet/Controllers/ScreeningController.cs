using CtgWorksheet.Model;
using MVCEngine.Model;
using MVCEngine.Tools.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.Tools;
using MVCEngine.ControllerView.Attributes;

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
        [ActionMethod("Recalculate")]
        public virtual void Recalculate(long id, string SessionId)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Screening screening = ctx.Screenings.FirstOrDefulatEntity(s => s.Id == id);
            if (screening.IsNotNull())
            {
                screening.ValueResult = (long.Parse(screening.ValueA) + long.Parse(screening.ValueB)).ToString();
            }
        }

        [ActionMethod("Lock")]
        public virtual object Lock(long id, string SessionId)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Screening screening = ctx.Screenings.FirstOrDefulatEntity(s => s.Id == id);
            if (screening.IsNotNull())
            {
                if (screening.IsFrozen) WorksheetContext.UnFreeze(screening);
                else WorksheetContext.Freeze(screening);  
            }
            return new { Id = id, Frozen = screening.IsFrozen };
        }

        [ActionMethod("AcceptChanges")]
        public virtual void AcceptChanges(long id, string SessionId)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Screening screening = ctx.Screenings.FirstOrDefulatEntity(s => s.Id == id);
            if (screening.IsNotNull())
            {
                screening.AcceptChanges();
            }
        }

        [ActionMethod("RemoveWorkshetRow")]
        public virtual void RemoveWorkshetRow(long id, string SessionId)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            WorksheetRow row = ctx.WorksheetRows.FirstOrDefulatEntity(r => r.Id == id);
            if (row.IsNotNull())
            {
                ctx.WorksheetRows.Remove(row);
            }
        }        
        
        #endregion Action Method
    }
}
