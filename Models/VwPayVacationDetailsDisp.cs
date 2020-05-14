using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayVacationDetailsDisp
    {
        [StringLength(50)]
        public string VacationCode { get; set; }
        [Column("N_VacationGroupID")]
        public int? NVacationGroupId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("Emp Code")]
        [StringLength(50)]
        public string EmpCode { get; set; }
        [Column("Emp Name")]
        [StringLength(100)]
        public string EmpName { get; set; }
        [Column("Vacation Request", TypeName = "datetime")]
        public DateTime? VacationRequest { get; set; }
        [Column("Vacation Type")]
        [StringLength(50)]
        public string VacationType { get; set; }
        [Column("N_TransType")]
        public int? NTransType { get; set; }
        [StringLength(30)]
        public string Vacation { get; set; }
        [StringLength(30)]
        public string VacationRequestDate { get; set; }
        [StringLength(30)]
        public string VacTypeId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_VacationStartDate", TypeName = "datetime")]
        public DateTime? DVacationStartDate { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_VacationID")]
        public int NVacationId { get; set; }
        [Required]
        [Column("D_VacDateFrom")]
        [StringLength(8000)]
        public string DVacDateFrom { get; set; }
        [Required]
        [Column("D_VacDateTo")]
        [StringLength(8000)]
        public string DVacDateTo { get; set; }
        [Required]
        [Column("X_VacRemarks")]
        [StringLength(250)]
        public string XVacRemarks { get; set; }
        [Column("N_VacDays")]
        [StringLength(30)]
        public string NVacDays { get; set; }
        [Column("D_VacDateFromNew", TypeName = "datetime")]
        public DateTime? DVacDateFromNew { get; set; }
        [Column("B_IsAdjustEntry")]
        public bool? BIsAdjustEntry { get; set; }
    }
}
