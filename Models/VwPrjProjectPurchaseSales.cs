using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjProjectPurchaseSales
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("N_VendorID")]
        public int NVendorId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_Invoiceno")]
        [StringLength(50)]
        public string XInvoiceno { get; set; }
        [Column("N_InvoiceAmt", TypeName = "money")]
        public decimal? NInvoiceAmt { get; set; }
        [Column("N_PurchaseType")]
        public int? NPurchaseType { get; set; }
        [Column("N_PurchaseRefID")]
        public int? NPurchaseRefId { get; set; }
        [Required]
        [Column("X_ClientVendor")]
        [StringLength(6)]
        public string XClientVendor { get; set; }
    }
}
