using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.Model;

namespace CtgWorksheet.Model
{
    public class WorksheetContext : ModelContext
    {
        #region Tables
        public ModelBindingList<Worksheet> Worksheets = new ModelBindingList<Worksheet>();
        public ModelBindingList<Screening> Screenings = new ModelBindingList<Screening>();
        #endregion Tables
    }
}
