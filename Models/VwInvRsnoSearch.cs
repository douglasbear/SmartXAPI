using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvRsnoSearch
    {
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PRSID")]
        public int NPrsid { get; set; }
        [Column("RS No")]
        [StringLength(50)]
        public string RsNo { get; set; }
        [Column("D_PRSDate", TypeName = "datetime")]
        public DateTime? DPrsdate { get; set; }
        [Column("RS Date")]
        [StringLength(8000)]
        public string RsDate { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        public string Department { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Purpose")]
        [StringLength(50)]
        public string XPurpose { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("N_TransTypeID")]
        public int? NTransTypeId { get; set; }
    }
}
