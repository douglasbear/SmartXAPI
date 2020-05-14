using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_PayCodeAdjustmentDetails")]
    public partial class PayPayCodeAdjustmentDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_AdjustmentID")]
        public int NAdjustmentId { get; set; }
        [Key]
        [Column("N_AdjustmentDetailsID")]
        public int NAdjustmentDetailsId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_ProcessedAmount", TypeName = "money")]
        public decimal? NProcessedAmount { get; set; }
        [Column("N_CalculatedAmount", TypeName = "money")]
        public decimal? NCalculatedAmount { get; set; }
        [Column("N_AdjustmentAmount", TypeName = "money")]
        public decimal? NAdjustmentAmount { get; set; }
    }
}
