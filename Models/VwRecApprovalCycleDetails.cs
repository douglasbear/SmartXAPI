using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRecApprovalCycleDetails
    {
        [Column("N_ApprovalID")]
        public int? NApprovalId { get; set; }
        [Column("N_ApprovalDetailsID")]
        public int? NApprovalDetailsId { get; set; }
        [Column("X_OrderNo")]
        [StringLength(200)]
        public string XOrderNo { get; set; }
        [Column("X_StatusName")]
        [StringLength(50)]
        public string XStatusName { get; set; }
        [Column("N_StatusId")]
        public int NStatusId { get; set; }
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
    }
}
