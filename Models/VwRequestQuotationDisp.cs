using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRequestQuotationDisp
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_RequestId")]
        public int NRequestId { get; set; }
        [Column("X_RequestNo")]
        [StringLength(50)]
        public string XRequestNo { get; set; }
        [Column("N_QuotationID")]
        public int? NQuotationId { get; set; }
        [Column("D_RequestDate")]
        [StringLength(8000)]
        public string DRequestDate { get; set; }
        [Column("N_VendorId")]
        public int? NVendorId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Required]
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
    }
}
