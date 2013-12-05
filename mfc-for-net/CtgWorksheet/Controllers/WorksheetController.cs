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
        public object Load(int id)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Worksheet w = ctx._worksheets.AddNew();
            w.Id = id;
            w.Description = "Workshet(" + screeningrecid + ")";
            ctx._worksheets.AcceptChanges();
            int c = w.Screenings.Count();
            return new RedirectView("AddScreening") { Params = new { Model = w } };
        }

        private static int screeningrecid = 0;
        [ActionMethod("AddScreening")]
        public object AddScreening()
        {
            int ScreeningNumber = ++screeningrecid;
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            ctx._worksheets[0].Description = "Workshet(" + screeningrecid + ")";
            return new { Model = new Screening() { Id = ScreeningNumber }, ScreeningNumber };
        }

        [ActionMethod("DeleteScreening")]
        public object AddScreening(int id)
        {
            int ScreeningNumber = --screeningrecid;
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            ctx._worksheets[0].Description = "Workshet(" + screeningrecid + ")";
            return new { ScreeningNumber };
        }
        #endregion Action Method

        #region Property
        public string SessionId { get; set; }
        #endregion Property
    }
}
