using MVCEngine.Attributes;
using MVCEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CtgWorksheet.Model;
using MVCEngine.View;
using MVCEngine.Session;
using MVCEngine;

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
        public virtual object Load(string SessionId)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Worksheet w = ctx.Worksheets.AddNew();
            w.Description = "Workshet(" + w.Screenings.Count() + ")";
            ctx.AcceptChanges();
            return new { Model = w };
        }

        [ActionMethod("AddScreening")]
        public virtual object AddScreening(long id, string SessionId)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Worksheet worksheet = ctx.Worksheets.FirstOrDefault(w => w.Id == id);
            if( worksheet != null)
            {
                Screening screening = ctx.Screenings.AddNew();
                screening.WorksheetId = worksheet.Id;
                worksheet.Description = "Workshet(" + worksheet.Screenings.Count() + ")";
                return new { Model = screening };
            }
            else
            {
                return new ErrorView();
            }
        }

        [ActionMethod("DeleteScreening")]
        public virtual object DeleteScreening(long id, string SessionId)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Screening screening = ctx.Screenings.FirstOrDefault(s => s.Id == id);
            if (screening != null)
            {
                Worksheet w = screening.Worksheet;                
                ctx.Screenings.Remove(screening);
                w.Description = "Workshet(" + w.Screenings.Count() + ")";
                return new { SreeeningNumber = w.Screenings.Count() };
            }
            else
            {
                return new ErrorView();
            }            
        }
        #endregion Action Method
    }
}
