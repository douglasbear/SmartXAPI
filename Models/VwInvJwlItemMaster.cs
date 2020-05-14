using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvJwlItemMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("X_PurchaseDescription")]
        [StringLength(600)]
        public string XPurchaseDescription { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Column("X_TypeName")]
        [StringLength(100)]
        public string XTypeName { get; set; }
        [Column("N_ItemUnitID")]
        public int NItemUnitId { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("N_Karat")]
        public double? NKarat { get; set; }
        public int? Expr1 { get; set; }
        [Column("N_WastageRatePerc")]
        public double? NWastageRatePerc { get; set; }
        [Column("N_WastageRate", TypeName = "money")]
        public decimal? NWastageRate { get; set; }
        [Column("N_StoneProfitPerc")]
        public double? NStoneProfitPerc { get; set; }
        [Column("N_StoneRate", TypeName = "money")]
        public decimal? NStoneRate { get; set; }
        [Column("N_LabourCharge", TypeName = "money")]
        public decimal? NLabourCharge { get; set; }
        [Column("N_LabourChargePerc")]
        public double? NLabourChargePerc { get; set; }
        [Column("N_Weight")]
        public double? NWeight { get; set; }
        [Column("N_PurchasePrice", TypeName = "money")]
        public decimal? NPurchasePrice { get; set; }
        [Column("B_IsIMEI")]
        public bool? BIsImei { get; set; }
        public int? Expr2 { get; set; }
        [Column("N_ItemCost", TypeName = "money")]
        public decimal? NItemCost { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("X_ItemName_a")]
        [StringLength(100)]
        public string XItemNameA { get; set; }
        [Column("I_Image", TypeName = "image")]
        public byte[] IImage { get; set; }
        [Column("N_ReOrderQty")]
        public double? NReOrderQty { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("X_ClassName")]
        [StringLength(25)]
        public string XClassName { get; set; }
        [Column("N_MinMCPerc")]
        public double? NMinMcperc { get; set; }
        [Column("X_Color")]
        [StringLength(50)]
        public string XColor { get; set; }
    }
}
