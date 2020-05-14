using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_YearWiseLedgerSettings")]
    public partial class AccYearWiseLedgerSettings
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_YWLID")]
        public int NYwlid { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [ForeignKey("NCompanyId,NLedgerId,NFnYearId")]
        [InverseProperty(nameof(AccMastLedger.AccYearWiseLedgerSettings))]
        public virtual AccMastLedger N { get; set; }
    }
}
