using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Chart_Of_account_2")]
    public partial class ChartOfAccount2
    {
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_GroupID")]
        public int? NGroupId { get; set; }
        [StringLength(50)]
        public string AccountCode { get; set; }
        [Column("accountName")]
        [StringLength(100)]
        public string AccountName { get; set; }
        [StringLength(50)]
        public string ParentNo { get; set; }
        [Column("X_Level")]
        [StringLength(1000)]
        public string XLevel { get; set; }
    }
}
