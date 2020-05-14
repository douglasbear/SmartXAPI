using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_StockMaster")]
    public partial class InvStockMaster
    {
        public InvStockMaster()
        {
            InvStockOut = new HashSet<InvStockOut>();
            InvStockOutByTransfer = new HashSet<InvStockOutByTransfer>();
            PrjStockOutByTransfer = new HashSet<PrjStockOutByTransfer>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_StockID")]
        public int NStockId { get; set; }
        [Column("X_Type")]
        [StringLength(50)]
        public string XType { get; set; }
        [Column("N_InventoryID")]
        public int? NInventoryId { get; set; }
        [Column("N_InventoryDetailsID")]
        public int? NInventoryDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_OpenStock")]
        public double? NOpenStock { get; set; }
        [Column("N_CurrentStock")]
        public double? NCurrentStock { get; set; }
        [Column("N_LPrice", TypeName = "decimal(20, 6)")]
        public decimal? NLprice { get; set; }
        [Column("N_SPrice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_SalesDetailsID")]
        public int? NSalesDetailsId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("D_DateIn", TypeName = "datetime")]
        public DateTime? DDateIn { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }

        [ForeignKey(nameof(NItemId))]
        [InverseProperty(nameof(InvItemMaster.InvStockMaster))]
        public virtual InvItemMaster NItem { get; set; }
        [InverseProperty("NStock")]
        public virtual ICollection<InvStockOut> InvStockOut { get; set; }
        [InverseProperty("NStock")]
        public virtual ICollection<InvStockOutByTransfer> InvStockOutByTransfer { get; set; }
        [InverseProperty("NStock")]
        public virtual ICollection<PrjStockOutByTransfer> PrjStockOutByTransfer { get; set; }
    }
}
