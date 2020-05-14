using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_AssemblyDetails")]
    public partial class InvAssemblyDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AssemblyID")]
        public int? NAssemblyId { get; set; }
        [Key]
        [Column("N_AssemblyDetailsID")]
        public int NAssemblyDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Qtyused")]
        public double? NQtyused { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("B_IsProcess")]
        public bool? BIsProcess { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_RecycleQty")]
        public double? NRecycleQty { get; set; }
        [Column("N_WastageQty")]
        public double? NWastageQty { get; set; }
        [Column("N_AltMainItemID")]
        public double? NAltMainItemId { get; set; }

        [ForeignKey(nameof(NAssemblyId))]
        [InverseProperty(nameof(InvAssembly.InvAssemblyDetails))]
        public virtual InvAssembly NAssembly { get; set; }
        [ForeignKey(nameof(NItemId))]
        [InverseProperty(nameof(InvItemMaster.InvAssemblyDetails))]
        public virtual InvItemMaster NItem { get; set; }
    }
}
