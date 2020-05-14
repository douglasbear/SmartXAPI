using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_SalesReturnStockWise")]
    public partial class InvSalesReturnStockWise
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_DebitNoteId")]
        public int? NDebitNoteId { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_SalesDetailsId")]
        public int? NSalesDetailsId { get; set; }
        [Column("N_StockId")]
        public int? NStockId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
