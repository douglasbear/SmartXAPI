using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ass_AssetMaster")]
    public partial class AssAssetMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("N_LifePeriod")]
        public double? NLifePeriod { get; set; }
        [Column("N_BookValue", TypeName = "money")]
        public decimal? NBookValue { get; set; }
        [Column("N_AssetInventoryDetailsID")]
        public int? NAssetInventoryDetailsId { get; set; }
        [Column("N_ItemCodeId")]
        public int? NItemCodeId { get; set; }
        [Column("X_Location")]
        [StringLength(50)]
        public string XLocation { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column("X_CategoryPrefix")]
        [StringLength(20)]
        public string XCategoryPrefix { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("D_PlacedDate", TypeName = "datetime")]
        public DateTime? DPlacedDate { get; set; }
        [Column("X_GISRefNo")]
        [StringLength(50)]
        public string XGisrefNo { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
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
        [Column("N_projectId")]
        public int? NProjectId { get; set; }
        [Column("N_MaxMaintenance", TypeName = "money")]
        public decimal? NMaxMaintenance { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_PlateNumber")]
        [StringLength(100)]
        public string XPlateNumber { get; set; }
        [Column("X_ContractNo")]
        [StringLength(100)]
        public string XContractNo { get; set; }
        [Column("X_HypothecatedTo")]
        [StringLength(100)]
        public string XHypothecatedTo { get; set; }
        [Column("N_DwnPayemnt", TypeName = "money")]
        public decimal? NDwnPayemnt { get; set; }
        [Column("N_EmiAmount", TypeName = "money")]
        public decimal? NEmiAmount { get; set; }
        [Column("D_EmiDueDate", TypeName = "datetime")]
        public DateTime? DEmiDueDate { get; set; }
        [Column("D_EmiStartDate", TypeName = "datetime")]
        public DateTime? DEmiStartDate { get; set; }
        [Column("D_EmiEndDate", TypeName = "datetime")]
        public DateTime? DEmiEndDate { get; set; }
        [Column("N_POrderID")]
        public int? NPorderId { get; set; }
        [Column("X_SerialNo")]
        [StringLength(100)]
        public string XSerialNo { get; set; }
    }
}
