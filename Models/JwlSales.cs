using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Jwl_Sales")]
    public partial class JwlSales
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
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
        [Column("B_SaveDraft")]
        public bool? BSaveDraft { get; set; }
    }
}
