using MVCEngine.Attributes;
using MVCEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCTestGui.CtgWorksheet.Model;

namespace MVCTestGui.CtgWorksheet.Controllers
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

        int screeningrecid = 1;
        [ActionMethod("AddScreening")]
        public object AddScreening(int id)
        {
            return new { Model = new Screening() { Id = screeningrecid++ } };
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
