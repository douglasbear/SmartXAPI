using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ass_Transactions")]
    public partial class AssTransactions
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_ActionID")]
        public int NActionId { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("N_LifePeriod")]
        public double? NLifePeriod { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("X_Type")]
        [StringLength(50)]
        public string XType { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Reference")]
        [StringLength(50)]
        public string XReference { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_LedgerId")]
        public int? NLedgerId { get; set; }
        [Column("N_ItemId")]
        public int? NItemId { get; set; }

        [ForeignKey("NCompanyId,NLedgerId,NFnYearId")]
        [InverseProperty(nameof(AccMastLedger.AssTransactions))]
        public virtual AccMastLedger N { get; set; }
    }
}
