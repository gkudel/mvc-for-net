using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.Model;

namespace CtgWorksheet.Model
{
    public class WorksheetContext : ModelContext
    {
        public ModelBindingList<Worksheet> _worksheets = new ModelBindingList<Worksheet>();
    }
}
