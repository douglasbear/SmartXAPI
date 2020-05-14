using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_Mig_ProductList")]
    public partial class MigProductList
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Product_code")]
        [StringLength(200)]
        public string ProductCode { get; set; }
        [Column("Description_English")]
        [StringLength(200)]
        public string DescriptionEnglish { get; set; }
        [Column("Description_Arabic")]
        [StringLength(200)]
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
        [Column("Stock_Location_1")]
        [StringLength(200)]
        public string StockLocation1 { get; set; }
        [Column("Stock_Location_2")]
        [StringLength(200)]
        public string StockLocation2 { get; set; }
        [Column("Stock_Location_3")]
        [StringLength(200)]
        public string StockLocation3 { get; set; }
        [Column("Stock_Location_4")]
        [StringLength(200)]
        public string StockLocation4 { get; set; }
        [Column("Stock_Location_5")]
        [StringLength(200)]
        public string StockLocation5 { get; set; }
        [Column("Stock_Location_6")]
        [StringLength(200)]
        public string StockLocation6 { get; set; }
        [Column("Stock_Location_7")]
        [StringLength(200)]
        public string StockLocation7 { get; set; }
        [Column("Stock_Location_8")]
        [StringLength(200)]
        public string StockLocation8 { get; set; }
        [Column("Stock_Location_9")]
        [StringLength(200)]
        public string StockLocation9 { get; set; }
        [Column("Stock_Location_10")]
        [StringLength(200)]
        public string StockLocation10 { get; set; }
        [Column("Stock_Location_11")]
        [StringLength(200)]
        public string StockLocation11 { get; set; }
        [Column("Stock_Location_12")]
        [StringLength(200)]
        public string StockLocation12 { get; set; }
        [Column("Stock_Location_13")]
        [StringLength(200)]
        public string StockLocation13 { get; set; }
        [Column("Stock_Location_14")]
        [StringLength(200)]
        public string StockLocation14 { get; set; }
        [Column("Stock_Location_15")]
        [StringLength(200)]
        public string StockLocation15 { get; set; }
        [Column("Stock_Location_16")]
        [StringLength(200)]
        public string StockLocation16 { get; set; }
        [Column("Stock_Location_17")]
        [StringLength(200)]
        public string StockLocation17 { get; set; }
        [Column("Stock_Location_18")]
        [StringLength(200)]
        public string StockLocation18 { get; set; }
        [Column("Stock_Location_19")]
        [StringLength(200)]
        public string StockLocation19 { get; set; }
        [Column("Stock_Location_20")]
        [StringLength(200)]
        public string StockLocation20 { get; set; }
    }
}
