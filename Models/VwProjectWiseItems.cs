using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProjectWiseItems
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_RefID")]
        public int? NRefId { get; set; }
        [Column("X_RefNo")]
        [StringLength(50)]
        public string XRefNo { get; set; }
        [Column("D_TrnDate", TypeName = "datetime")]
        public DateTime? DTrnDate { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("x_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_ItemDiscAmt", TypeName = "money")]
        public decimal? NItemDiscAmt { get; set; }
        [Column("N_Cost")]
        public double? NCost { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Required]
        [Column("X_Type")]
        [StringLength(15)]
        public string XType { get; set; }
        [Column("X_FreeDescription")]
        [StringLength(1000)]
        public string XFreeDescription { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Column("N_PartyCode")]
        [StringLength(50)]
        public string NPartyCode { get; set; }
        [Column("N_PartyName")]
        [StringLength(100)]
        public string NPartyName { get; set; }
        [Column("B_IsSaveDraft")]
        public int? BIsSaveDraft { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("X_LedgerName")]
        [StringLength(500)]
        public string XLedgerName { get; set; }
    }
}
