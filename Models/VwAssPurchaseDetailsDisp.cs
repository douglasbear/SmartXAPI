using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssPurchaseDetailsDisp
    {
        [Column("N_AssetInventoryDetailsID")]
        public int NAssetInventoryDetailsId { get; set; }
        [Column("X_ItemName")]
        [StringLength(100)]
        public string XItemName { get; set; }
        [Column("X_Description")]
        [StringLength(150)]
        public string XDescription { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_PurchaseQty")]
        public double? NPurchaseQty { get; set; }
        [Column("N_LifePeriod")]
        public double? NLifePeriod { get; set; }
        [Column("D_PurchaseDate", TypeName = "datetime")]
        public DateTime? DPurchaseDate { get; set; }
        [Column("B_BegningbalEntry")]
        public bool? BBegningbalEntry { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("N_DepreciationAmt", TypeName = "money")]
        public decimal? NDepreciationAmt { get; set; }
        [Column("N_Bookvalue", TypeName = "money")]
        public decimal? NBookvalue { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_Depreciation")]
        public double? NDepreciation { get; set; }
        [Column("X_CategoryPrefix")]
        [StringLength(20)]
        public string XCategoryPrefix { get; set; }
        [Column("X_BranchCode")]
        [StringLength(50)]
        public string XBranchCode { get; set; }
        [Column("N_AssetInventoryID")]
        public int? NAssetInventoryId { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("X_CostCentreCode")]
        [StringLength(50)]
        public string XCostCentreCode { get; set; }
        [Column("X_CostcentreName")]
        [StringLength(100)]
        public string XCostcentreName { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
    }
}
