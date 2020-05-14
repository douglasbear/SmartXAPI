using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_StockOut")]
    public partial class InvStockOut
    {
        [Key]
        [Column("N_SoldItemsId")]
        public int NSoldItemsId { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_StockId")]
        public int? NStockId { get; set; }
        [Column("N_SalesId")]
        public int? NSalesId { get; set; }
        [Column("N_SalesDetailsId")]
        public int? NSalesDetailsId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Cost", TypeName = "decimal(20, 6)")]
        public decimal? NCost { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_Type")]
        [StringLength(20)]
        public string XType { get; set; }
        [Column("D_DateOut", TypeName = "datetime")]
        public DateTime? DDateOut { get; set; }

        [ForeignKey(nameof(NStockId))]
        [InverseProperty(nameof(InvStockMaster.InvStockOut))]
        public virtual InvStockMaster NStock { get; set; }
    }
}
