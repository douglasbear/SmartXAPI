using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseReturnSub
    {
        public double? RetQty { get; set; }
        [Column("N_PurchaseId")]
        public int? NPurchaseId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_PurchaseDetailsId")]
        public int? NPurchaseDetailsId { get; set; }
        [Column("X_CreditNoteNo")]
        [StringLength(50)]
        public string XCreditNoteNo { get; set; }
    }
}
