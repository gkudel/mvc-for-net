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
            w.Description = "Workshet(" + w.Screenings.CountEntity() + ")";

            Probe probe = ctx.Probes.AddNew();
            probe.Code = "PROBE1";
            probe.Name = "Probe Nr One";
            EntitiesContext.Freeze(probe);
            probe = ctx.Probes.AddNew();
            probe.Code = "PROBE2";
            probe.Name = "Probe Nr Two";
            EntitiesContext.Freeze(probe);           

            return new { Model = w, Probes = ctx.Probes.ToList()};
        }

        [ActionMethod("AddScreening")]
        public virtual object AddScreening(long id, long probeid, string SessionId)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            Worksheet worksheet = ctx.Worksheets.FirstOrDefulatEntity(w => w.Id == id);
            if( worksheet != null)
            {
                Screening screening = ctx.Screenings.CreateInstance();
                screening.WorksheetId = worksheet.Id;
                screening.ProbetId = probeid;
                ctx.Screenings.Add(screening);
                worksheet.Description = "Workshet(" + worksheet.Screenings.CountEntity() + ")";
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
            Screening screening = ctx.Screenings.FirstOrDefulatEntity(s => s.Id == id);
            if (screening != null)
            {
                Worksheet w = screening.Worksheet;                
                ctx.Screenings.Remove(screening);
                w.Description = "Workshet(" + w.Screenings.CountEntity() + ")";
                return new { Model = w };
            }
            else
            {
                return new ErrorView();
            }            
        }

        [ActionMethod("ProbeChenged")]
        public virtual object ProbeChenged(long probeId, string SessionId)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            return new { Screenings = ctx.Screenings.WhereEntity(s => s.ProbetId == probeId).ToList() };
        }

        [ActionMethod("ScreeningChanged")]
        public virtual object ScreeningChanged(long screeningId, string SessionId)
        {
            WorksheetContext ctx = Session.GetSessionData(SessionId, "WorksheetContext").CastToType<WorksheetContext>();
            return new { Screening = ctx.Screenings.FirstOrDefulatEntity(s => s.Id == screeningId) };
        }
        #endregion Action Method
    }
}
