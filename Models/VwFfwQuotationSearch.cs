using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwQuotationSearch
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_QuotationId")]
        public int NQuotationId { get; set; }
        [Column("X_QuotationNo")]
        [StringLength(50)]
        public string XQuotationNo { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Required]
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("D_QuotationDate")]
        [StringLength(8000)]
        public string DQuotationDate { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("B_IsEnquiry")]
        public bool BIsEnquiry { get; set; }
    }
}
