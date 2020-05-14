﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesOrderDistribution
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_SPriceID")]
        public int? NSpriceId { get; set; }
    }
}
