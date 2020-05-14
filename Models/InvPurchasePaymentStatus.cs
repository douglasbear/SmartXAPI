using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PurchasePaymentStatus")]
    public partial class InvPurchasePaymentStatus
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnyearID")]
        public int NFnyearId { get; set; }
        [Key]
        [Column("N_PaymentStatusID")]
        public int NPaymentStatusId { get; set; }
        [Column("X_StatusCode")]
        [StringLength(50)]
        public string XStatusCode { get; set; }
        [Column("N_PaymentID")]
        public int? NPaymentId { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("D_RefDate", TypeName = "datetime")]
        public DateTime? DRefDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_CollectedBy")]
        [StringLength(100)]
        public string XCollectedBy { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
    }
}
