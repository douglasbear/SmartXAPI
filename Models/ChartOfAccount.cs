using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Chart_Of_Account")]
    public partial class ChartOfAccount
    {
        public int? Srl { get; set; }
        [StringLength(50)]
        public string AccountNo { get; set; }
        [StringLength(1000)]
        public string Name { get; set; }
        public int? Level { get; set; }
        [StringLength(50)]
        public string Type { get; set; }
        [StringLength(50)]
        public string ParentAccountNo { get; set; }
        [Column("N_GroupID")]
        public int? NGroupId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_Level")]
        [StringLength(1000)]
        public string XLevel { get; set; }
        [Column("N_ParentGroupID")]
        public int? NParentGroupId { get; set; }
    }
}
