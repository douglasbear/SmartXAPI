using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_StatusApproval")]
    public partial class GenStatusApproval
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Column("X_UserID")]
        [StringLength(100)]
        public string XUserId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
    }
}
