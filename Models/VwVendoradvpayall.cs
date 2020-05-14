using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVendoradvpayall
    {
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
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_BalanceAmount", TypeName = "money")]
        public decimal? NBalanceAmount { get; set; }
        [Column("X_Remarks")]
        [StringLength(50)]
        public string XRemarks { get; set; }
        [Column(TypeName = "money")]
        public decimal? NetAmount { get; set; }
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
        [Column(TypeName = "money")]
        public decimal? NetAmountCom { get; set; }
    }
}
