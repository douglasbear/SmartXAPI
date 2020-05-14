using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Log_Recruitment")]
    public partial class LogRecruitment
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_RecID")]
        public int NRecId { get; set; }
        [Column("N_RecruitmentId")]
        public int? NRecruitmentId { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_Order")]
        public int? NOrder { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_Notes")]
        [StringLength(500)]
        public string XNotes { get; set; }
    }
}
