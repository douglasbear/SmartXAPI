using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSrnnoSearch
    {
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_SRNID")]
        public int NSrnid { get; set; }
        [Column("SRN No")]
        [StringLength(50)]
        public string SrnNo { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        public string Department { get; set; }
        [Column("CostCenter Code")]
        [StringLength(50)]
        public string CostCenterCode { get; set; }
        [StringLength(100)]
        public string CostCenter { get; set; }
        [Column("Department Code")]
        [StringLength(50)]
        public string DepartmentCode { get; set; }
        [Column("D_SRNDate", TypeName = "datetime")]
        public DateTime? DSrndate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("SRN Date")]
        [StringLength(8000)]
        public string SrnDate { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_Reason")]
        public string XReason { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
    }
}
