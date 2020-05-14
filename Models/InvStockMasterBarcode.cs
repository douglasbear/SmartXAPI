using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_StockMaster_Barcode")]
    public partial class InvStockMasterBarcode
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_Type")]
        [StringLength(50)]
        public string XType { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_Model")]
        [StringLength(50)]
        public string XModel { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_GoldWeight")]
        public double? NGoldWeight { get; set; }
        [Column("N_StoneWeight")]
        public double? NStoneWeight { get; set; }
        [Column("N_LessWeight")]
        public double? NLessWeight { get; set; }
        [Column("N_NetWeight")]
        public double? NNetWeight { get; set; }
        [Column("N_MCRate", TypeName = "money")]
        public decimal? NMcrate { get; set; }
        [Column("N_MCPerc")]
        public double? NMcperc { get; set; }
        [Column("N_StoneRate", TypeName = "money")]
        public decimal? NStoneRate { get; set; }
        [Column("N_StoneProfitPerc")]
        public double? NStoneProfitPerc { get; set; }
        [Column("N_WastageRate", TypeName = "money")]
        public decimal? NWastageRate { get; set; }
        [Column("N_WastageRatePerc")]
        public double? NWastageRatePerc { get; set; }
        [Column("N_PurchaseID")]
        public int? NPurchaseId { get; set; }
        [Column("N_PurchaseDetailsID")]
        public int? NPurchaseDetailsId { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_SalesDetailsID")]
        public int? NSalesDetailsId { get; set; }
        [Column("N_Karat")]
        public double? NKarat { get; set; }
        [Key]
        [Column("N_StockID")]
        public int NStockId { get; set; }
        [Column("N_UnitPPrice", TypeName = "money")]
        public decimal? NUnitPprice { get; set; }
        [Column("N_UnitSPrice", TypeName = "money")]
        public decimal? NUnitSprice { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("X_ImgPath")]
        [StringLength(1000)]
        public string XImgPath { get; set; }
        [Column("N_MCMinimumPerc")]
        public double? NMcminimumPerc { get; set; }
        [Column("B_Status")]
        public bool? BStatus { get; set; }
        [Column("X_Comments")]
        [StringLength(50)]
        public string XComments { get; set; }
    }
}
