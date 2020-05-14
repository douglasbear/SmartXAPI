using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_LedgerBehaviour")]
    public partial class AccLedgerBehaviour
    {
        [Key]
        [Column("N_LedgerBehaviourID")]
        public int NLedgerBehaviourId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
    }
}
