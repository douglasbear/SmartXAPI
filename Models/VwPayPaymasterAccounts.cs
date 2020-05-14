using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayPaymasterAccounts
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_PayID")]
        public int NPayId { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_ExpenseDefAccountID")]
        public int? NExpenseDefAccountId { get; set; }
        [Column("X_Expense_LedgerCode")]
        [StringLength(50)]
        public string XExpenseLedgerCode { get; set; }
        [Column("X_LedgerExpenseName")]
        [StringLength(100)]
        public string XLedgerExpenseName { get; set; }
        [Column("N_Payable_LedgerID")]
        public int? NPayableLedgerId { get; set; }
        [Column("X_Payable_LedgerCode")]
        [StringLength(50)]
        public string XPayableLedgerCode { get; set; }
        [Column("X_LedgerPayableName")]
        [StringLength(100)]
        public string XLedgerPayableName { get; set; }
        [Column("X_Cr_MappingLevel")]
        [StringLength(10)]
        public string XCrMappingLevel { get; set; }
        [Column("X_Dr_MappingLevel")]
        [StringLength(10)]
        public string XDrMappingLevel { get; set; }
    }
}
