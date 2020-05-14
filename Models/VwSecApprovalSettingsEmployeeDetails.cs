using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSecApprovalSettingsEmployeeDetails
    {
        [Required]
        [Column("X_ApprovalCode")]
        [StringLength(50)]
        public string XApprovalCode { get; set; }
        [Required]
        [Column("X_ApprovalDescription")]
        [StringLength(100)]
        public string XApprovalDescription { get; set; }
        [Column("N_ApprovalSettingsID")]
        public int NApprovalSettingsId { get; set; }
        [Column("X_ActionDesc")]
        [StringLength(50)]
        public string XActionDesc { get; set; }
        [Column("N_ActionID")]
        public int NActionId { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
    }
}
