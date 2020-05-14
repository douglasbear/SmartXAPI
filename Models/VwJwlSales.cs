using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwJwlSales
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_SalesID")]
        public int NSalesId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
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
        [Column("N_ChequeReceived", TypeName = "money")]
        public decimal? NChequeReceived { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(50)]
        public string XChequeNo { get; set; }
        [Column("N_BankID")]
        public int? NBankId { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_QuotationID")]
        public int? NQuotationId { get; set; }
        [Column("N_SalesOrderID")]
        public int? NSalesOrderId { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("N_SalesType")]
        public int? NSalesType { get; set; }
        [Column("N_SalesRefID")]
        public int? NSalesRefId { get; set; }
        [Column("N_SalesmanID")]
        public int? NSalesmanId { get; set; }
        [Column("N_SalesmanAmt", TypeName = "money")]
        public decimal? NSalesmanAmt { get; set; }
        [Column("N_SalesmanPerc")]
        public double? NSalesmanPerc { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_GoldReceived", TypeName = "money")]
        public decimal? NGoldReceived { get; set; }
        [Column("N_CreditCard", TypeName = "money")]
        public decimal? NCreditCard { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
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
        [Column("X_ZipCode")]
        [StringLength(25)]
        public string XZipCode { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("X_PhoneNo1")]
        [StringLength(20)]
        public string XPhoneNo1 { get; set; }
        [Column("X_PhoneNo2")]
        [StringLength(20)]
        public string XPhoneNo2 { get; set; }
        [Column("X_IBANNo")]
        [StringLength(50)]
        public string XIbanno { get; set; }
        [Column("X_IqamaIssue")]
        [StringLength(25)]
        public string XIqamaIssue { get; set; }
        [Column("X_IqamaNo")]
        [StringLength(25)]
        public string XIqamaNo { get; set; }
        [Column("X_FaxNo")]
        [StringLength(100)]
        public string XFaxNo { get; set; }
        [Column("X_Email")]
        [StringLength(30)]
        public string XEmail { get; set; }
        [Column("X_WebSite")]
        [StringLength(50)]
        public string XWebSite { get; set; }
        [Column("Invoice Date")]
        [StringLength(8000)]
        public string InvoiceDate { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime? DDob { get; set; }
        [Column("D_Anniversary", TypeName = "datetime")]
        public DateTime? DAnniversary { get; set; }
        [Column("B_SaveDraft")]
        public bool? BSaveDraft { get; set; }
        [Column("CustomerTypeID")]
        public int? CustomerTypeId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("B_DirPosting")]
        public bool? BDirPosting { get; set; }
    }
}
