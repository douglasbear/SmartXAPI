using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_Shipper")]
    public partial class FfwShipper
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ShipperID")]
        public int NShipperId { get; set; }
        [Column("X_ShipperCode")]
        [StringLength(50)]
        public string XShipperCode { get; set; }
        [Column("X_ShipperName")]
        [StringLength(100)]
        public string XShipperName { get; set; }
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
        [StringLength(50)]
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
    }
}
