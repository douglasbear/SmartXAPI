using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvStockAdjustmentDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_RefNo")]
        [StringLength(50)]
        public string XRefNo { get; set; }
        [StringLength(8000)]
        public string AdjustDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_LoactionID")]
        public int? NLoactionId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_NewQty")]
        public double? NNewQty { get; set; }
        [Column("N_AdjustedQty")]
        public double? NAdjustedQty { get; set; }
        [Column("N_QtyOnHand")]
        public double? NQtyOnHand { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_AdjustmentID")]
        public int? NAdjustmentId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("N_ReasonID")]
        public int? NReasonId { get; set; }
        [StringLength(50)]
        public string Description { get; set; }
        [Column("X_Description_Ar")]
        [StringLength(50)]
        public string XDescriptionAr { get; set; }
        [Column("D_AdjustDate", TypeName = "datetime")]
        public DateTime? DAdjustDate { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("X_Department")]
        public string XDepartment { get; set; }
        [Column("X_LocationCode")]
        public string XLocationCode { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
    }
}
