using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVendoradvancepay
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_Type")]
        [StringLength(25)]
        public string XType { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_PurchaseID")]
        public int? NPurchaseId { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_BalanceAmount", TypeName = "money")]
        public decimal? NBalanceAmount { get; set; }
        [Column("N_PurchaseAmount", TypeName = "money")]
        public decimal? NPurchaseAmount { get; set; }
        [Column("N_PurchaseAmountCom", TypeName = "money")]
        public decimal? NPurchaseAmountCom { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_advanceamount", TypeName = "money")]
        public decimal? NAdvanceamount { get; set; }
        [Column("X_Code")]
        [StringLength(100)]
        public string XCode { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Required]
        [StringLength(9)]
        public string Status { get; set; }
    }
}
