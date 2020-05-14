using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_Sales")]
    public partial class SchSales
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(100)]
        public string XInvoiceNo { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_SalesAmt", TypeName = "money")]
        public decimal? NSalesAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("B_IsRemoved")]
        public int? BIsRemoved { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_RefId")]
        public int? NRefId { get; set; }
        [Column("B_BeginningBalance")]
        public bool? BBeginningBalance { get; set; }
        [Column("B_IsDiscount")]
        public bool? BIsDiscount { get; set; }
        [Column("N_RefFormID")]
        public int? NRefFormId { get; set; }
        [Column("X_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }

        [ForeignKey("NCompanyId,NLedgerId,NFnYearId")]
        public virtual AccMastLedger N { get; set; }
    }
}
