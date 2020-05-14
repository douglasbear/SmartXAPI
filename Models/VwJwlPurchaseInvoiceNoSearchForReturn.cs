using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwJwlPurchaseInvoiceNoSearchForReturn
    {
        [Column("Vendor Code")]
        [StringLength(50)]
        public string VendorCode { get; set; }
        [StringLength(100)]
        public string Vendor { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("Invoice No")]
        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime? DInvoiceDate { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("Invoice Date")]
        [StringLength(8000)]
        public string InvoiceDate { get; set; }
        [Column("N_PurchaseRefID")]
        public int? NPurchaseRefId { get; set; }
        [Column("N_PurchaseType")]
        public int NPurchaseType { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
    }
}
