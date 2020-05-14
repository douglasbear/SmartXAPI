using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_Vendor")]
    public partial class InvVendor
    {
        public InvVendor()
        {
            InvPurchase = new HashSet<InvPurchase>();
            InvPurchaseFreights = new HashSet<InvPurchaseFreights>();
            InvPurchaseOrder = new HashSet<InvPurchaseOrder>();
            InvPurchaseReturnMaster = new HashSet<InvPurchaseReturnMaster>();
        }

        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_VendorID")]
        public int NVendorId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
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
        [StringLength(100)]
        public string XWebSite { get; set; }
        [Column("N_CreditLimit", TypeName = "money")]
        public decimal? NCreditLimit { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_InvDueDays")]
        public int? NInvDueDays { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_CountryCode")]
        [StringLength(50)]
        public string XCountryCode { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("B_DirPosting")]
        public bool? BDirPosting { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("X_ReminderMsg")]
        [StringLength(200)]
        public string XReminderMsg { get; set; }
        [Column("X_VendorName_Ar")]
        [StringLength(250)]
        public string XVendorNameAr { get; set; }
        [Column("N_CountryID")]
        public int? NCountryId { get; set; }
        [Column("X_TaxRegistrationNo")]
        [StringLength(50)]
        public string XTaxRegistrationNo { get; set; }
        [Column("N_TaxCategoryID")]
        public int? NTaxCategoryId { get; set; }
        [Column("B_AllowCashPay")]
        public bool? BAllowCashPay { get; set; }
        [Column("N_PartnerTypeID")]
        public int? NPartnerTypeId { get; set; }
        [Column("N_VendorTypeID")]
        public int? NVendorTypeId { get; set; }
        [Column("N_GoodsDeliveryIn")]
        public int? NGoodsDeliveryIn { get; set; }
        [Column("X_TandC")]
        [StringLength(500)]
        public string XTandC { get; set; }
        [Column("X_CRNumber")]
        [StringLength(50)]
        public string XCrnumber { get; set; }

        [ForeignKey(nameof(NCurrencyId))]
        [InverseProperty(nameof(AccCurrencyMaster.InvVendor))]
        public virtual AccCurrencyMaster NCurrency { get; set; }
        [InverseProperty("N")]
        public virtual ICollection<InvPurchase> InvPurchase { get; set; }
        [InverseProperty("N")]
        public virtual ICollection<InvPurchaseFreights> InvPurchaseFreights { get; set; }
        [InverseProperty("N")]
        public virtual ICollection<InvPurchaseOrder> InvPurchaseOrder { get; set; }
        [InverseProperty("N")]
        public virtual ICollection<InvPurchaseReturnMaster> InvPurchaseReturnMaster { get; set; }
    }
}
