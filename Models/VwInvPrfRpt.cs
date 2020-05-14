using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPrfRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_PaymentID")]
        public int? NPaymentId { get; set; }
        [Column("N_BeneficiaryID")]
        public int? NBeneficiaryId { get; set; }
        [Column("X_Requestor")]
        [StringLength(50)]
        public string XRequestor { get; set; }
        [Column("N_PayableAmount", TypeName = "money")]
        public decimal? NPayableAmount { get; set; }
        [Column("N_TotalAmount", TypeName = "money")]
        public decimal? NTotalAmount { get; set; }
        [Column("N_BalanceAmount", TypeName = "money")]
        public decimal? NBalanceAmount { get; set; }
        [Column("X_Documents")]
        [StringLength(500)]
        public string XDocuments { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_BeneficiaryCode")]
        [StringLength(50)]
        public string XBeneficiaryCode { get; set; }
        [Column("X_BeneficiaryName")]
        [StringLength(100)]
        public string XBeneficiaryName { get; set; }
        [Required]
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("X_PaymentMethod")]
        [StringLength(50)]
        public string XPaymentMethod { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(50)]
        public string XChequeNo { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_BeneficiaryBank")]
        [StringLength(50)]
        public string XBeneficiaryBank { get; set; }
        [Column("X_BeneficiaryAccount")]
        [StringLength(50)]
        public string XBeneficiaryAccount { get; set; }
        [Column("X_BeneficiarySwiftCode")]
        [StringLength(50)]
        public string XBeneficiarySwiftCode { get; set; }
        [Column("X_BeneficiaryAddress")]
        public string XBeneficiaryAddress { get; set; }
        [Column("X_BeneficiaryPhone")]
        [StringLength(20)]
        public string XBeneficiaryPhone { get; set; }
        [Column("X_BeneficiaryBranch")]
        [StringLength(100)]
        public string XBeneficiaryBranch { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("X_AmountInWords")]
        [StringLength(200)]
        public string XAmountInWords { get; set; }
        [Column("X_BeneficaryName_Eng")]
        [StringLength(100)]
        public string XBeneficaryNameEng { get; set; }
        [Column("X_IbanNo")]
        [StringLength(50)]
        public string XIbanNo { get; set; }
        [Column("X_ProjectName")]
        [StringLength(100)]
        public string XProjectName { get; set; }
        [Column("X_ShortName")]
        [StringLength(50)]
        public string XShortName { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
    }
}
