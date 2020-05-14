using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ItemDetails")]
    public partial class InvItemDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ItemDetailsID")]
        public int NItemDetailsId { get; set; }
        [Column("N_MainItemID")]
        public int? NMainItemId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_AlternativeItemID")]
        public int? NAlternativeItemId { get; set; }
        [Column("N_RecycleQty")]
        public double? NRecycleQty { get; set; }
        [Column("N_WasteQty")]
        public double? NWasteQty { get; set; }

        [ForeignKey(nameof(NMainItemId))]
        [InverseProperty(nameof(InvItemMaster.InvItemDetails))]
        public virtual InvItemMaster NMainItem { get; set; }
    }
}
