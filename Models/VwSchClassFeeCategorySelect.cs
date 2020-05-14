﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchClassFeeCategorySelect
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FeeCategoryID")]
        public int NFeeCategoryId { get; set; }
        [Column("X_FeeCategory")]
        [StringLength(50)]
        public string XFeeCategory { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_FeeTypeID")]
        public int NFeeTypeId { get; set; }
        [Required]
        [Column("X_FeeType")]
        [StringLength(50)]
        public string XFeeType { get; set; }
        [Column("B_IsDefault")]
        public bool? BIsDefault { get; set; }
    }
}
