using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvVendorPaymentRpt
    {
        [Column("N_PayReceiptId")]
        public int NPayReceiptId { get; set; }
        [Required]
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("X_PaymentMethod")]
        [StringLength(50)]
        public string XPaymentMethod { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(50)]
        public string XChequeNo { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("N_ItemDisc", TypeName = "money")]
        public decimal? NItemDisc { get; set; }
        [StringLength(1000)]
        public string Notes { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_Type")]
        [StringLength(25)]
        public string XType { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("Inv_Date", TypeName = "datetime")]
        public DateTime? InvDate { get; set; }
        [Column("N_BalanceAmount", TypeName = "money")]
        public decimal? NBalanceAmount { get; set; }
        [Column("N_CashPaidF", TypeName = "money")]
        public decimal NCashPaidF { get; set; }
        [Column("N_DiscountAmtF", TypeName = "money")]
        public decimal NDiscountAmtF { get; set; }
        [Column("X_Remarks")]
        [StringLength(50)]
        public string XRemarks { get; set; }
        [Column(TypeName = "money")]
        public decimal? NetAmount { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_AmountCom", TypeName = "money")]
        public decimal? NAmountCom { get; set; }
        [Column("N_BalanceAmountCom", TypeName = "money")]
        public decimal? NBalanceAmountCom { get; set; }
        [Column("N_CashPaid", TypeName = "money")]
        public decimal NCashPaid { get; set; }
        [Column("X_Description")]
        [StringLength(1000)]
        public string XDescription { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_VendorInvoice")]
        [StringLength(50)]
        public string XVendorInvoice { get; set; }
        [Column("N_DefLedgerID")]
        public int? NDefLedgerId { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("N_dueamount", TypeName = "money")]
        public decimal NDueamount { get; set; }
    }
}
