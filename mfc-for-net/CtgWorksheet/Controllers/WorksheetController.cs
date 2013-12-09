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
        public object Load()
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Worksheet w = ctx.Worksheets.AddNew();
            w.Description = "Workshet(" + w.Screenings.Count() + ")";
            ctx.Worksheets.Add(w);
            return new RedirectView("AddScreening") { Params = new { Model = w }, RedirectParams = new { Id = w.Id } };
        }

        [ActionMethod("AddScreening")]
        public object AddScreening(int id)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Worksheet worksheet = ctx.Worksheets.FirstOrDefault(w => w.Id == id);
            if( worksheet != null)
            {
                Screening screening = ctx.Screenings.AddNew();
                screening.WorksheetId = worksheet.Id;

                worksheet.Description = "Workshet(" + worksheet.Screenings.Count() + ")";
                return new { Model = screening};
            }
            else
            {
                return new ErrorView();
            }
        }

        [ActionMethod("DeleteScreening")]
        public object DeleteScreening(int id)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Screening screening = ctx.Screenings.FirstOrDefault(s => s.Id == id);
            if (screening != null)
            {
                Worksheet w = screening.Worksheet;                
                ctx.Screenings.Remove(screening);
                w.Description = "Workshet(" + w.Screenings.Count() + ")";
                return new { Id = id };
            }
            else
            {
                return new ErrorView();
            }            
        }
        #endregion Action Method

        #region Property
        public string SessionId { get; set; }
        #endregion Property
    }
}
