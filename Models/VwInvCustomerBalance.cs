using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvCustomerBalance
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AccType")]
        public int NAccType { get; set; }
        [Column("N_AccID")]
        public int? NAccId { get; set; }
        [Column("N_BalanceAmount", TypeName = "money")]
        public decimal? NBalanceAmount { get; set; }
    }
}
