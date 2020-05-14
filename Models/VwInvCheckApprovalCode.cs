using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvCheckApprovalCode
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Required]
        [Column("X_ApprovalCode")]
        [StringLength(50)]
        public string XApprovalCode { get; set; }
        [Required]
        [Column("X_ApprovalDescription")]
        [StringLength(100)]
        public string XApprovalDescription { get; set; }
    }
}
