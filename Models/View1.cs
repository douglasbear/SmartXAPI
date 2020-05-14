using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class View1
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AccType")]
        public int? NAccType { get; set; }
        [Column("N_AccID")]
        public int? NAccId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime? DInvoiceDate { get; set; }
        [Column("N_InventoryID")]
        public int? NInventoryId { get; set; }
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("N_PayReceiptId")]
        public int NPayReceiptId { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_PaidAmt", TypeName = "money")]
        public decimal? NPaidAmt { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
    }
}
