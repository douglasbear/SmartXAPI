﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_LedgerGroupCategory")]
    public partial class AccLedgerGroupCategory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(50)]
        public string XCategoryCode { get; set; }
        [Column("X_CategoryName")]
        [StringLength(100)]
        public string XCategoryName { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_GroupTypeID")]
        public int? NGroupTypeId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
