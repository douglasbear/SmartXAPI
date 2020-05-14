using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_SalesQuotation")]
    public partial class InvSalesQuotation
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_QuotationId")]
        public int NQuotationId { get; set; }
        [Column("X_QuotationNo")]
        [StringLength(50)]
        public string XQuotationNo { get; set; }
        [Column("D_QuotationDate", TypeName = "smalldatetime")]
        public DateTime? DQuotationDate { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_BillAmt", TypeName = "money")]
        public decimal? NBillAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("N_CashReceived", TypeName = "money")]
        public decimal? NCashReceived { get; set; }
        [Column("x_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_SalesmanID")]
        public int? NSalesmanId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_CRMID")]
        public int? NCrmid { get; set; }
        [Column("B_Revised")]
        public bool? BRevised { get; set; }
        [Column("X_ActualQuotationNo")]
        [StringLength(100)]
        public string XActualQuotationNo { get; set; }
        [Column("X_RfqRefNo")]
        [StringLength(100)]
        public string XRfqRefNo { get; set; }
        [Column("N_FreightAmtF", TypeName = "money")]
        public decimal? NFreightAmtF { get; set; }
        [Column("N_ValidDays")]
        public int? NValidDays { get; set; }
        [Column("D_QuotationExpiry", TypeName = "date")]
        public DateTime? DQuotationExpiry { get; set; }
        [Column("D_RfqRefDate", TypeName = "smalldatetime")]
        public DateTime? DRfqRefDate { get; set; }
        [Column("X_TandC")]
        [StringLength(2500)]
        public string XTandC { get; set; }
        [Column("X_RequestedBy")]
        [StringLength(100)]
        public string XRequestedBy { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("X_Subject")]
        [StringLength(300)]
        public string XSubject { get; set; }
        [Column("N_OthTaxAmt", TypeName = "money")]
        public decimal? NOthTaxAmt { get; set; }
        [Column("N_OthTaxCategoryID")]
        public int? NOthTaxCategoryId { get; set; }
        [Column("N_OthTaxPercentage", TypeName = "money")]
        public decimal? NOthTaxPercentage { get; set; }
    }
}
