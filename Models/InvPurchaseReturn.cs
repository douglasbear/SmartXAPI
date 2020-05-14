using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PurchaseReturn")]
    public partial class InvPurchaseReturn
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_DebitNoteId")]
        public int NDebitNoteId { get; set; }
        [Column("X_DebitNoteNo")]
        [StringLength(50)]
        public string XDebitNoteNo { get; set; }
        [Column("N_PurchaseId")]
        public int? NPurchaseId { get; set; }
        [Column("N_PurchaseDetailsId")]
        public int? NPurchaseDetailsId { get; set; }
        [Column("N_RetQty")]
        public double? NRetQty { get; set; }
        [Column("D_RetDate", TypeName = "datetime")]
        public DateTime? DRetDate { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("N_TotalReceived", TypeName = "money")]
        public decimal? NTotalReceived { get; set; }

        [ForeignKey(nameof(NPurchaseId))]
        [InverseProperty(nameof(InvPurchase.InvPurchaseReturn))]
        public virtual InvPurchase NPurchase { get; set; }
    }
}
