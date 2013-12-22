using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCEngine.Model;

namespace CtgWorksheet.Model
{
    public class WorksheetContext : EntitiesContext
    {
        #region Tables
        public EntitiesCollection<Worksheet> Worksheets = new EntitiesCollection<Worksheet>();
        public EntitiesCollection<Screening> Screenings = new EntitiesCollection<Screening>();
        public EntitiesCollection<Probe> Probes = new EntitiesCollection<Probe>();
        public EntitiesCollection<WorksheetRow> WorksheetRows = new EntitiesCollection<WorksheetRow>();
        public EntitiesCollection<UserDefined> UserDefineds = new EntitiesCollection<UserDefined>();
        #endregion Tables
    }
}
