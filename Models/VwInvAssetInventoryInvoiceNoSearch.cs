using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvAssetInventoryInvoiceNoSearch
    {
        [Column("N_AssetInventoryID")]
        public int NAssetInventoryId { get; set; }
        [Column("Invoice No")]
        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [Column("Invoice Date")]
        [StringLength(8000)]
        public string InvoiceDate { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("Vendor Code")]
        [StringLength(50)]
        public string VendorCode { get; set; }
        [StringLength(100)]
        public string Vendor { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Required]
        [Column("X_VendorInvoice")]
        [StringLength(50)]
        public string XVendorInvoice { get; set; }
        [Required]
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [StringLength(20)]
        public string NetAmount { get; set; }
    }
}
