using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProjectTransferDetails
    {
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_TransferID")]
        public int? NTransferId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("Project To")]
        [StringLength(50)]
        public string ProjectTo { get; set; }
        [Column("Project From")]
        [StringLength(50)]
        public string ProjectFrom { get; set; }
        [Column("N_ProjectIDTo")]
        public int? NProjectIdto { get; set; }
        [Column("N_ProjectIDFrom")]
        public int? NProjectIdfrom { get; set; }
        [Column("N_TransferDetailsID")]
        public int NTransferDetailsId { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_StockUnitID")]
        public int? NStockUnitId { get; set; }
    }
}
