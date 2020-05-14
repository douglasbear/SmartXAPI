using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccAccountWiseTotal
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Level")]
        [StringLength(100)]
        public string XLevel { get; set; }
        [Column("N_GroupID")]
        public int NGroupId { get; set; }
    }
}
