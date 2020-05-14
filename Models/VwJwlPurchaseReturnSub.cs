using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwJwlPurchaseReturnSub
    {
        public double? RetQty { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_PurchaseId")]
        public int? NPurchaseId { get; set; }
    }
}
