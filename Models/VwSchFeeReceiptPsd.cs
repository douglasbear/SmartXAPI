using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchFeeReceiptPsd
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("B_Issued")]
        public bool BIssued { get; set; }
        [Column("B_Paid")]
        public bool? BPaid { get; set; }
        [Column("N_FrequencyID")]
        public int? NFrequencyId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(100)]
        public string XInvoiceNo { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_InvoiceDueAmt", TypeName = "money")]
        public decimal? NInvoiceDueAmt { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal? NDiscount { get; set; }
        [Column("N_ReceiptID")]
        public int? NReceiptId { get; set; }
        [Column("B_IsRemoved")]
        public int? BIsRemoved { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("D_ReceiptDate", TypeName = "datetime")]
        public DateTime? DReceiptDate { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_FeeCategoryCode")]
        [StringLength(50)]
        public string XFeeCategoryCode { get; set; }
    }
}
