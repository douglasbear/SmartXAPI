using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccBankDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BankID")]
        public int NBankId { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [Column("B_Iscompany")]
        public bool? BIscompany { get; set; }
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
        [Column("X_Address")]
        [StringLength(500)]
        public string XAddress { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(500)]
        public string XLedgerName { get; set; }
        [Column("X_AccountHolderName")]
        [StringLength(100)]
        public string XAccountHolderName { get; set; }
        [Column("X_IBAN")]
        [StringLength(100)]
        public string XIban { get; set; }
        [Column("X_CSVTemplatePath")]
        [StringLength(500)]
        public string XCsvtemplatePath { get; set; }
        [Column("N_CsvTemplateID")]
        public int NCsvTemplateId { get; set; }
    }
}
