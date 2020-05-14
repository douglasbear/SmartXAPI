using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayVacationEmployee
    {
        [Column("Employee Code")]
        [StringLength(50)]
        public string EmployeeCode { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(30)]
        public string FromDate { get; set; }
        [StringLength(30)]
        public string ToDate { get; set; }
        [StringLength(30)]
        public string VacationId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_Status")]
        public int NStatus { get; set; }
        [Column("D_VacDateFrom", TypeName = "datetime")]
        public DateTime? DVacDateFrom { get; set; }
        [Column("D_VacDateTo", TypeName = "datetime")]
        public DateTime? DVacDateTo { get; set; }
        [Column("X_VacType")]
        [StringLength(50)]
        public string XVacType { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_VacationStatus")]
        public int? NVacationStatus { get; set; }
        [Column("N_VacTypeID")]
        public int? NVacTypeId { get; set; }
        [Column("N_VacDays")]
        public double? NVacDays { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("X_Type")]
        [StringLength(5)]
        public string XType { get; set; }
        [Column("X_VacationCode")]
        [StringLength(50)]
        public string XVacationCode { get; set; }
        [Column("B_IsReturn")]
        public bool? BIsReturn { get; set; }
        [Column("N_VacationID")]
        public int NVacationId { get; set; }
    }
}
