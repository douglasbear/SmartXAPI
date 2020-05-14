using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayVacationReturn
    {
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [StringLength(30)]
        public string ReturnDate { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
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
        [Column("N_VacationID")]
        public int? NVacationId { get; set; }
        [Column("X_VacType")]
        [StringLength(50)]
        public string XVacType { get; set; }
        [Column("X_Remarks")]
        [StringLength(250)]
        public string XRemarks { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("X_VacationCode")]
        [StringLength(50)]
        public string XVacationCode { get; set; }
        [Column("B_IsReturn")]
        public bool? BIsReturn { get; set; }
    }
}
