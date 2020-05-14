using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvLastsavedStatus
    {
        [Column("D_RefDate")]
        [StringLength(8000)]
        public string DRefDate { get; set; }
        [Column("X_StatusName")]
        [StringLength(500)]
        public string XStatusName { get; set; }
        [Column("N_PaymentID")]
        public int? NPaymentId { get; set; }
    }
}
