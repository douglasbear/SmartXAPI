using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwApprovalCycleStatus
    {
        [Required]
        [Column("X_RecruitmentCode")]
        [StringLength(400)]
        public string XRecruitmentCode { get; set; }
        [Column("X_ApprovalName")]
        [StringLength(50)]
        public string XApprovalName { get; set; }
        [Column("N_RecTypeID")]
        public int? NRecTypeId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("X_OrderNo")]
        public int? XOrderNo { get; set; }
        [Column("X_StatusCaption")]
        [StringLength(50)]
        public string XStatusCaption { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
