using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFreightEnquiryDashBoard
    {
        [Column("X_QuotationNo")]
        [StringLength(50)]
        public string XQuotationNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("D_QuotationDate", TypeName = "smalldatetime")]
        public DateTime? DQuotationDate { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_QuotationId")]
        public int NQuotationId { get; set; }
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("B_IsEnquiry")]
        public bool? BIsEnquiry { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(6)]
        public string XStatus { get; set; }
    }
}
