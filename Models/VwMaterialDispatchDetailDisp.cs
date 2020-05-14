using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMaterialDispatchDetailDisp
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_DispatchId")]
        public int NDispatchId { get; set; }
        [Column("X_DispatchNo")]
        [StringLength(50)]
        public string XDispatchNo { get; set; }
        [Column("D_DispatchDate", TypeName = "smalldatetime")]
        public DateTime? DDispatchDate { get; set; }
        [Column("N_ProjectId")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_ProjectDescription")]
        [StringLength(250)]
        public string XProjectDescription { get; set; }
        [Column("N_BillAmt", TypeName = "money")]
        public decimal? NBillAmt { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_LocaionInGrid")]
        public int? NLocaionInGrid { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        public int? Expr2 { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_MainItemID")]
        public int? NMainItemId { get; set; }
        [Column("N_MainQty")]
        public double? NMainQty { get; set; }
        [Column("N_DispatchReqID")]
        public int? NDispatchReqId { get; set; }
        public int? Expr3 { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("X_ItemRemarks")]
        [StringLength(250)]
        public string XItemRemarks { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_DispatchDetailsID")]
        public int? NDispatchDetailsId { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("N_StockUnitID")]
        public int? NStockUnitId { get; set; }
        [Column("N_RsID")]
        public int? NRsId { get; set; }
        [Column("X_RsNo")]
        [StringLength(15)]
        public string XRsNo { get; set; }
        [Column("N_RsDetailsID")]
        public int? NRsDetailsId { get; set; }
    }
}
