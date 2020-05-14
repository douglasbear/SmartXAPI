using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesBudgetCustomer
    {
        [Column("X_CustomerName")]
        [StringLength(1000)]
        public string XCustomerName { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
    }
}
