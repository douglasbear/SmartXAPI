using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ItemMasterWHLink")]
    public partial class InvItemMasterWhlink
    {
        [Key]
        [Column("N_RowID")]
        public int NRowId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_WarehouseID")]
        public int? NWarehouseId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
