using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Jwl_PurchaseReturnDetails")]
    public partial class JwlPurchaseReturnDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CreditNoteDetailsID")]
        public int NCreditNoteDetailsId { get; set; }
        [Column("N_CreditNoteId")]
        public int? NCreditNoteId { get; set; }
        [Column("N_PurchaseDetailsId")]
        public int? NPurchaseDetailsId { get; set; }
        [Column("N_RetQty")]
        public double? NRetQty { get; set; }
        [Column("N_RetAmount", TypeName = "money")]
        public decimal? NRetAmount { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
    }
}
