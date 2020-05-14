using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvStoreIssueReport
    {
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("X_Purpose")]
        [StringLength(50)]
        public string XPurpose { get; set; }
        [Column("PRSEntryDate", TypeName = "datetime")]
        public DateTime? PrsentryDate { get; set; }
        [Column("N_DepQty")]
        public double? NDepQty { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_Rack")]
        [StringLength(50)]
        public string XRack { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [StringLength(500)]
        public string StoreUnit { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("X_Department")]
        public string XDepartment { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("X_CostCentreCode")]
        [StringLength(50)]
        public string XCostCentreCode { get; set; }
        [Column("X_CostcentreName")]
        [StringLength(100)]
        public string XCostcentreName { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("N_TransferId")]
        public int? NTransferId { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("N_PRSDetailsID")]
        public int NPrsdetailsId { get; set; }
        [Column("N_PRSID")]
        public int NPrsid { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("D_PRSDate", TypeName = "datetime")]
        public DateTime? DPrsdate { get; set; }
        [Column("D_TransferDate", TypeName = "smalldatetime")]
        public DateTime? DTransferDate { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_PartNo")]
        [StringLength(250)]
        public string XPartNo { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("X_CustPONo")]
        [StringLength(50)]
        public string XCustPono { get; set; }
        [Column("N_UnitSPrice", TypeName = "money")]
        public decimal? NUnitSprice { get; set; }
        [Column("N_DeliveryDays")]
        public double? NDeliveryDays { get; set; }
        [Column("N_ItemManufacturerID")]
        public int? NItemManufacturerId { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Expr1 { get; set; }
        [Column("X_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EntryDate { get; set; }
        [Column("N_ApprovalLevelID")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [StringLength(50)]
        public string DeliveryNoteno { get; set; }
    }
}
