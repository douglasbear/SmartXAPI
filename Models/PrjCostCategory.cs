using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_CostCategory")]
    public partial class PrjCostCategory
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
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
    }
}
