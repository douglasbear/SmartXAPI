using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwItemMaster
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
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_LocFrom")]
        public int? NLocFrom { get; set; }
        [Column("N_LocTo")]
        public int? NLocTo { get; set; }
        [Column("N_Price")]
        [StringLength(30)]
        public string NPrice { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Column("X_PurchaseDescription")]
        [StringLength(600)]
        public string XPurchaseDescription { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("N_ItemCost", TypeName = "money")]
        public decimal? NItemCost { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_MinimumMargin")]
        public double? NMinimumMargin { get; set; }
        [Column("X_Unit")]
        [StringLength(50)]
        public string XUnit { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("B_ShowDefault")]
        public bool? BShowDefault { get; set; }
        [Column("N_PriceDetailsID")]
        public int NPriceDetailsId { get; set; }
        [Column("X_LocFrom")]
        [StringLength(100)]
        public string XLocFrom { get; set; }
        [Column("X_LocTo")]
        [StringLength(100)]
        public string XLocTo { get; set; }
        [Column("D_DateFrom", TypeName = "datetime")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTo", TypeName = "datetime")]
        public DateTime? DDateTo { get; set; }
    }
}
