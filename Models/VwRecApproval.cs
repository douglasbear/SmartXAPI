using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRecApproval
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Column("N_ApprovalDetailsID")]
        public int NApprovalDetailsId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("X_UserID")]
        [StringLength(100)]
        public string XUserId { get; set; }
        [Column("X_ApprovalCode")]
        [StringLength(50)]
        public string XApprovalCode { get; set; }
        [Column("X_UserName")]
        [StringLength(200)]
        public string XUserName { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
    }
}
