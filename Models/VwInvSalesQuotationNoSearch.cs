using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesQuotationNoSearch
    {
        [Column("N_QuotationId")]
        public int NQuotationId { get; set; }
        [Column("Quotation No")]
        [StringLength(50)]
        public string QuotationNo { get; set; }
        [Column("Quotation Date")]
        [StringLength(8000)]
        public string QuotationDate { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Required]
        [Column("Customer Code")]
        [StringLength(50)]
        public string CustomerCode { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_QuotationDate", TypeName = "smalldatetime")]
        public DateTime? DQuotationDate { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("X_RfqRefNo")]
        [StringLength(100)]
        public string XRfqRefNo { get; set; }
        [Column("D_RfqRefDate")]
        [StringLength(8000)]
        public string DRfqRefDate { get; set; }
        [Column("N_Amount")]
        [StringLength(30)]
        public string NAmount { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("N_OthTaxAmt", TypeName = "money")]
        public decimal? NOthTaxAmt { get; set; }
        [Column("N_BillAmt", TypeName = "money")]
        public decimal? NBillAmt { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Required]
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
    }
}
