using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_CRMDetails")]
    public partial class InvCrmdetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CRMID")]
        public int? NCrmid { get; set; }
        [Key]
        [Column("N_CRMDetailsID")]
        public int NCrmdetailsId { get; set; }
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
        [StringLength(500)]
        public string XItemRemarks { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("B_Processed")]
        public bool? BProcessed { get; set; }
    }
}
