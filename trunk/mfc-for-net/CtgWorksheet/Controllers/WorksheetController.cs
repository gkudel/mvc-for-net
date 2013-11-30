using MVCEngine.Attributes;
using MVCEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CtgWorksheet.Model;

namespace CtgWorksheet.Controllers
{
    [Controller("Worksheet")]
    public class WorksheetController : IDisposable
    {
        #region Member
        private ModelContext ctx;
        #endregion Member

        #region Constructor
        public WorksheetController(ModelContext ctx)
        {
            this.ctx = ctx;
        }
        #endregion Constructor

        #region Action Method
        [ActionMethod("Load")]
        public object Load(int id)
        {
            return new { Model = new Worksheet() { Id = id } };
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

        #region Dispose
        public void Dispose()
        {
            ctx = null;
        }
        #endregion Dispose
    }
}
