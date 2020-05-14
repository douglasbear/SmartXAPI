using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_StockOutByTransfer")]
    public partial class InvStockOutByTransfer
    {
        [Key]
        [Column("N_TransferedItemsId")]
        public int NTransferedItemsId { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_StockId")]
        public int? NStockId { get; set; }
        [Column("N_TransferId")]
        public int? NTransferId { get; set; }
        [Column("N_TransferDetailsId")]
        public int? NTransferDetailsId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [ForeignKey(nameof(NStockId))]
        [InverseProperty(nameof(InvStockMaster.InvStockOutByTransfer))]
        public virtual InvStockMaster NStock { get; set; }
        [ForeignKey(nameof(NTransferDetailsId))]
        [InverseProperty(nameof(InvTransferStockDetails.InvStockOutByTransfer))]
        public virtual InvTransferStockDetails NTransferDetails { get; set; }
    }
}
