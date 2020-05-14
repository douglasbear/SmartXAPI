using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPrepaymentDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnyearId")]
        public int? NFnyearId { get; set; }
        [Column("N_PrepaymentID")]
        public int? NPrepaymentId { get; set; }
        [Column("X_PaymentNo")]
        [StringLength(100)]
        public string XPaymentNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("X_Startmonth")]
        [StringLength(50)]
        public string XStartmonth { get; set; }
        [Column("N_Duration")]
        public int? NDuration { get; set; }
        [Column("N_Frequency")]
        public int? NFrequency { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("B_IsSavedraft")]
        public bool? BIsSavedraft { get; set; }
        [Column("X_Notes")]
        [StringLength(500)]
        public string XNotes { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(100)]
        public string XCategoryCode { get; set; }
        [Column("X_CategoryName")]
        [StringLength(100)]
        public string XCategoryName { get; set; }
        [Column("X_CategoryPrefix")]
        [StringLength(20)]
        public string XCategoryPrefix { get; set; }
        [Column("N_AssetLedgerID")]
        public int NAssetLedgerId { get; set; }
        [Column("N_ExpenseLedgerID")]
        public int NExpenseLedgerId { get; set; }
    }
}
