using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwItemMasterDisp
    {
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_DefaultId")]
        public int? NDefaultId { get; set; }
        [Column("X_TypeCode")]
        [StringLength(5)]
        public string XTypeCode { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(100)]
        public string XCategoryCode { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
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
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_ItemCost", TypeName = "money")]
        public decimal? NItemCost { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("B_ShowDefault")]
        public bool? BShowDefault { get; set; }
        [Column("B_OwnProduct")]
        public bool? BOwnProduct { get; set; }
        [Column("N_AssetID")]
        public int? NAssetId { get; set; }
        [Column("X_AssetName")]
        [StringLength(100)]
        public string XAssetName { get; set; }
    }
}
