using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaFileCancel
    {
        [StringLength(50)]
        public string Code { get; set; }
        [Column("N_CancelId")]
        public int NCancelId { get; set; }
        [Column("D_CancelDate", TypeName = "smalldatetime")]
        public DateTime? DCancelDate { get; set; }
        [Column("N_FileId")]
        public int? NFileId { get; set; }
        [Column("N_RefundlAmt", TypeName = "money")]
        public decimal? NRefundlAmt { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("X_mobileNo")]
        [StringLength(20)]
        public string XMobileNo { get; set; }
        [Column("X_Email")]
        [StringLength(30)]
        public string XEmail { get; set; }
        [Column("AMount", TypeName = "money")]
        public decimal? Amount { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
    }
}
