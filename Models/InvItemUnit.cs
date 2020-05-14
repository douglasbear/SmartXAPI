using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ItemUnit")]
    public partial class InvItemUnit
    {
        public InvItemUnit()
        {
            InvItemMaster = new HashSet<InvItemMaster>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ItemUnitID")]
        public int NItemUnitId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("B_BaseUnit")]
        public bool? BBaseUnit { get; set; }
        [Column("N_BaseUnitID")]
        public int? NBaseUnitId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_SellingPrice", TypeName = "money")]
        public decimal? NSellingPrice { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }

        [InverseProperty("NItemUnit")]
        public virtual ICollection<InvItemMaster> InvItemMaster { get; set; }
    }
}
