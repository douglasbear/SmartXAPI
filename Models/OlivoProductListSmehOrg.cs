using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Olivo_ProductList_SMEH_org")]
    public partial class OlivoProductListSmehOrg
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
        public double? StockLocation1 { get; set; }
        [Column("Stock_Location_2")]
        public double? StockLocation2 { get; set; }
        [Column("Stock_Location_3")]
        public double? StockLocation3 { get; set; }
        [Column("Stock_Location_4")]
        public double? StockLocation4 { get; set; }
        [Column("Stock_Location_5")]
        public double? StockLocation5 { get; set; }
        [Column("Stock_Location_6")]
        public double? StockLocation6 { get; set; }
        [Column("Stock_Location_7")]
        public double? StockLocation7 { get; set; }
        [Column("Stock_Location_8")]
        public double? StockLocation8 { get; set; }
        [Column("Stock_Location_9")]
        public double? StockLocation9 { get; set; }
        [Column("Stock_Location_10")]
        public double? StockLocation10 { get; set; }
        [Column("Stock_Location_11")]
        public double? StockLocation11 { get; set; }
        [Column("Stock_Location_12")]
        public double? StockLocation12 { get; set; }
        [Column("Stock_Location_13")]
        public double? StockLocation13 { get; set; }
        [Column("Stock_Location_14")]
        public double? StockLocation14 { get; set; }
        [Column("Stock_Location_15")]
        public double? StockLocation15 { get; set; }
        [Column("Stock_Location_16")]
        public double? StockLocation16 { get; set; }
        [Column("Stock_Location_17")]
        public double? StockLocation17 { get; set; }
        [Column("Stock_Location_18")]
        public double? StockLocation18 { get; set; }
        [Column("Stock_Location_19")]
        public double? StockLocation19 { get; set; }
        [Column("Stock_Location_20")]
        public double? StockLocation20 { get; set; }
    }
}
