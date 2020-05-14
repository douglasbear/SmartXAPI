using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_StudentAssessment")]
    public partial class SchStudentAssessment
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_AssessmentID")]
        public int NAssessmentId { get; set; }
        [Column("X_AssessmentCode")]
        [StringLength(50)]
        public string XAssessmentCode { get; set; }
        [Column("N_ReservationID")]
        public int NReservationId { get; set; }
        [Column("D_TestDate", TypeName = "datetime")]
        public DateTime? DTestDate { get; set; }
        [Column("N_ResultID")]
        public int? NResultId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
