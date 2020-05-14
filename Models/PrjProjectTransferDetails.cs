using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_ProjectTransferDetails")]
    public partial class PrjProjectTransferDetails
    {
        public PrjProjectTransferDetails()
        {
            PrjStockOutByTransfer = new HashSet<PrjStockOutByTransfer>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TransferID")]
        public int? NTransferId { get; set; }
        [Key]
        [Column("N_TransferDetailsID")]
        public int NTransferDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_ItemDiscAmt", TypeName = "money")]
        public decimal? NItemDiscAmt { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_MainItemID")]
        public int? NMainItemId { get; set; }
        [Column("N_MainQty")]
        public double? NMainQty { get; set; }
        [Column("N_MainSPrice", TypeName = "money")]
        public decimal? NMainSprice { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("X_ItemRemarks")]
        [StringLength(250)]
        public string XItemRemarks { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_ProjectIDTo")]
        public int? NProjectIdto { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }

        [ForeignKey(nameof(NItemId))]
        [InverseProperty(nameof(InvItemMaster.PrjProjectTransferDetails))]
        public virtual InvItemMaster NItem { get; set; }
        [ForeignKey(nameof(NTransferId))]
        [InverseProperty(nameof(PrjProjectTransfer.PrjProjectTransferDetails))]
        public virtual PrjProjectTransfer NTransfer { get; set; }
        [InverseProperty("NTransferDetails")]
        public virtual ICollection<PrjStockOutByTransfer> PrjStockOutByTransfer { get; set; }
    }
}
