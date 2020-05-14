using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_BalanceAdjustmentMasterDetails")]
    public partial class InvBalanceAdjustmentMasterDetails
    {
        [Key]
        [Column("N_AdjustmentDetailsId")]
        public int NAdjustmentDetailsId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Remarks")]
        [StringLength(50)]
        public string XRemarks { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_AdjustmentId")]
        public int NAdjustmentId { get; set; }
        [Column("N_AmountF", TypeName = "money")]
        public decimal? NAmountF { get; set; }

        [ForeignKey(nameof(NAdjustmentId))]
        [InverseProperty(nameof(InvBalanceAdjustmentMaster.InvBalanceAdjustmentMasterDetails))]
        public virtual InvBalanceAdjustmentMaster NAdjustment { get; set; }
    }
}
