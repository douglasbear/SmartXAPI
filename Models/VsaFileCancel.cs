using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("vsa_FileCancel")]
    public partial class VsaFileCancel
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_CancelId")]
        public int NCancelId { get; set; }
        [Column("X_CancelNo")]
        [StringLength(50)]
        public string XCancelNo { get; set; }
        [Column("D_CancelDate", TypeName = "smalldatetime")]
        public DateTime? DCancelDate { get; set; }
        [Column("N_FileId")]
        public int? NFileId { get; set; }
        [Column("N_RefundlAmt", TypeName = "money")]
        public decimal? NRefundlAmt { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
    }
}
