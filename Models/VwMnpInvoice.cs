using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMnpInvoice
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Required]
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime DInvoiceDate { get; set; }
        [Column("X_Month")]
        [StringLength(50)]
        public string XMonth { get; set; }
        [Column("X_Year")]
        [StringLength(50)]
        public string XYear { get; set; }
        [Column("N_PayRun")]
        public int? NPayRun { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("D_InvoiceFromDate", TypeName = "datetime")]
        public DateTime DInvoiceFromDate { get; set; }
        [Column("D_InvoiceToDate", TypeName = "datetime")]
        public DateTime DInvoiceToDate { get; set; }
        [Column("N_TotalAmount", TypeName = "money")]
        public decimal NTotalAmount { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal NTaxAmount { get; set; }
        [Column("N_NetAmount", TypeName = "money")]
        public decimal NNetAmount { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_ContactName")]
        [StringLength(100)]
        public string XContactName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("X_PhoneNo1")]
        [StringLength(20)]
        public string XPhoneNo1 { get; set; }
        [Column("X_PhoneNo2")]
        [StringLength(20)]
        public string XPhoneNo2 { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("B_DirPosting")]
        public bool? BDirPosting { get; set; }
        [Column("N_EnablePopup")]
        public bool? NEnablePopup { get; set; }
        [Column("N_AllowCashPay")]
        public bool? NAllowCashPay { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
    }
}
