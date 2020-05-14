using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayTimesheetMaster4rpt
    {
        [Column("N_BatchID")]
        public int? NBatchId { get; set; }
        [StringLength(10)]
        public string Batch { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("X_Month")]
        [StringLength(41)]
        public string XMonth { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
