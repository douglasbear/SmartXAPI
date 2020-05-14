using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_TmpOpeningBalance")]
    public partial class AccTmpOpeningBalance
    {
        [Column("X_Descr")]
        [StringLength(50)]
        public string XDescr { get; set; }
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("N_OpeningBalance", TypeName = "money")]
        public decimal? NOpeningBalance { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PeriodID")]
        public int? NPeriodId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }

        [ForeignKey("NCompanyId,NLedgerId,NFnYearId")]
        public virtual AccMastLedger N { get; set; }
    }
}
