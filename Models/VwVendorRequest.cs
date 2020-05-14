using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVendorRequest
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_QuotationId")]
        public int NQuotationId { get; set; }
        [Column("X_QuotationNo")]
        [StringLength(50)]
        public string XQuotationNo { get; set; }
        [Column("D_QuotationDate")]
        [StringLength(8000)]
        public string DQuotationDate { get; set; }
    }
}
