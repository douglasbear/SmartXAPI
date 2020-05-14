using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssetDashBoard
    {
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("D_PurchaseDate")]
        [StringLength(8000)]
        public string DPurchaseDate { get; set; }
        [Column("D_PlacedDate", TypeName = "datetime")]
        public DateTime? DPlacedDate { get; set; }
        [Column("N_Price")]
        [StringLength(30)]
        public string NPrice { get; set; }
        [Required]
        [Column("N_LifePeriod")]
        [StringLength(30)]
        public string NLifePeriod { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Required]
        [Column("X_CostcentreName")]
        [StringLength(100)]
        public string XCostcentreName { get; set; }
        [Column("X_GISRefNo")]
        [StringLength(50)]
        public string XGisrefNo { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Required]
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("AssetLedgerID")]
        public int? AssetLedgerId { get; set; }
        [StringLength(500)]
        public string AssetLedger { get; set; }
        [Column("PurchaseLedgerID")]
        public int? PurchaseLedgerId { get; set; }
        [StringLength(500)]
        public string PurchaseLedger { get; set; }
        public int? SalesLedgerId { get; set; }
        [StringLength(500)]
        public string SalesLedger { get; set; }
        [Column("X_make")]
        [StringLength(50)]
        public string XMake { get; set; }
        [Column("X_Model")]
        [StringLength(50)]
        public string XModel { get; set; }
        [Column("D_WarExpDate", TypeName = "datetime")]
        public DateTime? DWarExpDate { get; set; }
        [Column("D_LastMaintanance", TypeName = "datetime")]
        public DateTime? DLastMaintanance { get; set; }
        [Column("D_NextMaintanance", TypeName = "datetime")]
        public DateTime? DNextMaintanance { get; set; }
        [Column("X_DisposalRestrictions")]
        [StringLength(500)]
        public string XDisposalRestrictions { get; set; }
        [Column(TypeName = "money")]
        public decimal? Salesprice { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Required]
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Required]
        [Column("X_ItemName")]
        [StringLength(100)]
        public string XItemName { get; set; }
        [Column("N_AssetInventoryDetailsID")]
        public int? NAssetInventoryDetailsId { get; set; }
        [Column("N_AssetInventoryID")]
        public int? NAssetInventoryId { get; set; }
        [Column("N_BookValue")]
        [StringLength(30)]
        public string NBookValue { get; set; }
        [Column("D_InvoiceDate")]
        [StringLength(8000)]
        public string DInvoiceDate { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("Physical Location")]
        [StringLength(50)]
        public string PhysicalLocation { get; set; }
        [StringLength(500)]
        public string AccuDepLedgerName { get; set; }
        [Column("AccuDepLedgerID")]
        public int? AccuDepLedgerId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Required]
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Required]
        [Column("X_PlateNumber")]
        [StringLength(100)]
        public string XPlateNumber { get; set; }
        [Required]
        [Column("X_SerialNo")]
        [StringLength(100)]
        public string XSerialNo { get; set; }
        [Required]
        [StringLength(8)]
        public string Status { get; set; }
        [Required]
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
    }
}
