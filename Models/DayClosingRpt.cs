using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class DayClosingRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Required]
        [Column("X_EntryForm")]
        [StringLength(15)]
        public string XEntryForm { get; set; }
        [Column(TypeName = "money")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "money")]
        public decimal? Credit { get; set; }
        [Required]
        [Column("DATE")]
        [StringLength(1)]
        public string Date { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
