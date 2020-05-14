using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwJwlStockTransferDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_StockID")]
        public int? NStockId { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("N_Qty")]
        public int? NQty { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_UnitPPrice", TypeName = "money")]
        public decimal? NUnitPprice { get; set; }
        [Column("N_Karat")]
        public double? NKarat { get; set; }
        [Column("X_Model")]
        [StringLength(50)]
        public string XModel { get; set; }
        [Column("N_GoldWeight")]
        public double? NGoldWeight { get; set; }
        [Column("N_TransferID")]
        public int? NTransferId { get; set; }
    }
}
