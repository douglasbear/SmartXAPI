using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchFeeReceiptDisp
    {
        [Column("X_Remarks")]
        [StringLength(150)]
        public string XRemarks { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_ReceiptID")]
        public int NReceiptId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("D_ReceiptDate", TypeName = "datetime")]
        public DateTime? DReceiptDate { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("X_Name")]
        [StringLength(200)]
        public string XName { get; set; }
        [Column("N_Inactive")]
        public int? NInactive { get; set; }
        [Column("N_DefLedgerID")]
        public int? NDefLedgerId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_Paymentmethod")]
        [StringLength(50)]
        public string XPaymentmethod { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(25)]
        public string XChequeNo { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("X_PayMethod")]
        [StringLength(50)]
        public string XPayMethod { get; set; }
        [Column("B_IsCheque")]
        public bool? BIsCheque { get; set; }
        [Column("N_PaymentMethodID")]
        public int? NPaymentMethodId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_TaxCategoryID")]
        public int? NTaxCategoryId { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal? NDiscount { get; set; }
    }
}
