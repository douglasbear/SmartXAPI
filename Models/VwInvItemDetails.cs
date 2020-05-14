using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemDetails
    {
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_MainItemID")]
        public int? NMainItemId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemDetailsID")]
        public int? NItemDetailsId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("X_ClassName")]
        [StringLength(25)]
        public string XClassName { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_StockUnitID")]
        public int? NStockUnitId { get; set; }
        [Column("Stock Unit")]
        [StringLength(500)]
        public string StockUnit { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("N_LeadDays")]
        public int? NLeadDays { get; set; }
        [Column("N_TransitDays")]
        public int? NTransitDays { get; set; }
        public int? DeliveryDays { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_DeliveryDays")]
        public int? NDeliveryDays { get; set; }
        [Column("X_DisplayName")]
        [StringLength(100)]
        public string XDisplayName { get; set; }
        [Column("X_CategoryName")]
        [StringLength(100)]
        public string XCategoryName { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(50)]
        public string XPkeyCode { get; set; }
        [Column("N_PkeyID")]
        public int? NPkeyId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_RecycleQty")]
        public double? NRecycleQty { get; set; }
        [Column("N_WasteQty")]
        public double? NWasteQty { get; set; }
        [Column("N_AlternativeItemID")]
        public int? NAlternativeItemId { get; set; }
        [Column("X_AlternativeItemCode")]
        [StringLength(100)]
        public string XAlternativeItemCode { get; set; }
        [Column("X_AlternativeItemName")]
        [StringLength(800)]
        public string XAlternativeItemName { get; set; }
    }
}
