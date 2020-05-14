using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesAmountCustomer
    {
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_SalesID")]
        public int NSalesId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_CommissionAmt", TypeName = "money")]
        public decimal? NCommissionAmt { get; set; }
        [Column("N_CommissionPer", TypeName = "money")]
        public decimal? NCommissionPer { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Required]
        [Column("X_CardNo")]
        [StringLength(20)]
        public string XCardNo { get; set; }
        [Column("N_TaxID")]
        public int? NTaxId { get; set; }
        [Column("N_PointsOut")]
        public int? NPointsOut { get; set; }
        [Column("N_AppliedAmt", TypeName = "money")]
        public decimal? NAppliedAmt { get; set; }
        [Column("N_AvailPoints")]
        public int? NAvailPoints { get; set; }
    }
}
