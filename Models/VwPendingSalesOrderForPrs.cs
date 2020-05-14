using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPendingSalesOrderForPrs
    {
        [Column("N_TotalSOQty")]
        public double? NTotalSoqty { get; set; }
        [Column("N_TotalReturnSOQty")]
        public double? NTotalReturnSoqty { get; set; }
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_TotalPRSQty")]
        public double? NTotalPrsqty { get; set; }
    }
}
