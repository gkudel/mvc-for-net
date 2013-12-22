using MVCEngine.Model;
using MVCEngine.Model.Attributes;
using MVCEngine.Model.Attributes.Default;
using MVCEngine.Model.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using attributes = MVCEngine.Model.Attributes;

namespace CtgWorksheet.Model
{
    [Table("GP_WORKSHEETROW")]
    [attributes.Interceptor(DefaultInterceptors.SecurityInterceptor, "", RegEx = "(?=(?!^(set_Screening|set_UserDefineds)$).*)(?=^(set_))")]
    [attributes.Interceptor(DefaultInterceptors.ModificationInterceptor, "", RegEx = "(?=(?!^set_Screening|set_UserDefineds$).*)(?=^(set_|get_))")]
    [attributes.Validation.PrimaryKeyValidator(RealTimeValidation = true, ErrrorMessage = "Integrity Constraint")]
    [attributes.EntityInterceptor("Screening", "CtgWorksheet.Model.Screening, mfc-for-net", RelationName = "WorksheetRow_Screening")]
    [attributes.CollectionInterceptor("UserDefineds", "CtgWorksheet.Model.UserDefined, mfc-for-net", RelationName = "UserDefined_WorksheetRow")]
    public class WorksheetRow : Entity
    {
        [Column("GP_WROW_RECID", IsPrimaryKey = true)]
        [PrimaryKeyDefaultValue()]
        public virtual long Id { get; set; }

        [Column("GP_WROW_CODE")]
        public virtual string Code { get; set; }

        [Column("GP_WROW_NAME")]
        public virtual string Name { get; set; }

        [Column("GP_WROW_SCREENINGID", IsForeignKey = true, ForeignTable = "GP_FISHSCREENING", RelationName = "WorksheetRow_Screening", OnDelete = OnDelete.Cascade)]
        public virtual long ScreeningId { get; set; }

        public virtual Screening Screening { get; private set; }

        [DynamicProperties("Code", "Value")]
        public virtual EntitiesCollection<UserDefined> UserDefineds { get; private set; }

        public override void Dispose()
        {
            base.Dispose();
            UserDefineds.Clear();
            Screening = null;
        }
    }
}
