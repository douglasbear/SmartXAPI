using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Log_Transactions")]
    public partial class LogTransactions
    {
        [Column("N_ActionID")]
        public int NActionId { get; set; }
        [Column("X_Action")]
        [StringLength(100)]
        public string XAction { get; set; }
        [Column("D_ActionDate", TypeName = "datetime")]
        public DateTime? DActionDate { get; set; }
        [Column("N_ActionUserID")]
        public int? NActionUserId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AccTransID")]
        public int? NAccTransId { get; set; }
        [Column("N_LineNo")]
        public int? NLineNo { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PeriodID")]
        public int? NPeriodId { get; set; }
        [Column("X_EntryForm")]
        [StringLength(50)]
        public string XEntryForm { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_AccType")]
        public int? NAccType { get; set; }
        [Column("N_AccID")]
        public int? NAccId { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(100)]
        public string XChequeNo { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("X_Bank")]
        [StringLength(100)]
        public string XBank { get; set; }
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
        [Column("X_ProjectName")]
        [StringLength(100)]
        public string XProjectName { get; set; }
        [Column("N_InventoryID")]
        public int? NInventoryId { get; set; }
        [Column("X_ErrorMessage")]
        public string XErrorMessage { get; set; }
        [Column("X_SystemName")]
        [StringLength(100)]
        public string XSystemName { get; set; }
        [Column("N_AgainstLedgerID")]
        public int? NAgainstLedgerId { get; set; }
    }
}
