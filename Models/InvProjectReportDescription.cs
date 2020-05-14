using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ProjectReportDescription")]
    public partial class InvProjectReportDescription
    {
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_Order")]
        public int? NOrder { get; set; }
    }
}
