using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_SRNDetails")]
    public partial class InvSrndetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_SRNDetailsID")]
        public int NSrndetailsId { get; set; }
        [Column("N_SRNID")]
        public int? NSrnid { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_Qty")]
        public int? NQty { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_QtyDisp")]
        public int? NQtyDisp { get; set; }
    }
}
