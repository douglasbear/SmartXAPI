﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvCustomerStatementStatus
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Remarks")]
        [StringLength(8000)]
        public string XRemarks { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_PeriodID")]
        public int NPeriodId { get; set; }
        [Column("X_EntryForm")]
        [StringLength(50)]
        public string XEntryForm { get; set; }
        [Column("N_AccType")]
        public int NAccType { get; set; }
        [Column("N_AccID")]
        public int? NAccId { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Required]
        [Column("X_ChequeNo")]
        [StringLength(1)]
        public string XChequeNo { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Required]
        [Column("X_Bank")]
        [StringLength(1)]
        public string XBank { get; set; }
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Required]
        [Column("X_ProjectName")]
        [StringLength(1)]
        public string XProjectName { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_InventoryID")]
        public int? NInventoryId { get; set; }
        [Column("N_VoucherDetailsID")]
        public int NVoucherDetailsId { get; set; }
        [Column("N_VoucherID")]
        public int NVoucherId { get; set; }
        [Column("N_AgainstLedgerID")]
        public int? NAgainstLedgerId { get; set; }
    }
}
