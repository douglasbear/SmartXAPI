using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Jeel_Arabic_English")]
    public partial class JeelArabicEnglish
    {
        [Column("sr_no")]
        public int? SrNo { get; set; }
        [Column("arabic")]
        [StringLength(500)]
        public string Arabic { get; set; }
        [Column("english")]
        [StringLength(500)]
        public string English { get; set; }
        [Column("ledger_code")]
        [StringLength(20)]
        public string LedgerCode { get; set; }
    }
}
