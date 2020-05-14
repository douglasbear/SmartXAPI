using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_StudentHistory")]
    public partial class SchStudentHistory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_Stud_HistoryID")]
        public int NStudHistoryId { get; set; }
        [Column("N_AdmissionID")]
        public int? NAdmissionId { get; set; }
        [Column("N_DivisionID")]
        public int? NDivisionId { get; set; }
        [Column("N_PromotionID")]
        public int? NPromotionId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [ForeignKey(nameof(NPromotionId))]
        [InverseProperty(nameof(SchPromotionMaster.SchStudentHistory))]
        public virtual SchPromotionMaster NPromotion { get; set; }
    }
}
