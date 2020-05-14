using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_PrefectManager")]
    public partial class SchPrefectManager
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_CaseID")]
        public int NCaseId { get; set; }
        [Required]
        [Column("X_CaseCode")]
        [StringLength(50)]
        public string XCaseCode { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("X_Violation")]
        [StringLength(150)]
        public string XViolation { get; set; }
        [Column("N_SanctionID")]
        public int? NSanctionId { get; set; }
        [Column("X_Remarks")]
        [StringLength(150)]
        public string XRemarks { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("X_Other")]
        [StringLength(150)]
        public string XOther { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
