using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwJwlPurchaseDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PurchaseID")]
        public int? NPurchaseId { get; set; }
        [Column("N_PurchaseDetailsID")]
        public int NPurchaseDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_Model")]
        [StringLength(100)]
        public string XModel { get; set; }
        [Column("N_Karat")]
        public double? NKarat { get; set; }
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
        [Column("N_MCPerc")]
        public double? NMcperc { get; set; }
        [Column("N_MCRate", TypeName = "money")]
        public decimal? NMcrate { get; set; }
        [Column("N_WastagePerc")]
        public double? NWastagePerc { get; set; }
        [Column("N_WastageRate", TypeName = "money")]
        public decimal? NWastageRate { get; set; }
        [Column("N_StoneProfitPerc")]
        public double? NStoneProfitPerc { get; set; }
        [Column("N_StoneRate", TypeName = "money")]
        public decimal? NStoneRate { get; set; }
        [Column("N_UnitRate", TypeName = "money")]
        public decimal? NUnitRate { get; set; }
        [Column("N_Quantity")]
        public int? NQuantity { get; set; }
        [Column("N_GoldAmount", TypeName = "money")]
        public decimal? NGoldAmount { get; set; }
        [Column("N_StoneAmount", TypeName = "money")]
        public decimal? NStoneAmount { get; set; }
        [Column("N_MakingCharge", TypeName = "money")]
        public decimal? NMakingCharge { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal? NDiscount { get; set; }
        [Column("N_TotalAmount", TypeName = "money")]
        public decimal? NTotalAmount { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_MCMinimumPerc")]
        public double? NMcminimumPerc { get; set; }
        [Column("X_ImgPath")]
        [StringLength(1000)]
        public string XImgPath { get; set; }
        [Column("X_ItemCode")]
        public int? XItemCode { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [StringLength(50)]
        public string ItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("X_ImgName")]
        [StringLength(100)]
        public string XImgName { get; set; }
        [Column("N_OtherExp", TypeName = "money")]
        public decimal? NOtherExp { get; set; }
    }
}
