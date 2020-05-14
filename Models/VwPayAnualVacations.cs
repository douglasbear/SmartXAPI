using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayAnualVacations
    {
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [StringLength(30)]
        public string ReturnDate { get; set; }
        [StringLength(30)]
        public string ReturnId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_VacationReturnCode")]
        [StringLength(50)]
        public string XVacationReturnCode { get; set; }
        [Column("D_ReturnDate", TypeName = "datetime")]
        public DateTime? DReturnDate { get; set; }
        [Column("D_VacDateFrom", TypeName = "datetime")]
        public DateTime? DVacDateFrom { get; set; }
        [Column("D_VacDateTo", TypeName = "datetime")]
        public DateTime? DVacDateTo { get; set; }
        [Column("N_VacDays")]
        public double? NVacDays { get; set; }
        [Column("N_VacationID")]
        public int? NVacationId { get; set; }
        [Column("X_Remarks")]
        [StringLength(250)]
        public string XRemarks { get; set; }
        [Required]
        [Column("status")]
        [StringLength(13)]
        public string Status { get; set; }
        [Column("N_VacTypeID")]
        public int? NVacTypeId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_Type")]
        [StringLength(5)]
        public string XType { get; set; }
    }
}
