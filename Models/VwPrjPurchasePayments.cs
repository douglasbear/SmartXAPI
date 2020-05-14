using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjPurchasePayments
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("X_ReferenseNo")]
        [StringLength(66)]
        public string XReferenseNo { get; set; }
        [Column("N_PurchaseAmount", TypeName = "money")]
        public decimal? NPurchaseAmount { get; set; }
        [Column("N_ReturnAmount")]
        public int NReturnAmount { get; set; }
        [Column("N_PaidAmount", TypeName = "money")]
        public decimal? NPaidAmount { get; set; }
        [Column("N_BalanceAmount", TypeName = "money")]
        public decimal? NBalanceAmount { get; set; }
        [Column("ProjectID")]
        public int? ProjectId { get; set; }
        [Column("N_PurchaseType")]
        public int? NPurchaseType { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
    }
}
