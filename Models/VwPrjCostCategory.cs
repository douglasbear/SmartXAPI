﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjCostCategory
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(20)]
        public string XCategoryCode { get; set; }
        [Column("X_CategoryName")]
        [StringLength(500)]
        public string XCategoryName { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
