using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_TransferStockDetails")]
    public partial class InvTransferStockDetails
    {
        public InvTransferStockDetails()
        {
            InvReceivableStockDetails = new HashSet<InvReceivableStockDetails>();
            InvStockOutByTransfer = new HashSet<InvStockOutByTransfer>();
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
        [Column("N_PRSDetailsID")]
        public int? NPrsdetailsId { get; set; }
        [Column("N_TruckID")]
        public int? NTruckId { get; set; }
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Column("N_IMEITo")]
        [StringLength(50)]
        public string NImeito { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }

        [ForeignKey(nameof(NItemId))]
        [InverseProperty(nameof(InvItemMaster.InvTransferStockDetails))]
        public virtual InvItemMaster NItem { get; set; }
        [ForeignKey(nameof(NTransferId))]
        [InverseProperty(nameof(InvTransferStock.InvTransferStockDetails))]
        public virtual InvTransferStock NTransfer { get; set; }
        [InverseProperty("NTransferDetails")]
        public virtual ICollection<InvReceivableStockDetails> InvReceivableStockDetails { get; set; }
        [InverseProperty("NTransferDetails")]
        public virtual ICollection<InvStockOutByTransfer> InvStockOutByTransfer { get; set; }
    }
}
