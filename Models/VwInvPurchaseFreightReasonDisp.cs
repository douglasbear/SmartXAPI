using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseFreightReasonDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        public int Code { get; set; }
        [Column("X_Reason")]
        [StringLength(50)]
        public string XReason { get; set; }
        [StringLength(100)]
        public string ReasonCode { get; set; }
    }
}
