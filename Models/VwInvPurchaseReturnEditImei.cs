using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseReturnEditImei
    {
        [Column("X_CreditNoteNo")]
        [StringLength(50)]
        public string XCreditNoteNo { get; set; }
        [Column("D_RetDate", TypeName = "datetime")]
        public DateTime? DRetDate { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_RetQty")]
        public double? NRetQty { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_PPrice", TypeName = "money")]
        public decimal? NPprice { get; set; }
        [Column("N_PurchaseID")]
        public int? NPurchaseId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_PurchaseDetailsID")]
        public int? NPurchaseDetailsId { get; set; }
        [Column("N_MRP", TypeName = "money")]
        public decimal? NMrp { get; set; }
        [Column("N_SPrice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_UnitQty")]
        public double? NUnitQty { get; set; }
        [Column("X_BaseUnit")]
        [StringLength(500)]
        public string XBaseUnit { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_IMEI")]
        [StringLength(50)]
        public string XImei { get; set; }
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        public double? RetQty { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
    }
}
