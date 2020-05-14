using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccStatementImportRulesList
    {
        [Column("N_BankID")]
        public int NBankId { get; set; }
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("X_Code")]
        [StringLength(50)]
        public string XCode { get; set; }
    }
}
