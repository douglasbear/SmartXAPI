using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccLedgerGroupCategory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
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
        [Column("N_ReportGroupID")]
        public int? NReportGroupId { get; set; }
        [Column("X_ReportGroupDesc")]
        [StringLength(50)]
        public string XReportGroupDesc { get; set; }
    }
}
