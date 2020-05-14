using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Rec_Approval")]
    public partial class RecApproval
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Column("X_ApprovalCode")]
        [StringLength(50)]
        public string XApprovalCode { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
