using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Jwl_SalesDetails")]
    public partial class JwlSalesDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Key]
        [Column("N_SalesDetailsID")]
        public int NSalesDetailsId { get; set; }
        [Column("N_StockID")]
        public int? NStockId { get; set; }
        [Column("N_UnitRate", TypeName = "money")]
        public decimal? NUnitRate { get; set; }
        [Column("N_MCUnitRate", TypeName = "money")]
        public decimal? NMcunitRate { get; set; }
        [Column("N_WastageUnitRate", TypeName = "money")]
        public decimal? NWastageUnitRate { get; set; }
        [Column("N_Qty")]
        public int? NQty { get; set; }
        [Column("N_StoneCharges", TypeName = "money")]
        public decimal? NStoneCharges { get; set; }
        [Column("N_MCDiscPerc")]
        public double? NMcdiscPerc { get; set; }
        [Column("N_MCDiscount", TypeName = "money")]
        public decimal? NMcdiscount { get; set; }
        [Column("N_TotalAmount", TypeName = "money")]
        public decimal? NTotalAmount { get; set; }
        [Column("N_SaleOrderID")]
        public int? NSaleOrderId { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Barcode")]
        [StringLength(100)]
        public string XBarcode { get; set; }
        [Column("N_GoldWeight")]
        public double? NGoldWeight { get; set; }
        [Column("N_MakingCharge", TypeName = "money")]
        public decimal? NMakingCharge { get; set; }
    }
}
