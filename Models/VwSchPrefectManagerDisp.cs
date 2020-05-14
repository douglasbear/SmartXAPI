using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchPrefectManagerDisp
    {
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_CaseID")]
        public int NCaseId { get; set; }
        [Required]
        [Column("X_CaseCode")]
        [StringLength(50)]
        public string XCaseCode { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("X_Violation")]
        [StringLength(150)]
        public string XViolation { get; set; }
        [StringLength(50)]
        public string Sanction { get; set; }
        [Column("X_Status")]
        [StringLength(50)]
        public string XStatus { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
    }
}
