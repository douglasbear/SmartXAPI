using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayReceiptMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_PayReceiptId")]
        public int NPayReceiptId { get; set; }
        [Required]
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Required]
        [Column("X_Type")]
        [StringLength(10)]
        public string XType { get; set; }
        [Column("N_DefLedgerID")]
        public int? NDefLedgerId { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(50)]
        public string XChequeNo { get; set; }
        [Column("X_PaymentMethod")]
        [StringLength(50)]
        public string XPaymentMethod { get; set; }
        [Column("X_ProjectName")]
        [StringLength(100)]
        public string XProjectName { get; set; }
        [Column("X_Description")]
        [StringLength(1000)]
        public string XDescription { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("X_BranchCode")]
        [StringLength(50)]
        public string XBranchCode { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("B_IsDraft")]
        public bool BIsDraft { get; set; }
    }
}
