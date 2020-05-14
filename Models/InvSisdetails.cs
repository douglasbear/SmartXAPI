using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_SISDetails")]
    public partial class InvSisdetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SISID")]
        public int? NSisid { get; set; }
        [Key]
        [Column("N_SISDetailsID")]
        public int NSisdetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_QtyIssued")]
        public double? NQtyIssued { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column("N_UnitPrice", TypeName = "money")]
        public decimal? NUnitPrice { get; set; }
        [Column("N_TotalPrice", TypeName = "money")]
        public decimal? NTotalPrice { get; set; }
        [Column("N_PRSDetailsID")]
        public int? NPrsdetailsId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
    }
}
