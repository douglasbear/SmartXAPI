using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseReturnPayments
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_PurchaseId")]
        public int? NPurchaseId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("X_ReferenseNo")]
        [StringLength(69)]
        public string XReferenseNo { get; set; }
        [Column("N_PurchaseAmount", TypeName = "money")]
        public decimal? NPurchaseAmount { get; set; }
        [Column("N_ReturnAmount", TypeName = "money")]
        public decimal? NReturnAmount { get; set; }
        [Column("N_TotalReturnAmt", TypeName = "money")]
        public decimal? NTotalReturnAmt { get; set; }
        [Column("N_PaidAmount", TypeName = "money")]
        public decimal? NPaidAmount { get; set; }
        [Column("N_BalanceAmount", TypeName = "money")]
        public decimal? NBalanceAmount { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_VendorPayment", TypeName = "money")]
        public decimal? NVendorPayment { get; set; }
        [Column("X_TransType")]
        [StringLength(25)]
        public string XTransType { get; set; }
        [Column("N_PurchaseAmtC", TypeName = "money")]
        public decimal? NPurchaseAmtC { get; set; }
        [Column("N_PaidAmntC", TypeName = "money")]
        public decimal? NPaidAmntC { get; set; }
        [Column("N_BalanceAmntC", TypeName = "money")]
        public decimal? NBalanceAmntC { get; set; }
        [Column("N_ReturnAmountC", TypeName = "money")]
        public decimal? NReturnAmountC { get; set; }
    }
}
