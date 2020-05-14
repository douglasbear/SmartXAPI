using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_Mig_ItemList")]
    public partial class MigItemList
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Product_code")]
        [StringLength(200)]
        public string ProductCode { get; set; }
        [Column("Description_English")]
        [StringLength(800)]
        public string DescriptionEnglish { get; set; }
        [Column("Description_Arabic")]
        [StringLength(800)]
        public string DescriptionArabic { get; set; }
        [Column("Product_Type")]
        [StringLength(200)]
        public string ProductType { get; set; }
        [Column("Item_Category")]
        [StringLength(200)]
        public string ItemCategory { get; set; }
        [Column("Part_No")]
        [StringLength(200)]
        public string PartNo { get; set; }
        [StringLength(200)]
        public string Manufacture { get; set; }
        [Column("Default_Vendor")]
        [StringLength(200)]
        public string DefaultVendor { get; set; }
        [Column("Item_cost")]
        [StringLength(200)]
        public string ItemCost { get; set; }
        [Column("Selling_Price")]
        [StringLength(200)]
        public string SellingPrice { get; set; }
        [Column("Stock_Unit")]
        [StringLength(200)]
        public string StockUnit { get; set; }
        [Column("Purchase_Unit")]
        [StringLength(200)]
        public string PurchaseUnit { get; set; }
        [Column("Purchase_Unit_Qty")]
        [StringLength(200)]
        public string PurchaseUnitQty { get; set; }
        [Column("Sales_Unit")]
        [StringLength(200)]
        public string SalesUnit { get; set; }
        [Column("Sales_Unit_Qty")]
        [StringLength(200)]
        public string SalesUnitQty { get; set; }
        [StringLength(200)]
        public string Branch { get; set; }
        [Column("Opening_Stock")]
        [StringLength(200)]
        public string OpeningStock { get; set; }
        [StringLength(200)]
        public string Location { get; set; }
        [StringLength(200)]
        public string BatchCode { get; set; }
        [Column(TypeName = "date")]
        public DateTime? BatchExpiry { get; set; }
        [Column("N_companyID")]
        public int? NCompanyId { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("X_FullDescription")]
        [StringLength(800)]
        public string XFullDescription { get; set; }
    }
}
