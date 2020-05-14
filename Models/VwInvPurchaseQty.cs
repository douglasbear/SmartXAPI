using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseQty
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_PurchaseQty")]
        public double? NPurchaseQty { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
    }
}
