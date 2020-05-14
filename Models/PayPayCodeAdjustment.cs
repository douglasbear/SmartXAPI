using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_PayCodeAdjustment")]
    public partial class PayPayCodeAdjustment
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_AdjustmentID")]
        public int NAdjustmentId { get; set; }
        [Column("X_AdjustmentCode")]
        public string XAdjustmentCode { get; set; }
        [Column("D_AdjustmentDate", TypeName = "datetime")]
        public DateTime? DAdjustmentDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
    }
}
