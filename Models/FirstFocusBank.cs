using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__FirstFocus_Bank")]
    public partial class FirstFocusBank
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BankID")]
        public int NBankId { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Column("X_AccType")]
        [StringLength(10)]
        public string XAccType { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_ContactPerson")]
        [StringLength(50)]
        public string XContactPerson { get; set; }
        [Column("X_Street")]
        [StringLength(50)]
        public string XStreet { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(50)]
        public string XPhoneNo { get; set; }
        [Column("X_MobileNo")]
        [StringLength(50)]
        public string XMobileNo { get; set; }
        [Column("X_FaxNo")]
        [StringLength(50)]
        public string XFaxNo { get; set; }
        [Column("X_EmaiID")]
        [StringLength(50)]
        public string XEmaiId { get; set; }
        [Column("X_BankNameLocale")]
        [StringLength(100)]
        public string XBankNameLocale { get; set; }
        [Column("X_BankCode")]
        [StringLength(50)]
        public string XBankCode { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_SwiftNo")]
        [StringLength(50)]
        public string XSwiftNo { get; set; }
        [Column("X_AccountNo")]
        [StringLength(50)]
        public string XAccountNo { get; set; }
        [Column("X_RptPath")]
        [StringLength(500)]
        public string XRptPath { get; set; }
        [Column("B_Iscompany")]
        public bool? BIscompany { get; set; }
        [Column("X_Address")]
        [StringLength(500)]
        public string XAddress { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_AccountHolderName")]
        [StringLength(100)]
        public string XAccountHolderName { get; set; }
        [Column("N_RuleID")]
        public int? NRuleId { get; set; }
    }
}
