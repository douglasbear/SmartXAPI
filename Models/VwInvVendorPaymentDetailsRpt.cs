using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvVendorPaymentDetailsRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Required]
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("X_BranchCode")]
        [StringLength(50)]
        public string XBranchCode { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_PhoneNo1")]
        [StringLength(20)]
        public string XPhoneNo1 { get; set; }
        [Column("X_PhoneNo2")]
        [StringLength(20)]
        public string XPhoneNo2 { get; set; }
        [Column("X_ZipCode")]
        [StringLength(25)]
        public string XZipCode { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("X_ContactName")]
        [StringLength(100)]
        public string XContactName { get; set; }
        [Column("X_CurrencyCode")]
        [StringLength(50)]
        public string XCurrencyCode { get; set; }
        [Column("X_CurrencyName")]
        [StringLength(50)]
        public string XCurrencyName { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_BeneficiaryID")]
        public int? NBeneficiaryId { get; set; }
        [Column("X_BeneficiaryName")]
        [StringLength(100)]
        public string XBeneficiaryName { get; set; }
        [Column("X_BeneficiaryAddress")]
        public string XBeneficiaryAddress { get; set; }
        [Column("X_BeneficiaryPhone")]
        [StringLength(20)]
        public string XBeneficiaryPhone { get; set; }
        [Column("X_BeneficiaryBank")]
        [StringLength(50)]
        public string XBeneficiaryBank { get; set; }
        [Column("X_BeneficiaryBranch")]
        [StringLength(100)]
        public string XBeneficiaryBranch { get; set; }
        [Column("X_BeneficiaryAccount")]
        [StringLength(50)]
        public string XBeneficiaryAccount { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("X_Password")]
        [StringLength(50)]
        public string XPassword { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_AmtPaidFromAdvance", TypeName = "money")]
        public decimal? NAmtPaidFromAdvance { get; set; }
        [Column("N_ExchangeRate", TypeName = "money")]
        public decimal? NExchangeRate { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal? NDiscountAmtF { get; set; }
        [Column("N_AmountF", TypeName = "money")]
        public decimal? NAmountF { get; set; }
        [Column("N_AmtPaidFromAdvanceF", TypeName = "money")]
        public decimal? NAmtPaidFromAdvanceF { get; set; }
        [Column("N_InventoryId")]
        public int NInventoryId { get; set; }
        [Column("N_PayReceiptId")]
        public int NPayReceiptId { get; set; }
        [Column("N_PayReceiptDetailsId")]
        public int NPayReceiptDetailsId { get; set; }
    }
}
