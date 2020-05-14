using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMnpInvoiceMasterDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Column("N_DetailsID")]
        public int NDetailsId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("N_PayFactor")]
        public double? NPayFactor { get; set; }
        [Column("B_Posted")]
        public bool? BPosted { get; set; }
        [Column("D_PostedDate", TypeName = "datetime")]
        public DateTime? DPostedDate { get; set; }
        [Column("N_SalaryPayMethod")]
        public int? NSalaryPayMethod { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("X_Remarks")]
        public string XRemarks { get; set; }
        [Column("D_DateTo", TypeName = "date")]
        public DateTime? DDateTo { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
        [Column("N_IsLoan")]
        public int NIsLoan { get; set; }
        [Column("N_LoanTransID")]
        public int NLoanTransId { get; set; }
        [Column("N_Days")]
        public double NDays { get; set; }
        [Column("N_TaxCategoryID")]
        public int NTaxCategoryId { get; set; }
        [Column("N_TaxPerc", TypeName = "money")]
        public decimal NTaxPerc { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal NTaxAmount { get; set; }
    }
}
