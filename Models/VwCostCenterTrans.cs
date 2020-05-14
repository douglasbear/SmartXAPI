using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCostCenterTrans
    {
        [Required]
        [Column("N_Segment_3")]
        [StringLength(1)]
        public string NSegment3 { get; set; }
        [Required]
        [Column("N_Segment_4")]
        [StringLength(1)]
        public string NSegment4 { get; set; }
        [Required]
        [Column("N_VoucherID")]
        [StringLength(1)]
        public string NVoucherId { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("X_CostcentreName")]
        [StringLength(100)]
        public string XCostcentreName { get; set; }
        [Column("X_CostCentreCode")]
        [StringLength(50)]
        public string XCostCentreCode { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("X_BranchCode")]
        [StringLength(50)]
        public string XBranchCode { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_CostCenterTransID")]
        public int? NCostCenterTransId { get; set; }
        [Column("N_GridLineNo")]
        public int? NGridLineNo { get; set; }
        [Column("X_Naration")]
        [StringLength(250)]
        public string XNaration { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_InventoryID")]
        public int? NInventoryId { get; set; }
        [Column("N_InventoryType")]
        public int? NInventoryType { get; set; }
        [Column("N_InventoryDetailsID")]
        public int? NInventoryDetailsId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_VoucherDetailsID")]
        public int? NVoucherDetailsId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_AssetID")]
        public int NAssetId { get; set; }
        [Required]
        [Column("X_AssetCode")]
        [StringLength(1)]
        public string XAssetCode { get; set; }
        [Column("D_RepaymentDate", TypeName = "datetime")]
        public DateTime DRepaymentDate { get; set; }
        [Required]
        [Column("X_AssetName")]
        [StringLength(1)]
        public string XAssetName { get; set; }
    }
}
