using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayVacationMasterDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_VacationGroupID")]
        public int NVacationGroupId { get; set; }
        [Column("X_VacationGroupCode")]
        [StringLength(50)]
        public string XVacationGroupCode { get; set; }
        [Column("Emp Code")]
        [StringLength(50)]
        public string EmpCode { get; set; }
        [Column("Emp Name")]
        [StringLength(100)]
        public string EmpName { get; set; }
        [Column("Vacation Request", TypeName = "datetime")]
        public DateTime? VacationRequest { get; set; }
        [Column("N_TransType")]
        public int? NTransType { get; set; }
        [StringLength(30)]
        public string VacationRequestDate { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Required]
        [Column("X_VacRemarks")]
        [StringLength(250)]
        public string XVacRemarks { get; set; }
        [Column("N_VacDays")]
        [StringLength(30)]
        public string NVacDays { get; set; }
    }
}
