using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProcessedRfq
    {
        [Column("X_QuotationNo")]
        [StringLength(50)]
        public string XQuotationNo { get; set; }
        [Column("N_QuotationId")]
        public int NQuotationId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
    }
}
