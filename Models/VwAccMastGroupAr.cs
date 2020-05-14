using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccMastGroupAr
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_GroupID")]
        public int NGroupId { get; set; }
        [Column("Group Code")]
        [StringLength(50)]
        public string GroupCode { get; set; }
        [Column("Group Name")]
        [StringLength(50)]
        public string GroupName { get; set; }
        [Column("X_Level")]
        [StringLength(100)]
        public string XLevel { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
