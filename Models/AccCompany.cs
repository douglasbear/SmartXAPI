using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_Company")]
    public partial class AccCompany
    {
        public AccCompany()
        {
            AccBranchMaster = new HashSet<AccBranchMaster>();
            AccFnYear = new HashSet<AccFnYear>();
            AccImportBank = new HashSet<AccImportBank>();
            AccMastGroup = new HashSet<AccMastGroup>();
            AccReconciliation = new HashSet<AccReconciliation>();
            AccVoucherMaster = new HashSet<AccVoucherMaster>();
            InvDeliveryNote = new HashSet<InvDeliveryNote>();
            InvMeetingTrackerCategory = new HashSet<InvMeetingTrackerCategory>();
            InvReceivableStock = new HashSet<InvReceivableStock>();
            InvSales = new HashSet<InvSales>();
            InvTransferStock = new HashSet<InvTransferStock>();
            InvWarehouseMaster = new HashSet<InvWarehouseMaster>();
            PayEmpAccruls = new HashSet<PayEmpAccruls>();
            PayEmployee = new HashSet<PayEmployee>();
            PayEmployeePayment = new HashSet<PayEmployeePayment>();
            PayVillaMaster = new HashSet<PayVillaMaster>();
            PrjProjectTransfer = new HashSet<PrjProjectTransfer>();
            SecUser = new HashSet<SecUser>();
            SecUserCategory = new HashSet<SecUserCategory>();
        }

        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_CompanyName")]
        [StringLength(250)]
        public string XCompanyName { get; set; }
        [Column("X_ShortName")]
        [StringLength(100)]
        public string XShortName { get; set; }
        [Column("X_ContactPerson")]
        [StringLength(250)]
        public string XContactPerson { get; set; }
        [Column("X_EmailID")]
        [StringLength(250)]
        public string XEmailId { get; set; }
        [Column("X_Address")]
        [StringLength(500)]
        public string XAddress { get; set; }
        [Column("X_Website")]
        [StringLength(100)]
        public string XWebsite { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("X_ZipCode")]
        [StringLength(25)]
        public string XZipCode { get; set; }
        [Column("X_Phone1")]
        [StringLength(50)]
        public string XPhone1 { get; set; }
        [Column("X_Phone2")]
        [StringLength(50)]
        public string XPhone2 { get; set; }
        [Column("X_CompanyCode")]
        [StringLength(50)]
        public string XCompanyCode { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("I_Logo", TypeName = "image")]
        public byte[] ILogo { get; set; }
        [Column("X_FaxNo")]
        [StringLength(50)]
        public string XFaxNo { get; set; }
        [Column("X_Slogan")]
        [StringLength(500)]
        public string XSlogan { get; set; }
        [Column("X_History")]
        [StringLength(500)]
        public string XHistory { get; set; }
        [Column("X_Certifications")]
        [StringLength(250)]
        public string XCertifications { get; set; }
        [Column("X_OperatingSince")]
        [StringLength(50)]
        public string XOperatingSince { get; set; }
        [Column("X_CompanyName_Ar")]
        [StringLength(250)]
        public string XCompanyNameAr { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("X_Currency")]
        [StringLength(30)]
        public string XCurrency { get; set; }
        [Column("N_CountryID")]
        public int? NCountryId { get; set; }
        [Column("N_Country")]
        public int? NCountry { get; set; }
        [Column("X_TaxRegistrationNo")]
        [StringLength(50)]
        public string XTaxRegistrationNo { get; set; }
        [Column("X_TaxRegistrationName")]
        [StringLength(50)]
        public string XTaxRegistrationName { get; set; }
        [Column("X_ShippingName")]
        [StringLength(200)]
        public string XShippingName { get; set; }
        [Column("X_ShippingStreet")]
        [StringLength(200)]
        public string XShippingStreet { get; set; }
        [Column("X_ShippingCity")]
        [StringLength(200)]
        public string XShippingCity { get; set; }
        [Column("X_ShippingAppartment")]
        [StringLength(200)]
        public string XShippingAppartment { get; set; }
        [Column("X_ShippingState")]
        [StringLength(200)]
        public string XShippingState { get; set; }
        [Column("X_ShippingZIP")]
        [StringLength(200)]
        public string XShippingZip { get; set; }
        [Column("X_ShippingPhone")]
        [StringLength(200)]
        public string XShippingPhone { get; set; }
        [Column("I_Header", TypeName = "image")]
        public byte[] IHeader { get; set; }
        [Column("I_Footer", TypeName = "image")]
        public byte[] IFooter { get; set; }
        [Column("X_ColorCode")]
        [StringLength(50)]
        public string XColorCode { get; set; }

        [InverseProperty("NCompany")]
        public virtual ICollection<AccBranchMaster> AccBranchMaster { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<AccFnYear> AccFnYear { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<AccImportBank> AccImportBank { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<AccMastGroup> AccMastGroup { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<AccReconciliation> AccReconciliation { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<AccVoucherMaster> AccVoucherMaster { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<InvDeliveryNote> InvDeliveryNote { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<InvMeetingTrackerCategory> InvMeetingTrackerCategory { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<InvReceivableStock> InvReceivableStock { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<InvSales> InvSales { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<InvTransferStock> InvTransferStock { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<InvWarehouseMaster> InvWarehouseMaster { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<PayEmpAccruls> PayEmpAccruls { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<PayEmployee> PayEmployee { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<PayEmployeePayment> PayEmployeePayment { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<PayVillaMaster> PayVillaMaster { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<PrjProjectTransfer> PrjProjectTransfer { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<SecUser> SecUser { get; set; }
        [InverseProperty("NCompany")]
        public virtual ICollection<SecUserCategory> SecUserCategory { get; set; }
    }
}
