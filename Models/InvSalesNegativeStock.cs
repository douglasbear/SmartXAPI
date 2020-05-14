using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_SalesNegativeStock")]
    public partial class InvSalesNegativeStock
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("B_Processed")]
        public bool? BProcessed { get; set; }
        [Column("B_Posted")]
        public bool? BPosted { get; set; }
        [Column("N_QtyProcessed")]
        public double? NQtyProcessed { get; set; }
        [Column("N_StockID")]
        public int? NStockId { get; set; }
        [Column("N_SalesDetailsID")]
        public int? NSalesDetailsId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("D_DateOut", TypeName = "datetime")]
        public DateTime? DDateOut { get; set; }
    }
}
