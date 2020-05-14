using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCustomerDetailsInPrs
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_ContactName")]
        [StringLength(100)]
        public string XContactName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_ZipCode")]
        [StringLength(25)]
        public string XZipCode { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("X_PhoneNo1")]
        [StringLength(20)]
        public string XPhoneNo1 { get; set; }
        [Column("X_PhoneNo2")]
        [StringLength(20)]
        public string XPhoneNo2 { get; set; }
        [Column("X_FaxNo")]
        [StringLength(100)]
        public string XFaxNo { get; set; }
        [Column("X_Email")]
        [StringLength(30)]
        public string XEmail { get; set; }
        [Column("X_WebSite")]
        [StringLength(50)]
        public string XWebSite { get; set; }
        [Column("N_CreditLimit", TypeName = "money")]
        public decimal? NCreditLimit { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_InvDueDays")]
        public int? NInvDueDays { get; set; }
        [Column("N_DefaultSalesManPerc")]
        public double? NDefaultSalesManPerc { get; set; }
        [Column("N_DefaultSalesManID")]
        public int? NDefaultSalesManId { get; set; }
        [Column("X_IBANNo")]
        [StringLength(50)]
        public string XIbanno { get; set; }
        [Column("D_IqamaExpiry")]
        [StringLength(25)]
        public string DIqamaExpiry { get; set; }
        [Column("X_IqamaIssue")]
        [StringLength(25)]
        public string XIqamaIssue { get; set; }
        [Column("X_IqamaNo")]
        [StringLength(25)]
        public string XIqamaNo { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime? DDob { get; set; }
        [Column("D_Anniversary", TypeName = "datetime")]
        public DateTime? DAnniversary { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("B_DirPosting")]
        public bool? BDirPosting { get; set; }
        [Column("N_ServiceCharge")]
        public double? NServiceCharge { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_EnablePopup")]
        public bool? NEnablePopup { get; set; }
        [Column("X_ReminderMsg")]
        [StringLength(300)]
        public string XReminderMsg { get; set; }
        [Column("X_CustomerName_Ar")]
        [StringLength(250)]
        public string XCustomerNameAr { get; set; }
        [Column("N_PRSID")]
        public int NPrsid { get; set; }
    }
}
