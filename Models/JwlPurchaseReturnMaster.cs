using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Jwl_PurchaseReturnMaster")]
    public partial class JwlPurchaseReturnMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CreditNoteId")]
        public int NCreditNoteId { get; set; }
        [Column("X_CreditNoteNo")]
        [StringLength(50)]
        public string XCreditNoteNo { get; set; }
        [Column("N_PurchaseId")]
        public int? NPurchaseId { get; set; }
        [Column("D_RetDate", TypeName = "datetime")]
        public DateTime? DRetDate { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("N_TotalReceived", TypeName = "money")]
        public decimal? NTotalReceived { get; set; }
        [Column("N_TotalReturnAmount", TypeName = "money")]
        public decimal? NTotalReturnAmount { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
    }
}
