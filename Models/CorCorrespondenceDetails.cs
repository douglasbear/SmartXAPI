using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Cor_CorrespondenceDetails")]
    public partial class CorCorrespondenceDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CorrespondenceID")]
        public int NCorrespondenceId { get; set; }
        [Key]
        [Column("N_CorDetailsID")]
        public int NCorDetailsId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_AttachmentID")]
        public int? XAttachmentId { get; set; }
        [Column("X_CurDetailsNo")]
        [StringLength(50)]
        public string XCurDetailsNo { get; set; }
    }
}
