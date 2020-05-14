using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssAssetItemSearch
    {
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_make")]
        [StringLength(50)]
        public string XMake { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("X_BranchCode")]
        [StringLength(50)]
        public string XBranchCode { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("N_BookValue", TypeName = "money")]
        public decimal? NBookValue { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("D_PurchaseDate", TypeName = "datetime")]
        public DateTime? DPurchaseDate { get; set; }
        [Column("N_LifePeriod")]
        public double? NLifePeriod { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("D_PlacedDate", TypeName = "datetime")]
        public DateTime? DPlacedDate { get; set; }
        [Column("X_ItemName")]
        [StringLength(100)]
        public string XItemName { get; set; }
        [Column("X_CostcentreName")]
        [StringLength(100)]
        public string XCostcentreName { get; set; }
        [Column("X_CostCentreCode")]
        [StringLength(50)]
        public string XCostCentreCode { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(8)]
        public string XStatus { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [StringLength(8000)]
        public string PlacedDate { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Required]
        [Column("STATUS")]
        [StringLength(9)]
        public string Status { get; set; }
        [Column("N_Depreciation")]
        public double? NDepreciation { get; set; }
        [StringLength(34)]
        public string LastDeprDate { get; set; }
        [Column("X_Description")]
        [StringLength(150)]
        public string XDescription { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Required]
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
    }
}
