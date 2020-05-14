using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalaryDepartment
    {
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("N_payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("percentge", TypeName = "money")]
        public decimal Percentge { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
