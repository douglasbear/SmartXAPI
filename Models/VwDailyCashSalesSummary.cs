using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDailyCashSalesSummary
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_AmountDr", TypeName = "money")]
        public decimal? NAmountDr { get; set; }
        [Column("N_AmountCr", TypeName = "money")]
        public decimal? NAmountCr { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column(TypeName = "money")]
        public decimal? CashOpening { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
    }
}
