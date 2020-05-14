using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvsalespurchaseRpt
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }
        [Required]
        [StringLength(2)]
        public string Type { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column(TypeName = "money")]
        public decimal? TotalAmt { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
    }
}
