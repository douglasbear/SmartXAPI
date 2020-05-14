using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Rec_ApprovalDetails")]
    public partial class RecApprovalDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Key]
        [Column("N_ApprovalDetailsID")]
        public int NApprovalDetailsId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("X_UserID")]
        [StringLength(100)]
        public string XUserId { get; set; }
        [Column("X_UserName")]
        [StringLength(200)]
        public string XUserName { get; set; }
    }
}
