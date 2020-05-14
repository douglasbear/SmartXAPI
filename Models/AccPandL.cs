using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_PandL")]
    public partial class AccPandL
    {
        [Column("N_SlNo")]
        public int NSlNo { get; set; }
        [Column("X_Expense")]
        [StringLength(50)]
        public string XExpense { get; set; }
        [Column("N_ExpAmt", TypeName = "money")]
        public decimal? NExpAmt { get; set; }
        [Column("N_Expense", TypeName = "money")]
        public decimal? NExpense { get; set; }
        [Column("X_Income")]
        [StringLength(50)]
        public string XIncome { get; set; }
        [Column("N_IncAmt", TypeName = "money")]
        public decimal? NIncAmt { get; set; }
        [Column("N_Income", TypeName = "money")]
        public decimal? NIncome { get; set; }
    }
}
