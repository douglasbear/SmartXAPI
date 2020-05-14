using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRfqVendorSearch
    {
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Amount")]
        [StringLength(30)]
        public string NAmount { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_QuotationID")]
        public int? NQuotationId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_RequestID")]
        public int? NRequestId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Required]
        [Column("x_source")]
        [StringLength(8)]
        public string XSource { get; set; }
    }
}
