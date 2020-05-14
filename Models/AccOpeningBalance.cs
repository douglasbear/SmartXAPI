using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_OpeningBalance")]
    public partial class AccOpeningBalance
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_PeriodID")]
        public int NPeriodId { get; set; }
        [Key]
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("N_OpeningBalance", TypeName = "money")]
        public decimal? NOpeningBalance { get; set; }
        [Column("N_CurrentBalance", TypeName = "money")]
        public decimal? NCurrentBalance { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
