using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccBudgeting
    {
        [Column("Account Code")]
        [StringLength(50)]
        public string AccountCode { get; set; }
        [StringLength(100)]
        public string Account { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }
        [StringLength(20)]
        public string Year { get; set; }
        [Column("X_Level")]
        [StringLength(100)]
        public string XLevel { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
