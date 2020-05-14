using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCustomerDiscountMaster
    {
        [Column("N_CDMID")]
        public int? NCdmid { get; set; }
        [Column("D_DateTo", TypeName = "smalldatetime")]
        public DateTime? DDateTo { get; set; }
        [Column("D_DateFrom", TypeName = "smalldatetime")]
        public DateTime? DDateFrom { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_ProductID")]
        public int NProductId { get; set; }
        [Column("N_ItemPrice", TypeName = "money")]
        public decimal? NItemPrice { get; set; }
        [Column("N_DiscPerc")]
        public int? NDiscPerc { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("N_CustDiscountId")]
        public int NCustDiscountId { get; set; }
        [Column("X_CDMCode")]
        [StringLength(100)]
        public string XCdmcode { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        public int? Expr3 { get; set; }
    }
}
