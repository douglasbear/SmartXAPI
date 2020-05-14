using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwLoanDetails
    {
        [Column(TypeName = "money")]
        public decimal? Total { get; set; }
        [Column(TypeName = "money")]
        public decimal? LoanAmount { get; set; }
        [Column("balance", TypeName = "money")]
        public decimal? Balance { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_LoanTransID")]
        public int NLoanTransId { get; set; }
    }
}
