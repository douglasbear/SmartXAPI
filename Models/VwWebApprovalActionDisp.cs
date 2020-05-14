using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwWebApprovalActionDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_Action")]
        public int NAction { get; set; }
        [Column("X_ActionDesc")]
        [StringLength(50)]
        public string XActionDesc { get; set; }
    }
}
