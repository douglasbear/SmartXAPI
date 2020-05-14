using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseFreightTax
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PurchaseID")]
        public int? NPurchaseId { get; set; }
        [Column("N_TaxAmt1", TypeName = "money")]
        public decimal? NTaxAmt1 { get; set; }
    }
}
