using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmpTimesheetMaster
    {
        [Column("N_TimeSheetID")]
        public int? NTimeSheetId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_BatchID")]
        public int? NBatchId { get; set; }
        [Column("X_BatchCode")]
        [StringLength(20)]
        public string XBatchCode { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_CatagoryId")]
        public int? NCatagoryId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [StringLength(20)]
        public string BatchCode { get; set; }
    }
}
