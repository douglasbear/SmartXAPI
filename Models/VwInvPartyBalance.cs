using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPartyBalance
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AccID")]
        public int? NAccId { get; set; }
        [Column("N_AccType")]
        public int NAccType { get; set; }
        [Column("Balance Amount", TypeName = "money")]
        public decimal? BalanceAmount { get; set; }
        [Required]
        [Column("Party Type")]
        [StringLength(8)]
        public string PartyType { get; set; }
    }
}
