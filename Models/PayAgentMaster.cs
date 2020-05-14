using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_AgentMaster")]
    public partial class PayAgentMaster
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_AgentID")]
        public int NAgentId { get; set; }
        [Column("X_AgentCode")]
        [StringLength(50)]
        public string XAgentCode { get; set; }
        [Column("X_AgentName")]
        [StringLength(50)]
        public string XAgentName { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
    }
}
