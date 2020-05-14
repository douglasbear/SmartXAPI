using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCustomerDiscSettings
    {
        [Column("N_MaxDiscCount")]
        public int? NMaxDiscCount { get; set; }
        [Column("N_MaxDiscPeriod")]
        public int? NMaxDiscPeriod { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_DiscID")]
        public int NDiscId { get; set; }
        [Column("N_DiscPerc", TypeName = "money")]
        public decimal? NDiscPerc { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_DiscCode")]
        [StringLength(50)]
        public string XDiscCode { get; set; }
    }
}
