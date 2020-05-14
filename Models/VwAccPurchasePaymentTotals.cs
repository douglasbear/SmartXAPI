using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccPurchasePaymentTotals
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AccType")]
        public int? NAccType { get; set; }
        [Column("N_AccID")]
        public int? NAccId { get; set; }
        [Column("N_InventoryID")]
        public int? NInventoryId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
