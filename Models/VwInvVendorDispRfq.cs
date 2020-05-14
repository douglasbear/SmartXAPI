using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvVendorDispRfq
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_VendorID")]
        public int NVendorId { get; set; }
        [Column("Vendor Code")]
        [StringLength(50)]
        public string VendorCode { get; set; }
        [Column("Vendor Name")]
        [StringLength(100)]
        public string VendorName { get; set; }
        [Column("Contact Name")]
        [StringLength(100)]
        public string ContactName { get; set; }
        [StringLength(250)]
        public string Address { get; set; }
        [Column("Zip Code")]
        [StringLength(25)]
        public string ZipCode { get; set; }
        [StringLength(50)]
        public string Country { get; set; }
        [Required]
        [Column("Phone No1")]
        [StringLength(20)]
        public string PhoneNo1 { get; set; }
        [Column("Phone No2")]
        [StringLength(20)]
        public string PhoneNo2 { get; set; }
        [Column("Fax No")]
        [StringLength(100)]
        public string FaxNo { get; set; }
        [Column("E-Mail")]
        [StringLength(100)]
        public string EMail { get; set; }
        [Column("Web site")]
        [StringLength(100)]
        public string WebSite { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_CreditLimit", TypeName = "money")]
        public decimal? NCreditLimit { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("B_DirPosting")]
        public bool? BDirPosting { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_InvDueDays")]
        public int? NInvDueDays { get; set; }
        [Column("X_CurrencyName")]
        [StringLength(50)]
        public string XCurrencyName { get; set; }
        [Column("X_ShortName")]
        [StringLength(50)]
        public string XShortName { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_QuotationID")]
        public int? NQuotationId { get; set; }
        [Column("N_Processed")]
        public int NProcessed { get; set; }
    }
}
