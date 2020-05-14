using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesPaymentTypeRpt
    {
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
    }
}
