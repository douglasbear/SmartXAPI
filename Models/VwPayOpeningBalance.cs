using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayOpeningBalance
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_AdjustmentID")]
        public int NAdjustmentId { get; set; }
        [Column("N_AdjustmentDetailsID")]
        public int NAdjustmentDetailsId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_ProcessedAmount", TypeName = "money")]
        public decimal? NProcessedAmount { get; set; }
        [Column("N_CalculatedAmount", TypeName = "money")]
        public decimal? NCalculatedAmount { get; set; }
        [Column("N_AdjustmentAmount", TypeName = "money")]
        public decimal? NAdjustmentAmount { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_PositionID")]
        public int? NPositionId { get; set; }
        [Column("N_SalaryPayMethod")]
        public int? NSalaryPayMethod { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
    }
}
