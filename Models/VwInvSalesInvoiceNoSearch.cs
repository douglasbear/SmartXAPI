using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesInvoiceNoSearch
    {
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("Invoice No")]
        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [Column("Invoice Date")]
        [StringLength(8000)]
        public string InvoiceDate { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("Customer Code")]
        [StringLength(50)]
        public string CustomerCode { get; set; }
        [StringLength(100)]
        public string Customer { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("N_SalesType")]
        public int? NSalesType { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Required]
        [Column("x_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_BillAmt")]
        [StringLength(30)]
        public string XBillAmt { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Required]
        [Column("type")]
        [StringLength(5)]
        public string Type { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Required]
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Required]
        [Column("X_CustPONo")]
        [StringLength(50)]
        public string XCustPono { get; set; }
        [Required]
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("N_SalesOrderId")]
        public int? NSalesOrderId { get; set; }
        [Required]
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Column("D_EntryTime")]
        [StringLength(8)]
        public string DEntryTime { get; set; }
        [Column("N_Hold")]
        public int NHold { get; set; }
        [Column("X_PhoneNo1")]
        [StringLength(20)]
        public string XPhoneNo1 { get; set; }
        [Column("B_POS")]
        public bool BPos { get; set; }
    }
}
