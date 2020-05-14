using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ItemPriceMaster")]
    public partial class InvItemPriceMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("N_PriceID")]
        public int NPriceId { get; set; }
        [Column("N_PriceVal", TypeName = "money")]
        public decimal? NPriceVal { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
