using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDailySalesReport
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_CashAmount", TypeName = "money")]
        public decimal? NCashAmount { get; set; }
        [Column("N_BankAmount", TypeName = "money")]
        public decimal? NBankAmount { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("N_CashTypeID")]
        public int? NCashTypeId { get; set; }
    }
}
