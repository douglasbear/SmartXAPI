using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBudgetingBrachRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        public int? NoOfDays { get; set; }
        [Column("N_BudAmtCurrent", TypeName = "money")]
        public decimal? NBudAmtCurrent { get; set; }
        [Column(TypeName = "money")]
        public decimal? BudgetAmtYear { get; set; }
    }
}
