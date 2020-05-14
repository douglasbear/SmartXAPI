using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaPaymentdetailsDefault
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_ItemCost", TypeName = "money")]
        public decimal? NItemCost { get; set; }
        [Column("date")]
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("X_CategoryName")]
        [StringLength(100)]
        public string XCategoryName { get; set; }
        public int? TaxCatId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_DisplayName")]
        [StringLength(100)]
        public string XDisplayName { get; set; }
        [Column(TypeName = "money")]
        public decimal? TaxPerc { get; set; }
    }
}
