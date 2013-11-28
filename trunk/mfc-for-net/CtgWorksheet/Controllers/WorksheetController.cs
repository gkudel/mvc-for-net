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
        private ModelContext ctx;
        public WorksheetController(ModelContext ctx)
        {
            this.ctx = ctx;
        }

        [ActionMethod("Load")]
        public object Load(int id)
        {
            return new { Model = new Worksheet() { Id = id } };
        }

        public void Dispose()
        {
            ctx = null;
        }
    }
}
