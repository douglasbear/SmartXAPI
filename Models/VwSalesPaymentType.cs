using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesPaymentType
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_SalesID")]
        public int NSalesId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_CommissionAmt", TypeName = "money")]
        public decimal? NCommissionAmt { get; set; }
        [Column("N_CommissionPer")]
        public int? NCommissionPer { get; set; }
        [Column("N_SalesAmountID")]
        public int? NSalesAmountId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
    }
}
