using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccVoucherMasterDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_VoucherID")]
        public int NVoucherId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_PeriodID")]
        public int? NPeriodId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("D_VoucherDate", TypeName = "datetime")]
        public DateTime? DVoucherDate { get; set; }
        [Column("X_EntryFrom")]
        [StringLength(100)]
        public string XEntryFrom { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("N_DefLedgerID")]
        public int? NDefLedgerId { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(50)]
        public string XChequeNo { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("X_BankName")]
        [StringLength(50)]
        public string XBankName { get; set; }
        [Column("X_DefLedgerType")]
        [StringLength(50)]
        public string XDefLedgerType { get; set; }
        [Column("B_IsAccPosted")]
        public bool? BIsAccPosted { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_PaymentMethodID")]
        public int? NPaymentMethodId { get; set; }
        public int? Expr1 { get; set; }
        [Column("N_GroupID")]
        public int? NGroupId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_Reserved")]
        public int? NReserved { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        public int? Expr2 { get; set; }
        [Column("X_Level")]
        [StringLength(100)]
        public string XLevel { get; set; }
        [Column("X_CashTypeBehaviour")]
        [StringLength(50)]
        public string XCashTypeBehaviour { get; set; }
        [Column("X_LedgerName_Ar")]
        [StringLength(50)]
        public string XLedgerNameAr { get; set; }
        [Column("B_CostCenterEnabled")]
        public bool? BCostCenterEnabled { get; set; }
        public int? Expr3 { get; set; }
        [Column("N_CashBahavID")]
        public int? NCashBahavId { get; set; }
        [Column("N_TransBehavID")]
        public int? NTransBehavId { get; set; }
        [Column("N_LedgerBehavID")]
        public int? NLedgerBehavId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Expr4 { get; set; }
        [Column("X_Form")]
        [StringLength(100)]
        public string XForm { get; set; }
        [Column("N_PostingBahavID")]
        public int? NPostingBahavId { get; set; }
        [Column("X_PayMethod")]
        [StringLength(50)]
        public string XPayMethod { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("B_IsCheque")]
        public bool? BIsCheque { get; set; }
    }
}
