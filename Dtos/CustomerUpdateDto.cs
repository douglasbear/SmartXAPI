using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Dtos
{
    public partial class CustomerUpdateDto
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
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
        [StringLength(500)]
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
        [StringLength(100)]
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
        [Key]
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
        [Column("N_CountryID")]
        public int? NCountryId { get; set; }
        [Column("X_TaxRegistrationNo")]
        [StringLength(50)]
        public string XTaxRegistrationNo { get; set; }
        [Column("N_AllowCashPay")]
        public bool? NAllowCashPay { get; set; }
        [Column("X_VendorCode")]
        [StringLength(500)]
        public string XVendorCode { get; set; }
        [Column("N_ServiceChargeLimit", TypeName = "money")]
        public decimal? NServiceChargeLimit { get; set; }
        [Column("N_CashTypeID")]
        public int? NCashTypeId { get; set; }
        [Column("B_SalesOrderRequired")]
        public bool? BSalesOrderRequired { get; set; }
        [Column("X_UserName")]
        [StringLength(100)]
        public string XUserName { get; set; }
        [Column("X_Password")]
        [StringLength(100)]
        public string XPassword { get; set; }
        [Column("N_TaxCategoryID")]
        public int? NTaxCategoryId { get; set; }
        [Column("N_TaxPercentage", TypeName = "money")]
        public decimal? NTaxPercentage { get; set; }
        [Column("X_URL")]
        [StringLength(200)]
        public string XUrl { get; set; }
        [Column("N_DiscID")]
        public int? NDiscId { get; set; }
        [Column("X_Address_Ar")]
        [StringLength(150)]
        public string XAddressAr { get; set; }
        [Column("X_TaxRegistrationNo_Ar")]
        public int? XTaxRegistrationNoAr { get; set; }
        [Column("B_EnablePortalLogin")]
        public bool? BEnablePortalLogin { get; set; }
        [Column("X_CRNumber")]
        [StringLength(50)]
        public string XCrnumber { get; set; }

    }
}
