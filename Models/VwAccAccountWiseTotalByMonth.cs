using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccAccountWiseTotalByMonth
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }
        [Column("N_Month")]
        [StringLength(30)]
        public string NMonth { get; set; }
    }
}
