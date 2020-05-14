using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_LoyaltyPointMaster")]
    public partial class InvLoyaltyPointMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_PointMasterID")]
        public int NPointMasterId { get; set; }
        [Column("X_PointMasterCode")]
        [StringLength(50)]
        public string XPointMasterCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_MinimumApplicableAmt", TypeName = "money")]
        public decimal? NMinimumApplicableAmt { get; set; }
        [Column("N_PercOfPointsEarnings", TypeName = "decimal(18, 2)")]
        public decimal? NPercOfPointsEarnings { get; set; }
        [Column("N_AmtSavedPerPoint", TypeName = "money")]
        public decimal? NAmtSavedPerPoint { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
    }
}
