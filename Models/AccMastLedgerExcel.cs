using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_MastLedger_Excel")]
    public partial class AccMastLedgerExcel
    {
        [Column("X_LedgerCode")]
        [StringLength(100)]
        public string XLedgerCode { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
    }
}
