using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_Terminal")]
    public partial class InvTerminal
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_TerminalID")]
        public int NTerminalId { get; set; }
        [Column("X_TerminalCode")]
        [StringLength(500)]
        public string XTerminalCode { get; set; }
        [Column("X_TerminalName")]
        [StringLength(500)]
        public string XTerminalName { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
    }
}
