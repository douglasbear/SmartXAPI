using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccGroupTrialBalanceDrCr
    {
        [Column("N_GroupID")]
        public int NGroupId { get; set; }
        [Column("X_GroupCode")]
        [StringLength(50)]
        public string XGroupCode { get; set; }
        [Column("X_GroupName")]
        [StringLength(100)]
        public string XGroupName { get; set; }
        [Column("N_Opening", TypeName = "money")]
        public decimal? NOpening { get; set; }
        [Column("N_Balance", TypeName = "money")]
        public decimal? NBalance { get; set; }
        [Column("X_Level")]
        [StringLength(100)]
        public string XLevel { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_GroupName_Ar")]
        [StringLength(50)]
        public string XGroupNameAr { get; set; }
    }
}
