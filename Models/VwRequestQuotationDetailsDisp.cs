using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRequestQuotationDetailsDisp
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_RequestId")]
        public int NRequestId { get; set; }
        [Column("X_RequestNo")]
        [StringLength(50)]
        public string XRequestNo { get; set; }
        [Column("N_QuotationID")]
        public int? NQuotationId { get; set; }
        [Column("D_RequestDate", TypeName = "smalldatetime")]
        public DateTime? DRequestDate { get; set; }
        [Column("N_TotalAmt", TypeName = "money")]
        public decimal? NTotalAmt { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Remarks")]
        [StringLength(200)]
        public string XRemarks { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        public int? Expr2 { get; set; }
        public int? Expr3 { get; set; }
    }
}
